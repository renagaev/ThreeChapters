using Infrastructure.Interfaces.DataAccess;
using Infrastructure.Interfaces.S3;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UseCases.GetUserAvatar;

public record GetUserAvatarCommand(long UserId) : IRequest<Stream>;

public class GetUserAvatarCommandHandler(IS3Service s3Service, IDbContext dbContext)
    : IRequestHandler<GetUserAvatarCommand, Stream>
{
    public async Task<Stream> Handle(GetUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var s3Path = await dbContext.Participants
            .Where(x => x.Id == request.UserId)
            .Select(x => x.AvatarPath)
            .FirstAsync(cancellationToken);

        return await s3Service.GetObjectStream(s3Path, cancellationToken);
    }
}