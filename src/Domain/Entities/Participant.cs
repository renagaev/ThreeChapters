namespace Domain.Entities;

public class Participant
{
    public long Id { get; init; }
    public string Name { get; init; }
    public DateOnly MemberFrom { get; init; }
    public long? TelegramId { get; init; }
    public bool IsActive { get; set; } = true;
    public ICollection<ReadEntry> ReadEntries { get; set; } = [];
    public string? AvatarPath { get; set; }
    public bool IsAdmin { get; set; }
}