using System.Text.Json;
using Domain.Entities;
using UseCases.Services;

namespace UseCases.UnitTests;

public class IntervalParserTests
{
    private static JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private static List<Book> _books;
    private static Dictionary<string, int> _bookDict;

    static IntervalParserTests()
    {
        var rawBooks = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText("books.json"), options)!;
        _books = Enumerable.Range(0, rawBooks.Count - 1).Select(x => new Book
        {
            Id = x,
            Title = rawBooks[x].Title,
            TitleVariants = rawBooks[x].TitleVariants,
        }).ToList();
        _bookDict = _books.ToDictionary(x => x.Title, x => x.Id);
    }

    public static TheoryData<string, ICollection<ReadInterval>> TestCases
    {
        get
        {
            var data = new TheoryData<string, ICollection<ReadInterval>>();
            var cases = JsonSerializer.Deserialize<List<TestCase>>(File.ReadAllText("parserCases.json"), options)!;
            foreach (var testCase in cases)
            {
                var expected = testCase.Entries
                    .Select(x => new ReadInterval(_bookDict[x.StartBook], x.Start, _bookDict[x.EndBook], x.End))
                    .ToList();
                data.Add(testCase.Source, expected);
            }
            return data;
        }
    }
    
    [Theory]
    [MemberData(nameof(TestCases))]
    public void Should_Parse_Messages(string source, ICollection<ReadInterval> expected)
    {
        // Arrange
        var parser = new IntervalParser();

        // Act
        var result = parser.Parse(source, _books);

        // Assert
        Assert.Equal(expected, result);
    }

    public static TheoryData<string, string, int, string, int> MultibookTestCases => new()
    {
        { "Деяния 1 - 1 Иоанна 2", "Деяния", 1, "1-Иоанна", 2 },
        { "Иисус Навин 12- Книга Судей 5", "Иисус Навин", 12, "Судей", 5 },
        { "Луки 22- Иоанна2", "Луки", 22, "Иоанна", 2 }
    };

    [Theory]
    [MemberData(nameof(MultibookTestCases))]
    public void Should_Parse_MultibookEntry(string text, string startBook, int start, string endBook, int end)
    {
        // Arrange
        var parser = new IntervalParser();

        // Act
        var result = parser.Parse(text, _books);

        // Assert
        var entry = result.Single();
        Assert.Equal(new ReadInterval(_bookDict[startBook], start, _bookDict[endBook], end), entry);
    }

    [Fact]
    public void Should_Parse_SingleChapterEntry()
    {
        // Arrange
        var parser = new IntervalParser();

        // Act
        var result = parser.Parse("Деяния 1", _books);

        // Assert
        var entry = result.Single();
        Assert.Equal(new ReadInterval(_bookDict["Деяния"], 1, _bookDict["Деяния"], 1), entry);
    }

    [Theory]
    [InlineData("Деяния 1:1")]
    [InlineData("Деяния 1:1-2")]
    [InlineData("Деяния 1 :1")]
    public void Should_Not_Parse_Verse_Entry(string text)
    {
        // Arrange
        var parser = new IntervalParser();

        // Act
        var result = parser.Parse(text, _books);

        // Assert
        Assert.Empty(result);
    }


    public static TheoryData<string, string, int, int> SingleBookCases => new()
    {
        { "1 Царств 1-15", "1-Царств", 1, 15 },
        { "Бытие 22-25", "Бытие", 22, 25 },
        { "1 Паралипоменон 1-5", "1-Паралипоменон", 1, 5 },
        { "Деяния:18-20", "Деяния", 18, 20 }
    };

    [Theory]
    [MemberData(nameof(SingleBookCases))]
    public void Should_Parse_SingleBookEntry(string text, string book, int start, int end)
    {
        // Arrange
        var parser = new IntervalParser();

        // Act
        var result = parser.Parse(text, _books);

        // Assert
        var entry = result.Single();
        Assert.Equal(new ReadInterval(_bookDict[book], start, _bookDict[book], end), entry);
    }


    public record TestCaseEntry(string StartBook, int Start, string EndBook, int End);

    public record TestCase(string Source, TestCaseEntry[] Entries);
}