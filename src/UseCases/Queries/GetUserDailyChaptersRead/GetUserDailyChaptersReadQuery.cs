using MediatR;

namespace UseCases.Queries.GetUserDailyChaptersRead;

public record GetUserDailyChaptersReadQuery(long UserId): IRequest<ICollection<DayChaptersReadDto>>;