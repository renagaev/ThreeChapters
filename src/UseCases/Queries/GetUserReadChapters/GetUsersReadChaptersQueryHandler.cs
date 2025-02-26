using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UseCases.Queries.GetUserReadChapters;

public class GetUsersReadChaptersQueryHandler(IDbContext dbContext)
    : IRequestHandler<GetUserReadChaptersQuery, ICollection<ReadBookChapters>>
{
    public async Task<ICollection<ReadBookChapters>> Handle(GetUserReadChaptersQuery request,
        CancellationToken cancellationToken)
    {
        var readEntries = await dbContext.ReadEntries
            .Where(x => x.Participant.Id == request.UserId)
            .ToListAsync(cancellationToken);

        var res = new List<ReadBookChapters>();
        foreach (var grouping in readEntries.GroupBy(x => x.BookId))
        {
            var bookId = grouping.Key;
            var set = new HashSet<int>();
            foreach (var entry in grouping)
            {
                set.UnionWith(Enumerable.Range(entry.StartChapter, entry.EndChapter - entry.StartChapter + 1));
            }

            res.Add(new ReadBookChapters(bookId, set));
        }

        return res;
    }
}