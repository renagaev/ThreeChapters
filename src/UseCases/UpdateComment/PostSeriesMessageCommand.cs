using MediatR;
using Telegram.Bot.Types;

namespace UseCases.UpdateSeries;

public record PostSeriesMessageCommand(Message Message): IRequest;