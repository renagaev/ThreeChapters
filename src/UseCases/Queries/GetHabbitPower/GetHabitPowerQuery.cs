using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UseCases.Services;

namespace UseCases.Queries.GetHabbitPower;

public record GetHabitPowerQuery(long UserId) : IRequest<HabbitPowerDto>;

public record HabbitPowerDto(decimal Current);

public class GetHabbitPowerQueryHandler(IDbContext dbContext, HabitPowerCalculator calculator)
    : IRequestHandler<GetHabitPowerQuery, HabbitPowerDto>
{
    public async Task<HabbitPowerDto> Handle(GetHabitPowerQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var positiveDates = await dbContext.ReadEntries
            .Where(x => x.ParticipantId == request.UserId)
            .Where(x => x.Date <= today)
            .Select(x => x.Date)
            .Distinct()
            .ToListAsync(cancellationToken);

        var res = calculator.GetPowerGraph(positiveDates, today);
        var todayValue = res.First(x => x.Date == today).Value;

        return new HabbitPowerDto(todayValue);
    }
}