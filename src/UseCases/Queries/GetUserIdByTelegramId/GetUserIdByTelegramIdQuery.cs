using MediatR;

namespace UseCases.Queries.GetUserIdByTelegramId;

public record GetUserIdByTelegramIdQuery(long TelegramId): IRequest<long?>;