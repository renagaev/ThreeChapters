using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Telegram.Bot.Extensions.LoginWidget;

namespace ThreeChapters.API.Auth;

public class TgAuthHandler(
    IOptionsMonitor<TgAuthOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<TgAuthOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var twa = Request.Headers.Authorization.ToString();
        if (twa.Length == 0)
        {
            return Task.FromResult(AuthenticateResult.Fail("no auth header"));
        }
        var queryString = HttpUtility.ParseQueryString(twa);
        
        var dataDict = new SortedDictionary<string, string>(
            queryString.AllKeys.ToDictionary(x => x!, x => queryString[x]!),
            StringComparer.Ordinal);
        var constantKey = "WebAppData";
        var dataCheckString = string.Join(
            '\n', dataDict.Where(x => x.Key != "hash")
                .Select(x => $"{x.Key}={x.Value}"));

        var secretKey = HMACSHA256.HashData(Encoding.UTF8.GetBytes(constantKey), Encoding.UTF8.GetBytes(OptionsMonitor.CurrentValue.BotToken));

        var generatedHash = HMACSHA256.HashData(secretKey, Encoding.UTF8.GetBytes(dataCheckString));

        var actualHash = Convert.FromHexString(dataDict["hash"]);

        if (!actualHash.SequenceEqual(generatedHash))
        {
            return Task.FromResult(AuthenticateResult.Fail("error"));
        }
        
        var tgUser = JsonSerializer.Deserialize<TgUser>(dataDict["user"]);
        var claims = new List<Claim>
        {
            new("tg_id", tgUser!.id.ToString())
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private record TgUser(long id);
}