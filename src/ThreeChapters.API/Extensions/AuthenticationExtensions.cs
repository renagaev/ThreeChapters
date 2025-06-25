using ThreeChapters.API.Auth;
using UseCases.Settings;

namespace ThreeChapters.API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddTelegramAuth(this IServiceCollection services)
    {
        services.AddOptions<TgAuthOptions>()
            .BindConfiguration(nameof(TgSettings));

        services.AddAuthentication("TWA")
            .AddScheme<TgAuthOptions, TgAuthHandler>("TWA", null);

        return services;
    }
}