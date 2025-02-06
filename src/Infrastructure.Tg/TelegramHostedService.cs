using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Infrastructure.Tg;

internal class TelegramHostedService(UpdateHandler updateHandler, ITelegramBotClient client): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions() { DropPendingUpdates = true, AllowedUpdates = [] };
        await client.ReceiveAsync(updateHandler, receiverOptions, stoppingToken);
    }
}