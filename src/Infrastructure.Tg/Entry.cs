using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using UseCases.Settings;

namespace Infrastructure.Tg;

public static class Entry
{
    public static IServiceCollection AddTg(this IServiceCollection services)
    {
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TgSettings>>();
            return new TelegramBotClient(options.Value.BotToken);
        });

        services.AddHostedService<TelegramHostedService>();
        services.AddSingleton<UpdateHandler>();

        return services;
    }
}