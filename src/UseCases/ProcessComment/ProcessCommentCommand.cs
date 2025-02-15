using System.Text.RegularExpressions;
using Domain.Entities;
using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using UseCases.Services;

namespace UseCases.ProcessComment;

public record ProcessCommentCommand(Message Message) : IRequest;

public class ProcessCommentCommandHandler(
    ITelegramBotClient botClient,
    IDbContext dbContext,
    IIntervalParser intervalParser,
    IntervalMerger merger,
    ILogger<ProcessCommentCommandHandler> logger)
    : IRequestHandler<ProcessCommentCommand>
{
    public async Task Handle(ProcessCommentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received comment from user {userId} {username} with text {text}", request.Message.From.Id, request.Message.From.Username, request.Message.Text);
        var message = request.Message;
        if (message.Text is not { Length: > 0 } text)
        {
            logger.LogInformation("Skipping empty message");
            return;
        }

        var user = await dbContext.Participants.FirstOrDefaultAsync(x => x.TelegramId == message.From.Id,
            cancellationToken);
        if (user is null)
        {
            logger.LogInformation("Skipping message from unknown user: {TelegramUsername}", message.From.Username);
            return;
        }

        var books = dbContext.Books.ToList();
        var intervals = intervalParser.Parse(text, books);
        if (intervals.Count == 0)
        {
            logger.LogInformation("Skipping message with no read entries, message: {MessageText}", text);
            return;
        }

        var mergedIntervals = merger.Merge(intervals, books);
        await UpdateMessage(message, user, books, mergedIntervals, cancellationToken);
    }

    private async Task UpdateMessage(Message message, Participant participant, ICollection<Book> books, ICollection<ReadInterval> readIntervals, CancellationToken cancellationToken)
    {
        var bookById = books.ToDictionary(x => x.Id, x => x.Title);
        var renderedIntervals = string.Join(", ", readIntervals.Select(x => x switch
        {
            _ when x.StartBook == x.EndBook && x.StartChapter == x.EndChapter => $"{bookById[x.StartBook]} {x.StartChapter}",
            _ when x.StartBook == x.EndBook => $"{bookById[x.StartBook]} {x.StartChapter}-{x.EndChapter}",
            _ => $"{bookById[x.StartBook]} {x.StartChapter} - {bookById[x.EndBook]} {x.EndChapter}"
        }));
        var source = message.ReplyToMessage!.Text!;
        var newMessage = Regex.Replace(source, $@"[{Constants.ReadMark}{Constants.UnreadMark}]\s*{participant.Name}(:.*)*", $"{Constants.ReadMark} {participant.Name}: {renderedIntervals}");
        if (source == newMessage)
        {
            logger.LogError("Failed to find  user in message {User}", participant.Name);
            return;
        }

        var sourceMessage = message.ReplyToMessage.ForwardOrigin as MessageOriginChannel;
        await botClient.EditMessageText(sourceMessage!.Chat.Id, sourceMessage.MessageId, newMessage, cancellationToken: cancellationToken);
    }
}