using MediatR;
using Microsoft.AspNetCore.Mvc;
using UseCases.Queries.GetUserDailyChaptersRead;
using UseCases.Queries.GetUserDetails;
using UseCases.Queries.GetUserReadChapters;
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

    [HttpGet("{userId:int}/read-chapters-by-book", Name = "getUserReadChaptersByBook")]
    public async Task<ICollection<ReadBookChapters>> GetUsersReadChapters(int userId, CancellationToken cancellationToken) =>
        await sender.Send(new GetUserReadChaptersQuery(userId), cancellationToken);

    [HttpGet("{userId:int}/read-chapters-by-day", Name = "getUserReadChaptersByDay")]
    public async Task<ICollection<DayChaptersReadDto>> GetUsersReadChaptersByDay(int userId,
        CancellationToken cancellationToken) =>
        await sender.Send(new GetUserDailyChaptersReadQuery(userId), cancellationToken);

    [HttpGet("{userId:int}", Name = "getUserDetails")]
    public async Task<UserDetailsDto> GetUserDetails(int userId, CancellationToken cancellationToken) =>
        await sender.Send(new GetUserDetailsQuery(userId), cancellationToken);
}