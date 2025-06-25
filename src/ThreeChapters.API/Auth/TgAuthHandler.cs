using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Web;
using Infrastructure.Interfaces.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Telegram.Bot.Extensions.LoginWidget;

namespace ThreeChapters.API.Auth;

public class TgAuthHandler(
    IOptionsMonitor<TgAuthOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IMemoryCache memoryCache,
    IDbContext dbContext)
    : AuthenticationHandler<TgAuthOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var twa = Request.Headers.Authorization.ToString();
        var queryString = HttpUtility.ParseQueryString(twa);
        var dict = queryString.AllKeys
            .Where(x => x != null)
            .ToDictionary(key => key!, key => queryString[key]!);

        var widget = new LoginWidget(Options.BotToken);
        var auth = widget.CheckAuthorization(dict);

        if (auth != Authorization.Valid)
        {
            return AuthenticateResult.Fail("Invalid auth");
        }

        var tgId = long.Parse(dict["id"]);
        var claims = new List<Claim>
        {
            new Claim("tg_id", tgId.ToString())
        };


        var participant = await memoryCache.GetOrCreateAsync($"participant_{tgId}", async entry =>
        {
            var participant = await dbContext.Participants.FirstOrDefaultAsync(x => x.TelegramId == tgId);
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return participant;
        });

        if (participant != null)
        {
            Context.Items["participant"] = participant;
            claims.Add(new Claim("user_id", participant.Id.ToString()));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}