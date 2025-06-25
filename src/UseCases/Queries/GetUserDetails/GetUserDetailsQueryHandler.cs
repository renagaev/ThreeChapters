using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UseCases.Queries.GetUserDetails;

public class GetUserDetailsQueryHandler(IDbContext dbContext) : IRequestHandler<GetUserDetailsQuery, UserDetailsDto>
{
    public async Task<UserDetailsDto> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Participants
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var avatar = user.AvatarPath != null ? Path.GetFileName(user.AvatarPath) : null;

        return new UserDetailsDto(
            user.Id, 
            user.Name,
            user.MemberFrom,
            user.IsActive,
            avatar);
    }
}