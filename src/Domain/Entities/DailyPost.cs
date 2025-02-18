namespace Domain.Entities;

public class DailyPost
{
    public int Id { get; init; }
    public DateOnly Date { get; init; }
    public long ChatId { get; init; }
    public int MessageId { get; init; }
}