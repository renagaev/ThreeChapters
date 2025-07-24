using MediatR;

namespace UseCases.Queries.GetUserBibleProgress;

public record GetUserBibleProgressQuery(int UserId): IRequest<BibleProgressStats>;