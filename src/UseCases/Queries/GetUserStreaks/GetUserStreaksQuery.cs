using Domain;
using Infrastructure.Interfaces.DataAccess;
using MediatR;

namespace UseCases.Queries.GetUserStreaks;

public record GetUserStreaksQuery(int UserId) : IRequest<ReadStreaksDto>;

public class GetUserStreaksQueryHandler(IDbContext dbContext) : IRequestHandler<GetUserStreaksQuery, ReadStreaksDto>
{
    public async Task<ReadStreaksDto> Handle(GetUserStreaksQuery query, CancellationToken cancellationToken)
    {
        var streaks = await dbContext.GetUserStreaks(query.UserId, cancellationToken);
        if (streaks.Count == 0)
        {
            return new ReadStreaksDto(0, 0);
        }

        var max = streaks.Max(GetLenght);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var current = streaks.FirstOrDefault(x => x.To == today || x.To == today.AddDays(-1));
        var currentStreakLen = current == default ? 0 : GetLenght(current);
        return new ReadStreaksDto(currentStreakLen, max);

        int GetLenght(DateRange streak)
        {
            return streak.To.DayNumber - streak.From.DayNumber + 1;
        }
    }
}