using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UseCases.Queries.GetUserIdByTelegramId;

public class GetUserIdByTelegramIdQueryHandler(IDbContext dbContext)
    : IRequestHandler<GetUserIdByTelegramIdQuery, long?>
{
    public async Task<long?> Handle(GetUserIdByTelegramIdQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Participants.Where(x => x.TelegramId == request.TelegramId).Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}