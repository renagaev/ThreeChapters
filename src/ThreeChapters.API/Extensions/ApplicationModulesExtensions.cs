using Infrastructure.Implementation.DataAccess;
using Infrastructure.Tg;
using UseCases;

namespace ThreeChapters.API.Extensions;

public static class ApplicationModulesExtensions
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataAccess(configuration);

        services.AddOptions<TgSettings>().BindConfiguration(nameof(TgSettings));
        services.AddTg();

        services.AddUseCases();
        
        return services;
    }
}