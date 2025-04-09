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
            .Select(x => new { x.Id, x.Name, x.AvatarPath })
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
        var res = new List<UserListItemDto>();
        foreach (var user in users)
        {
            var avatar = user.AvatarPath != null ? Path.GetFileName(user.AvatarPath) : null;
            res.Add(new UserListItemDto(user.Id, user.Name, avatar));
        }

        return res;
    }
}