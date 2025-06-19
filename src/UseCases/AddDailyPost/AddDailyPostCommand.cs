using Domain.Entities;
using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using UseCases.Services;
using UseCases.Settings;

namespace UseCases.AddDailyPost;

public record AddDailyPostCommand : IRequest;

public class AddDailyPostCommandHandler(
    ITelegramBotClient botClient,
    IDbContext dbContext,
    ILogger<AddDailyPostCommandHandler> logger,
    DailyMessageRenderer dailyMessageRenderer,
    IOptionsSnapshot<TgSettings> options) : IRequestHandler<AddDailyPostCommand>
{
    public async Task Handle(AddDailyPostCommand request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var postExists = await dbContext.DailyPosts.AnyAsync(x => x.Date == today, cancellationToken);
        if (postExists)
        {
            logger.LogInformation("Skipped post creation: post already exists");
            return;
        }

        var participants = await dbContext.Participants
            .Where(x => x.IsActive)
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var post = dailyMessageRenderer.RenderDailyMessage(today, participants, []);

        var sentPost = await botClient.SendMessage(options.Value.ChannelId, post,
            cancellationToken: cancellationToken);
        dbContext.DailyPosts.Add(new DailyPost
        {
            Date = today,
            MessageId = sentPost.MessageId,
            ChatId = sentPost.Chat.Id
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}