using Domain.Entities;
using Framework;

namespace ThreeChapters.API.Auth;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public Participant? GetCurrentUser()
    {
        var context = httpContextAccessor.HttpContext;
        if (context?.Items.TryGetValue("participant", out var participant) == true)
        {
            return participant as Participant;
        }

        return null;
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