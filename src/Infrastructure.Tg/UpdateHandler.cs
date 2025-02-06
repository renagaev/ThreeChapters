using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UseCases.ProcessComment;

namespace Infrastructure.Tg;

internal class UpdateHandler(IServiceScopeFactory scopeFactory, IOptions<TgSettings> options) : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        await (update.Type switch
        {
            UpdateType.Message => HandleMessageUpdate(sender, update.Message!, cancellationToken),
            UpdateType.EditedMessage => HandleMessageUpdate(sender, update.EditedMessage!, cancellationToken),
            _ => Task.CompletedTask
        });
    }

    private async Task HandleMessageUpdate(ISender sender, Message message,CancellationToken cancellationToken)
    {
        if (message.ReplyToMessage?.ForwardOrigin is MessageOriginChannel channel && channel.Chat.Id == options.Value.ChannelId)
        {
            await sender.Send(new ProcessCommentCommand(message), cancellationToken);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}