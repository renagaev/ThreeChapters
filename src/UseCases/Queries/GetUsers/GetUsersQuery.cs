using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UseCases.Queries.GetUsers;

public record GetUsersQuery : IRequest<ICollection<UserListItemDto>>;

public class GetUsersQueryHandler(IDbContext dbContext) : IRequestHandler<GetUsersQuery, ICollection<UserListItemDto>>
{
    public async Task<ICollection<UserListItemDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await dbContext.Participants
            .Select(x => new { x.Id, x.Name })
            .OrderBy(x=> x.Id)
            .ToListAsync(cancellationToken);

        return users.Select(x => new UserListItemDto(x.Id, x.Name)).ToList();
    }
}