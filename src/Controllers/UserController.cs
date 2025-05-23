using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using UseCases.GetUserAvatar;
using UseCases.Queries.GetUserDailyChaptersRead;
using UseCases.Queries.GetUserDetails;
using UseCases.Queries.GetUserIdByTelegramId;
using UseCases.Queries.GetUserReadChapters;
using UseCases.Queries.GetUsers;

namespace Controllers;

[Route("api/v1/users")]
public class UserController(ISender sender) : ControllerBase
{
    [HttpGet(Name = "getUsers")]
    public async Task<ICollection<UserListItemDto>> GetUsersList(CancellationToken cancellationToken) 
        => await sender.Send(new GetUsersQuery(), cancellationToken);

    [HttpGet("{userId:int}/read-chapters", Name = "getUserReadChaptersByBook")]
    public async Task<ICollection<ReadBookChapters>> GetUsersReadChapters(int userId, CancellationToken cancellationToken) =>
        await sender.Send(new GetUserReadChaptersQuery(userId), cancellationToken);

    [HttpGet("{userId:int}/read-chapters-by-day", Name = "getUserReadChaptersByDay")]
    public async Task<ICollection<DayChaptersReadDto>> GetUsersReadChaptersByDay(int userId,
        CancellationToken cancellationToken) =>
        await sender.Send(new GetUserDailyChaptersReadQuery(userId), cancellationToken);

    [HttpGet("{userId:int}", Name = "getUserDetails")]
    public async Task<UserDetailsDto> GetUserDetails(int userId, CancellationToken cancellationToken) =>
        await sender.Send(new GetUserDetailsQuery(userId), cancellationToken);
    
    [HttpGet("by-telegram-id", Name = "getUserIdByTelegramId")]
    public async Task<long?> GetUserIdByTelegramId(long telegramId, CancellationToken cancellationToken) =>
       await sender.Send(new GetUserIdByTelegramIdQuery(telegramId), cancellationToken);

    [HttpGet("{userId:int}/avatar/{fileName}", Name = "getUserAvatar")]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 60*60*24*7)]
    public async Task<FileStreamResult> GetUserAvatar(int userId, string fileName, CancellationToken cancellationToken)
    {
        var stream = await sender.Send(new GetUserAvatarCommand(userId), cancellationToken);
        new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType);
        return new FileStreamResult(stream, contentType ?? "application/octet-stream");
    }
}