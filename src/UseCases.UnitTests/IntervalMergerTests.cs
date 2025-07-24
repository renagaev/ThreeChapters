using System.Text.Json;
using Domain.Entities;
using UseCases.Services;

namespace UseCases.UnitTests;

public class IntervalMergerTests
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private static readonly List<Book> Books;

    static IntervalMergerTests()
    {
        var rawBooks = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText("books.json"), Options)!;
        Books = Enumerable.Range(0, rawBooks.Count).Select(x => new Book
        {
            Id = x,
            Title = rawBooks[x].Title,
            TitleVariants = rawBooks[x].TitleVariants,
            ChaptersCount = rawBooks[x].ChaptersCount,
        }).ToList();
    }

    public static TheoryData<ReadInterval[], ReadInterval[]> TestCases = new()
    {
        {
            [
                new ReadInterval(1, 1, 1, 1),
                new ReadInterval(1, 2, 1, 2),
            ],
            [new ReadInterval(1, 1, 1, 2)]
        },
        {
            [
                new ReadInterval(1, 1, 1, 1),
                new ReadInterval(1, 2, 1, 2),
                new ReadInterval(1, 3, 1, 3),
                new ReadInterval(1, 5, 1, 5),
            ],
            [
                new ReadInterval(1, 1, 1, 3),
                new ReadInterval(1, 5, 1, 5)
            ]
        },
        {
            [new ReadInterval(1, 1, 1, 2)],
            [new ReadInterval(1, 1, 1, 2)]
        },
        {
            [
                new ReadInterval(1, 1, 1, 1),
                new ReadInterval(1, 3, 1, 3),
            ],
            [
                new ReadInterval(1, 1, 1, 1),
                new ReadInterval(1, 3, 1, 3),
            ]
        },
        {
            [
                // Бытие 48-50, Исход 1-3
                new ReadInterval(0, 48, 0, 50),
                new ReadInterval(1, 1, 1, 3),
            ],
            [
                // Бытие 48 - Исход 3
                new ReadInterval(0, 48, 1, 3),
            ]
        },
        {
            [
                // Исход 1-3, Бытие 48-50 
                new ReadInterval(0, 48, 0, 50),
                new ReadInterval(1, 1, 1, 3),
            ],
            [
                // Бытие 48 - Исход 3
                new ReadInterval(0, 48, 1, 3),
            ]
        },
        {
            [
                // Бытие 48-49, Исход 1-3
                new ReadInterval(0, 48, 0, 49),
                new ReadInterval(1, 1, 1, 3),
            ],
            [
                // Бытие 48-49, Исход 1-3
                new ReadInterval(0, 48, 0, 49),
                new ReadInterval(1, 1, 1, 3),
            ]
        }
    };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Should_Merge_Intervals(ReadInterval[] intervals, ReadInterval[] expected)
    {
        // Arrange
        var merger = new IntervalMerger();

        // Act 
        var result = merger.Merge(intervals, Books);

        // Assert
        Assert.Equal(expected, result);
    }
}