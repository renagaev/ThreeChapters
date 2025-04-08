namespace Infrastructure.Interfaces.S3;

public interface IS3Service
{
    Task<Stream> GetObjectStream(string s3Path, CancellationToken cancellationToken);
}