using Domain.Entities;

namespace Framework;

public interface ICurrentUserProvider
{
    Task<Participant?> GetCurrentUser();
    long? GetCurrentUserTelegramId();
}