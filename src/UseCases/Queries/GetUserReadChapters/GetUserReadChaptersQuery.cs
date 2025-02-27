using MediatR;

namespace UseCases.Queries.GetUserReadChapters;

public record GetUserReadChaptersQuery(int UserId): IRequest<ICollection<ReadBookChapters>>;