using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UseCases.Queries.GetUserDailyChaptersRead;

public class GetUserDailyChaptersReadQueryHandler(IDbContext dbContext)
    : IRequestHandler<GetUserDailyChaptersReadQuery, ICollection<DayChaptersReadDto>>
{
    public async Task<ICollection<DayChaptersReadDto>> Handle(GetUserDailyChaptersReadQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.ReadEntries
            .Where(x => x.ParticipantId == request.UserId)
            .GroupBy(x => x.Date)
            .Select(x => new DayChaptersReadDto(x.Key, x.Sum(entry => entry.EndChapter - entry.StartChapter + 1)))
            .ToListAsync(cancellationToken);
    }
}