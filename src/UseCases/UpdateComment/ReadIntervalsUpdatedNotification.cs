using Domain.Entities;
using MediatR;

namespace UseCases.UpdateSeries;

public record ReadIntervalsUpdatedNotification(DailyPost DailyPost) : INotification;