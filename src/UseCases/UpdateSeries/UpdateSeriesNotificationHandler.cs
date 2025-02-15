using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Telegram.Bot;
using UseCases.Notifications;

namespace UseCases.UpdateSeries;

public class UpdateSeriesNotificationHandler(IDbContext dbContext, ITelegramBotClient botClient)
    : INotificationHandler<ReadIntervalsUpdatedNotification>
{
    public Task Handle(ReadIntervalsUpdatedNotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}