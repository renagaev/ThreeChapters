using Domain.Entities;
using Framework;
using Infrastructure.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ThreeChapters.API.Auth;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor, IDbContext dbContext) : ICurrentUserProvider
{
    public Task<Participant?> GetCurrentUser()
    {
        var tgId = GetCurrentUserTelegramId();
        if (tgId == null)
        {
            return Task.FromResult<Participant?>(null);
        }

        return dbContext.Participants.FirstOrDefaultAsync(x => x.TelegramId == tgId);
    }

    public long? GetCurrentUserTelegramId()
    {
        var context = httpContextAccessor.HttpContext;
        var claim = context?.User.FindFirst("tg_id");
        if (claim != null)
        {
            return long.Parse(claim.Value);
        }

        return null;
    }
}