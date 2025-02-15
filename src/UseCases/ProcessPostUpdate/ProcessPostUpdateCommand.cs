using Domain.Entities;
using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UseCases.Services;

namespace UseCases.ProcessPostUpdate;

public record ProcessPostUpdateCommand(string Text) : IRequest;

public class ProcessPostUpdateCommandHandler(
    IDbContext dbContext,
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
            var parsedReport = reportParser.Parse(request.Text, books);
            if (parsedReport == null)
            {
                logger.LogInformation("failed to parse report from post: {PostMessage}", request.Text);
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

            if (participant.ReadEntries.Count == newIntervals.Count)
            {
                if (newIntervals.All(x => participant.ReadEntries.Any(existing =>
                        (existing.BookId, existing.StartChapter, existing.EndChapter) ==
                        (x.Book, x.StartChapter, x.EndChapter))))
                {
                    continue;
                }
            }

            // TODO use full outer join
            participant.ReadEntries.Clear();
            foreach (var newInterval in newIntervals)
            {
                participant.ReadEntries.Add(new ReadEntry
                {
                    Date = report.Date,
                    BookId = newInterval.Book,
                    StartChapter = newInterval.StartChapter,
                    EndChapter = newInterval.EndChapter
                });
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}