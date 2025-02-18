using Infrastructure.Implementation.BackgroundJobs;
using Infrastructure.Implementation.DataAccess;
using Infrastructure.Tg;
using UseCases;
using UseCases.Settings;

namespace ThreeChapters.API.Extensions;

public static class ApplicationModulesExtensions
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataAccess(configuration);
        services.AddBackgroundJobs(configuration);

        services.AddOptions<TgSettings>().BindConfiguration(nameof(TgSettings));
        services.AddTg();

        services.AddUseCases();
        
        return services;
    }
}