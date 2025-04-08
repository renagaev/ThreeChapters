using Infrastructure.Interfaces.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace Infrastructure.Implementation.S3;

public static class Entry
{
    public static IServiceCollection AddS3(this IServiceCollection services)
    {
        services.AddSingleton<IMinioClient>(s =>
        {
            var options = s.GetRequiredService<IOptions<S3Settings>>().Value;
            var factory = new MinioClientFactory(client => client
                .WithRegion(options.Region)
                .WithEndpoint(options.BaseUrl)
                .WithSSL()
                .WithCredentials(options.AccessKey, options.SecretKey));
            return factory.CreateClient();
        });
        services.AddScoped<IS3Service, S3Service>();
        return services;
    }
}