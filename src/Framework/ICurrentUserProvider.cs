using Domain.Entities;

namespace Framework;

public interface ICurrentUserProvider
{
    Participant? GetCurrentUser();
    long? GetCurrentUserTelegramId();
}