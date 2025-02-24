using MediatR;
using Telegram.Bot.Types;

namespace UseCases.Notifications;

public record DailyPostCreatedNotification(DateOnly Date, ChatId ChatId, int MessageId) : INotification;