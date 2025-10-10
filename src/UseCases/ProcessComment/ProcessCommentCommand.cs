using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Domain.Entities;
using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using UseCases.Services;
using UseCases.UpdateSeries;

namespace UseCases.ProcessComment;

public record ProcessCommentCommand(Message Message) : IRequest;

public class ProcessCommentCommandHandler(
    ITelegramBotClient botClient,
    IDbContext dbContext,
    IIntervalParser intervalParser,
    IntervalMerger merger,
    IntervalSplitter splitter,
    DailyPostRenderer dailyPostRenderer,
    IMediator mediator,
    ILogger<ProcessCommentCommandHandler> logger)
    : IRequestHandler<ProcessCommentCommand>
{
    public async Task Handle(ProcessCommentCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        if (!IsMessageValid(message, out var text, out var sourceMessage))
        {
            logger.LogInformation("Received invalid comment: {Text}", text ?? "null");
            return;
        }

        var user = await dbContext.Participants
            .FirstOrDefaultAsync(x => x.TelegramId == message.From!.Id, cancellationToken);

        if (user is null)
        {
            logger.LogInformation("Skipping message from unknown user: {TelegramUsername}", message.From!.Username);
            return;
        }

        if (user.IsAdmin && TryParseAdminCommand(text, out var username))
        {
            user = await dbContext.Participants.FirstOrDefaultAsync(
                x => x.Name.ToLower() == username.ToLower(), cancellationToken);
            if (user is null)
            {
                return;
            }
        }

        var books = await dbContext.Books.ToListAsync(cancellationToken);

        ICollection<ReadInterval> intervals = [];
        if (text.Contains("clear"))
        {
            intervals = [];
        }
        else
        {
            intervals = intervalParser.Parse(text, books);
            if (intervals.Count == 0)
            {
                logger.LogInformation("Skipping message with no read entries: {Text}", text);
                return;
            }
        }

        var dailyPost = await dbContext.DailyPosts
            .FirstOrDefaultAsync(x => x.MessageId == sourceMessage.MessageId && x.ChatId == sourceMessage.Chat.Id, cancellationToken);

        if (dailyPost is null)
        {
            logger.LogInformation("Skipping message from unknown daily post: {MessageId}", sourceMessage.MessageId);
            return;
        }

        var mergedIntervals = merger.Merge(intervals, books);
        var readEntries = mergedIntervals
            .SelectMany(x => splitter.Split(x, books))
            .Select(x => new ReadEntry
            {
                ParticipantId = user.Id,
                BookId = x.Book,
                StartChapter = x.StartChapter,
                EndChapter = x.EndChapter,
                Date = dailyPost.Date
            })
            .ToList();

        user.IsActive = true;
        await UpdateUserReadEntries(user.Id, dailyPost.Date, readEntries, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await UpdateDailyPostMessage(dailyPost, books, cancellationToken);
        await mediator.Publish(new ReadIntervalsUpdatedNotification(dailyPost), cancellationToken);
    }

    private bool IsMessageValid(Message message, [NotNullWhen(true)] out string? text,
        out MessageOriginChannel sourceMessage)
    {
        if (message.ReplyToMessage?.ForwardOrigin is not MessageOriginChannel channelOrigin)
        {
            text = null;
            sourceMessage = null!;
            return false;
        }

        sourceMessage = channelOrigin;
        text = message.Text ?? message.Caption;

        return !string.IsNullOrWhiteSpace(text);
    }

    private async Task UpdateUserReadEntries(long userId, DateOnly date, List<ReadEntry> newEntries,
        CancellationToken ct)
    {
        var existingEntries = await dbContext.ReadEntries
            .Where(e => e.ParticipantId == userId && e.Date == date)
            .ToListAsync(ct);

        dbContext.ReadEntries.RemoveRange(existingEntries);
        dbContext.ReadEntries.AddRange(newEntries);
    }

    private bool TryParseAdminCommand(string text, out string username)
    {
        var match = Regex.Match(text, @"отметить ([А-Яа-я|\w|\s]+):.*");
        if (match.Success)
        {
            username = match.Groups[1].Value;
            return true;
        }

        username = string.Empty;
        return false;
    }

    private async Task UpdateDailyPostMessage(DailyPost dailyPost, ICollection<Book> books, CancellationToken ct)
    {
        var participants = await dbContext.Participants
            .Where(p => p.IsActive)
            .Include(p => p.ReadEntries.Where(e => e.Date == dailyPost.Date))
            .ToListAsync(ct);

        var rendered = dailyPostRenderer.RenderDailyMessage(dailyPost.Date, participants, books);

        await botClient.EditMessageText(
            chatId: dailyPost.ChatId,
            messageId: dailyPost.MessageId,
            text: rendered,
            cancellationToken: ct);
    }
}