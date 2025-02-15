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
        var rows = new List<string>
        {
            $"{"Имя".PadRight(maxLen)}| Дней",
            $"{"-".PadRight(maxLen, '-')}|----"
        };
        foreach (var (participant, length) in lengths.OrderBy(x => x.participant.Id))
        {
            rows.Add($"{participant.Name.PadRight(maxLen)}| {length.ToString(),-4}");
        }

        var table = string.Join("\n", rows.Select(x => $"`{x}`"));
        var messageText = "Серии - количество дней подряд без пропуска\n\n" + table;

        var existingMessage = await dbContext.SeriesMessages.FirstOrDefaultAsync(x => x.Date == date, cancellationToken);
        if (existingMessage == null)
        {
            var message = await botClient.SendMessage(notification.ChatId, messageText, parseMode: ParseMode.Markdown,
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
                ParseMode.Markdown, cancellationToken: cancellationToken);
        }
    }
}