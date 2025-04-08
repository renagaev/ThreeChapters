namespace Infrastructure.Implementation.S3;

public class S3Settings
{
    public required string BaseUrl { get; init; }
    public required string AccessKey { get; init; }
    public required string SecretKey { get; init; }
    public required string BucketName { get; init; }
    public required string Region { get; init; }
}