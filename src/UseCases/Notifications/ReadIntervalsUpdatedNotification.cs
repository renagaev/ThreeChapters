using MediatR;
using Telegram.Bot.Types;

namespace UseCases.Notifications;

public record ReadIntervalsUpdatedNotification(DateOnly Date, ChatId ChatId, long MessageId) : INotification;