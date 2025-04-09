using Infrastructure.Interfaces.S3;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure.Implementation.S3;

public class S3Service(IMinioClient minioClient, IOptionsSnapshot<S3Settings> options) : IS3Service
{
    public async Task<Stream> GetObjectStream(string s3Path, CancellationToken cancellationToken)
    {
        var ms = new MemoryStream();
        await minioClient.GetObjectAsync(new GetObjectArgs().WithBucket(options.Value.BucketName).WithObject(s3Path)
                .WithCallbackStream((stream, cancellationToken) => stream.CopyToAsync(ms, cancellationToken)),
            cancellationToken);
        ms.Position = 0;
        return ms;
    }
}