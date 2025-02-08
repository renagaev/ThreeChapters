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
    ReadParser readParser,
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

        var user = await dbContext.Participants.FirstOrDefaultAsync(x => x.Id == message.From.Id,
            cancellationToken);
        if (user is null)
        {
            logger.LogInformation("Skipping message from unknown user: {TelegramUsername}", message.From.Username);
            return;
        }

        var books = dbContext.Books.ToList();
        var intervals = readParser.Parse(text, books);
        if (intervals.Count == 0)
        {
            logger.LogInformation("Skipping message with no read entries, message: {MessageText}", text);
            return;
        }

        var bookByName = books.ToDictionary(x => x.Title, x => x.Id);
        var readIntervals = intervals
            .Select(x => new ReadInterval(bookByName[x.StartBook], x.StartChapter, bookByName[x.EndBook], x.EndChapter))
            .ToList();

        var mergedIntervals = merger.Merge(readIntervals, books);
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
        var newMessage = Regex.Replace(source, $@"[✅❔]\s*{participant.Name}(:.*)*", $"✅ {participant.Name}: {renderedIntervals}");
        if (source == newMessage)
        {
            logger.LogError("Failed to find  user in message {User}", participant.Name);
            return;
        }

        var sourceMessage = message.ReplyToMessage.ForwardOrigin as MessageOriginChannel;
        await botClient.EditMessageText(sourceMessage!.Chat.Id, sourceMessage.MessageId, newMessage, cancellationToken: cancellationToken);
    }
}