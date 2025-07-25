using Domain.Entities;
using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using UseCases.Services;

namespace UseCases.RegisterUser;

public record RegisterUserCommand(string Name, long TelegramId) : IRequest<long>;

public class RegisterUserCommandHandler(
    IDbContext dbContext,
    DailyPostRenderer postRenderer,
    ITelegramBotClient botClient) : IRequestHandler<RegisterUserCommand, long>
{
    public async Task<long> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Participants
            .FirstOrDefaultAsync(x => x.TelegramId == request.TelegramId, cancellationToken);

        if (existing != null)
        {
            return existing.Id;
        }

        var today = DateOnly.FromDateTime(DateTime.Now);

        var participant = new Participant
        {
            TelegramId = request.TelegramId,
            Name = request.Name,
            MemberFrom = today
        };
        dbContext.Participants.Add(participant);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dailyPost = await dbContext.DailyPosts.FirstOrDefaultAsync(x => x.Date == today, cancellationToken);
        if (dailyPost != null)
        {
            var participants = await dbContext.Participants
                .Where(x => x.IsActive)
                .Include(x => x.ReadEntries.Where(x => x.Date == today))
                .ToListAsync(cancellationToken);

            var books = await dbContext.Books.ToListAsync(cancellationToken);
            
            await botClient.EditMessageText(
                chatId: dailyPost.ChatId,
                messageId: dailyPost.MessageId,
                text: postRenderer.RenderDailyMessage(today, participants, books),
                cancellationToken: cancellationToken);
            
        }

        return participant.Id;
    }
}