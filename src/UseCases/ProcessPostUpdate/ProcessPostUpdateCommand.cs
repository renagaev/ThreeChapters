using System.Text.RegularExpressions;
using Domain.Entities;
using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UseCases.Notifications;
using UseCases.Services;

namespace UseCases.ProcessPostUpdate;

public record ProcessPostUpdateCommand(Message Message) : IRequest;

public class ProcessPostUpdateCommandHandler(
    IDbContext dbContext,
    IMediator notificationPublisher,
    ILogger<ProcessPostUpdateCommandHandler> logger,
    ITelegramBotClient botClient,
    IntervalSplitter splitter,
    IntervalMerger merger,
    ReportParser reportParser)
    : IRequestHandler<ProcessPostUpdateCommand>
{
    public async Task Handle(ProcessPostUpdateCommand request, CancellationToken cancellationToken)
    {
        var books = await dbContext.Books.ToListAsync(cancellationToken);
        Report report = null!;
        try
        {
            var parsedReport = reportParser.Parse(request.Message.Text!, books);
            if (parsedReport == null)
            {
                logger.LogInformation("failed to parse report from post: {PostMessage}", request.Message.Text);
                return;
            }

            report = parsedReport;
        }
        catch (Exception e)
        {
            logger.LogInformation(e, "error during report parsing: {ErrorMessage}", e.Message);
        }

        var existingItems = dbContext.Participants
            .Include(x => x.ReadEntries.Where(x => x.Date == report.Date))
            .ToList();

        var intervalsUpdated = false;
        var allNewIntervals = new List<SingleBookInterval>();
        foreach (var item in report.Items)
        {
            var participant = existingItems.FirstOrDefault(x =>
                string.Equals(x.Name, item.User, StringComparison.InvariantCultureIgnoreCase));

            if (participant == null)
            {
                participant = new Participant
                {
                    Name = item.User,
                    IsActive = true,
                };
                dbContext.Participants.Add(participant);
            }

            var newIntervals = merger.Merge(item.Intervals, books)
                .SelectMany(x => splitter.Split(x, books))
                .ToList();
            allNewIntervals.AddRange(newIntervals);


            var oldLookup = participant.ReadEntries.ToLookup(x => (x.BookId, x.StartChapter, x.EndChapter));
            var newLookup = newIntervals.ToLookup(x => (x.Book, x.StartChapter, x.EndChapter));

            var keys = oldLookup
                .Select(x => x.Key)
                .Union(newLookup.Select(x => x.Key))
                .ToHashSet();

            var joined = from key in keys
                from old in oldLookup[key].DefaultIfEmpty()
                from newEntry in newLookup[key].DefaultIfEmpty()
                select new { old, newEntry };
            
            foreach (var pair in joined)
            {
                if (pair.old != null && pair.newEntry != null)
                {
                    continue;
                }

                if (pair.old != null && pair.newEntry == null)
                {
                    intervalsUpdated = true;
                    participant.ReadEntries.Remove(pair.old);
                }
                else if (pair.old == null && pair.newEntry != null)
                {
                    intervalsUpdated = true;
                    participant.ReadEntries.Add(new ReadEntry
                    {
                        Date = report.Date,
                        BookId = pair.newEntry.Book,
                        StartChapter = pair.newEntry.StartChapter,
                        EndChapter = pair.newEntry.EndChapter
                    });
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        if (intervalsUpdated || report.Items.All(x=> x.Intervals.Count == 0)) 
        {
            await UpdateReadCount(request.Message, report, allNewIntervals, cancellationToken);
            var notification = new ReadIntervalsUpdatedNotification(report.Date, request.Message.Chat.Id, request.Message.Id);
            await notificationPublisher.Publish(notification, cancellationToken);
        }
    }

    private async Task UpdateReadCount(Message message, Report report, ICollection<SingleBookInterval> intervals, CancellationToken cancellationToken)
    { 
        var readUsers = report.Items.Count(x => x.Intervals.Count != 0);
        var totalUsers = report.Items.Count;
        var totalChapters = intervals.Sum(x => x.EndChapter - x.StartChapter + 1);

        var match = Regex.Match(message.Text, @"\d+/\d+, прочитан\w \d+ глав\w?");
        var end = (totalChapters % 10) switch
        {
            1 => "а",
            2 => "ы",
            3 => "ы",
            4 => "ы",
            _ => "",
        };
        var read = (totalChapters % 10) switch
        {
            1 => "прочитана",
            _ => "прочитано"
        };
        var infoText = $"{readUsers}/{totalUsers}, {read} {totalChapters} глав{end}";
        
        var newText = match.Success ? message.Text.Replace(match.Value, infoText) : $"{message.Text}\n\n{infoText}";
        if (newText == message.Text)
        {
            return;
        }

        var sourceMessage = message.ForwardOrigin as MessageOriginChannel;
        await botClient.EditMessageText(sourceMessage.Chat.Id, sourceMessage.MessageId, newText, cancellationToken: cancellationToken);
    }
}