namespace Domain.Entities;

public class Participant
{
    public long Id { get; init; }
    public string Name { get; init; }
    public bool IsActive { get; set; } = true;
    public ICollection<ReadEntry> ReadEntries { get; set; } = [];
}