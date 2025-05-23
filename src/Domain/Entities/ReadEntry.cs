namespace Domain.Entities;

public class ReadEntry
{
    public long Id { get; init; }
    
    public Participant Participant { get; init; }
    public long ParticipantId { get; init; }
    public DateOnly Date { get; init; }
    public Book Book { get; init; }
    public int BookId { get; init; }
    
    public int StartChapter { get; init; }
    public int EndChapter { get; init; }
}