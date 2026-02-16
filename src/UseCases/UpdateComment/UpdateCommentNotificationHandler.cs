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
using UseCases.Services;
using UseCases.UpdateSeries;

namespace UseCases.UpdateComment;

public class UpdateCommentNotificationHandler(
    IDbContext dbContext,
    ITelegramBotClient botClient,
    HabitPowerCalculator powerCalculator)
    : INotificationHandler<ReadIntervalsUpdatedNotification>, IRequestHandler<PostSeriesMessageCommand>
{
    public async Task Handle(ReadIntervalsUpdatedNotification notification, CancellationToken cancellationToken) =>
        await UpdateSeries(notification.DailyPost.Date, notification.DailyPost.ChatId, notification.DailyPost.MessageId,
            cancellationToken);

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
                    .ToList()
            }).ToListAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var powers = rawSeries.Select(x => new
            {
                x.participant,
                graph = powerCalculator.GetPowerGraph(x.dates, today)
            })
            .OrderByDescending(x => x.graph[^1].Value)
            .ToList();


        var maxLen = powers.Max(x => x.participant.Name.Length) + 1;
        var rows = new List<string>
        {
            $"{"Имя".PadRight(maxLen)}| %",
            $"{"-".PadRight(maxLen, '-')}|----"
        };
        foreach (var pair in powers)
        {
            var currentValue = pair.graph[^1].Value;
            var fire = currentValue >= 0.85m ? "🔥" : "";
            var valueFormatted = (currentValue * 100).ToString("F1");
            rows.Add($"{pair.participant.Name.PadRight(maxLen)}| {valueFormatted.PadRight(6)} {fire}");
        }

        var table = string.Join("\n", rows.Select(x => $"`{x}`"));
        var messageText = "Регулярность. Чем чаще читаешь, тем больше значение\n🔥- 85%, около трех недель без пропуска\n\n" + table;

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