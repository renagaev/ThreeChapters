using Domain.Entities;
using MediatR;
using Telegram.Bot.Types;

namespace UseCases.Notifications;

public record ReadIntervalsUpdatedNotification(DailyPost DailyPost) : INotification;