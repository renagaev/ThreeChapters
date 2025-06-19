using System.Globalization;
using System.Text.RegularExpressions;
using Domain.Entities;
using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace UseCases.UpdateSeries;

public class UpdateSeriesNotificationHandler(IDbContext dbContext, ITelegramBotClient botClient)
    : INotificationHandler<ReadIntervalsUpdatedNotification>, IRequestHandler<PostSeriesMessageCommand>
{
    public async Task Handle(ReadIntervalsUpdatedNotification notification, CancellationToken cancellationToken) =>
        await UpdateSeries(notification.DailyPost.Date, notification.DailyPost.ChatId, notification.DailyPost.MessageId, cancellationToken);
    
    public async Task Handle(PostSeriesMessageCommand request, CancellationToken cancellationToken)
    {
        var source = request.Message.ForwardOrigin as MessageOriginChannel;
        var sourcePost =
            await dbContext.DailyPosts.FirstOrDefaultAsync(x => x.MessageId == source.MessageId, cancellationToken);

        var date = sourcePost?.Date;
        if (date == null)
        {
            var match = Regex.Match(request.Message.Text!, @"(\d+) (\w+) (\d\d\d\d)");
            if (match.Success)
            {
                date = DateOnly.ParseExact(match.Value, "d MMMM yyyy", CultureInfo.GetCultureInfo("ru-RU"));
            }
            else
            {
                return;
            }
            
        }

        var exists = await dbContext.SeriesMessages.AnyAsync(x => x.Date == date, cancellationToken);
        if (exists)
        {
            return;
        }

        await UpdateSeries(date.Value, request.Message.Chat.Id, request.Message.Id, cancellationToken);
    }

    private async Task UpdateSeries(DateOnly date, ChatId chatId, int messageId, CancellationToken cancellationToken)
    {
        var rawSeries = await dbContext.Participants
            .Where(x => x.IsActive)
            .Select(x => new
            {
                participant = x,
                dates = x.ReadEntries.Select(x => x.Date)
                    .Where(x => x <= date)
                    .Distinct()
            }).ToListAsync(cancellationToken);

        var lengths = new List<(Participant participant, int curr, int max)>();
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
            var currLength = curr == default ? 0 : curr.End.DayNumber - curr.Start.DayNumber + 1;
            var max = folded.Count != 0 ? folded.Max(x => x.End.DayNumber - x.Start.DayNumber + 1) : 0;
            lengths.Add((series.participant, currLength, max));
        }

        var maxLen = lengths.Max(x => x.participant.Name.Length) + 1;
        var rows = new List<string>
        {
            $"{"Имя".PadRight(maxLen)}| Дней",
            $"{"-".PadRight(maxLen, '-')}|----"
        };
        foreach (var (participant, length, maxLength) in lengths.OrderBy(x => x.participant.Id))
        {
            var isMax = (length == maxLength && length != 0);
            var lenStr = length.ToString().PadRight(3);
            if (isMax) lenStr += "🔥";
            rows.Add($"{participant.Name.PadRight(maxLen)}| {lenStr}");
        }

        var table = string.Join("\n", rows.Select(x => $"`{x}`"));
        var messageText = "Серии - количество дней подряд без пропуска\n🔥- лучшая серия участника\n\n" + table;

        var existingMessage =
            await dbContext.SeriesMessages.FirstOrDefaultAsync(x => x.Date == date, cancellationToken);
        if (existingMessage == null)
        {
            var message = await botClient.SendMessage(chatId, messageText, parseMode: ParseMode.Markdown,
                replyParameters: new ReplyParameters
                {
                    ChatId = chatId,
                    MessageId = messageId
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
            try
            {
                await botClient.EditMessageText(new ChatId(existingMessage.ChatId), existingMessage.MessageId,
                    messageText,
                    ParseMode.Markdown, cancellationToken: cancellationToken);
            }
            catch (ApiRequestException e) when (e.Message.Contains("not modified"))
            {
                // TODO handle cases like this
                // ignore
            }
        }
    }
}