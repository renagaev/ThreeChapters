namespace Domain.Entities;

public class SeriesMessage
{
    public int Id { get; init; }
    public DateOnly Date { get; init; }
    public long ChatId { get; init; }
    public int MessageId { get; init; }
}