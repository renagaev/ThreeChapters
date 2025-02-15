using System.Text;
using Domain.Entities;
using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UseCases.Notifications;

namespace UseCases.UpdateSeries;

public class UpdateSeriesNotificationHandler(IDbContext dbContext, ITelegramBotClient botClient)
    : INotificationHandler<ReadIntervalsUpdatedNotification>
{
    public async Task Handle(ReadIntervalsUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var date = notification.Date;
        var rawSeries = await dbContext.Participants.Select(x => new
        {
            participant = x,
            dates = x.ReadEntries.Select(x => x.Date)
                .Where(x => x <= notification.Date)
                .Distinct()
        }).ToListAsync(cancellationToken);

        var lengths = new List<(Participant participant, int length)>();
        foreach (var series in rawSeries)
        {
            var folded = series.dates.Order().Aggregate(new Stack<(DateOnly Start, DateOnly End)>(), (acc, curr) =>
            {
                if (acc.Count == 0)
                {
                    acc.Push((curr, curr));
                    return acc;
                }

                var last = acc.Pop();
                if (last.End.AddDays(1) == curr)
                {
                    acc.Push((last.Start, curr));
                    return acc;
                }

                acc.Push(last);
                acc.Push((curr, curr));
                return acc;
            });

            var curr = folded.FirstOrDefault(x => x.Start <= date && (x.End >= date || x.End == date.AddDays(-1)));
            var length = curr == default ? 0 : curr.End.DayNumber - curr.Start.DayNumber + 1;
            lengths.Add((series.participant, length));
        }

        var maxLen = lengths.Max(x => x.participant.Name.Length) + 1;
        var table = new StringBuilder();
        table.Append($"| {"Имя".PadRight(maxLen)}| Дней|\n");
        table.Append($"|{"-".PadRight(maxLen, '-')}|-----|\n");
        foreach (var (participant, length) in lengths.OrderBy(x => x.participant.Id))
        {
            table.Append($"| {participant.Name.PadRight(maxLen)}| {length.ToString().PadRight(4)}|\n");
        }

        var messageText = $"Дней подряд\n\n<pre>{table}</pre>";

        var existingMessage =
            await dbContext.SeriesMessages.FirstOrDefaultAsync(x => x.Date == date, cancellationToken);
        if (existingMessage == null)
        {
            var message = await botClient.SendMessage(notification.ChatId, messageText, parseMode: ParseMode.Html,
                replyParameters: new ReplyParameters
                {
                    ChatId = notification.ChatId,
                    MessageId = notification.MessageId
                }, cancellationToken: cancellationToken);
            dbContext.SeriesMessages.Add(new SeriesMessage
            {
                ChatId = message.Chat.Id,
                Date = date,
                MessageId = message.Id
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            await botClient.EditMessageText(new ChatId(existingMessage.ChatId), existingMessage.MessageId, messageText,
                ParseMode.Html, cancellationToken: cancellationToken);
        }
    }
}