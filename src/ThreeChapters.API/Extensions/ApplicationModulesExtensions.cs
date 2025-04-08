using Infrastructure.Implementation.BackgroundJobs;
using Infrastructure.Implementation.DataAccess;
using Infrastructure.Implementation.S3;
using Infrastructure.Tg;
using UseCases.Settings;
using Entry = UseCases.Entry;

namespace ThreeChapters.API.Extensions;

public static class ApplicationModulesExtensions
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataAccess(configuration);
        services.AddBackgroundJobs(configuration);

        services.AddOptions<TgSettings>().BindConfiguration(nameof(TgSettings));
        services.AddTg();

        services.AddOptions<S3Settings>().BindConfiguration(nameof(S3Settings));
        services.AddS3();

        Entry.AddUseCases(services);
        
        return services;
    }
}