using MediatR;
using Microsoft.AspNetCore.Mvc;
using UseCases.Queries.GetUsers;

namespace Controllers;

[Route("api/v1/users")]
public class UserController(ISender sender) : ControllerBase
{
    [HttpGet(Name = "getUsers")]
    public async Task<ICollection<UserListItemDto>> GetUsersList(CancellationToken cancellationToken)
    {
        return await sender.Send(new GetUsersQuery(), cancellationToken);
    }
}