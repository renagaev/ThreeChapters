namespace Domain.Entities;

public class Book
{
    public int Id { get; init; }
    public string Title { get; init; }
    public int ChaptersCount { get; init; }
    public string[] TitleVariants { get; init; }

    public Testament Testament { get; init; }
    public string GroupTitle { get; init; }
}