using Domain.Entities;
using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using UseCases.Notifications;
using UseCases.Services;

namespace UseCases.ProcessPostUpdate;

public record ProcessPostUpdateCommand(Message Message) : IRequest;

public class ProcessPostUpdateCommandHandler(
    IDbContext dbContext,
    IMediator notificationPublisher,
    ILogger<ProcessPostUpdateCommandHandler> logger,
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
                } else if (pair.old != null && pair.newEntry == null)
                {
                    participant.ReadEntries.Remove(pair.old);
                }
                else if (pair.old == null && pair.newEntry != null)
                {
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
        var notification = new ReadIntervalsUpdatedNotification(report.Date, request.Message.Chat.Id, request.Message.Id);
        await notificationPublisher.Publish(notification, cancellationToken);
    }
}