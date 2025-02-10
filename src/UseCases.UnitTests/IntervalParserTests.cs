using System.Text.Json;
using Domain.Entities;
using UseCases.Services;

namespace UseCases.UnitTests;

public class IntervalParserTests
{
    private static JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private static List<Book> _books;

    static IntervalParserTests()
    {
        var rawBooks = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText("books.json"), options)!;
        _books = Enumerable.Range(0, rawBooks.Count - 1).Select(x => new Book
        {
            Id = x,
            Title = rawBooks[x].Title,
            TitleVariants = rawBooks[x].TitleVariants,
        }).ToList();
    }

    public static TheoryData<TestCase> TestCases
    {
        get
        {
            var data = new TheoryData<TestCase>();
            var cases = JsonSerializer.Deserialize<List<TestCase>>(File.ReadAllText("parserCases.json"), options)!;
            foreach (var testCase in cases)
            {
                data.Add(testCase);
            }
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Should_Parse_Messages(TestCase testCase)
    {
        // Arrange
        var parser = new IntervalParser();

        // Act
        var result = parser.Parse(testCase.Source, _books);

        // Assert
        Assert.Equal(testCase.Entries.Select(x => new ParsedReadEntry(x.StartBook, x.Start, x.EndBook, x.End)), result);
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
        Assert.Equal(new ParsedReadEntry(startBook, start, endBook, end), entry);
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
        Assert.Equal(new ParsedReadEntry("Деяния", 1, "Деяния", 1), entry);
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
        Assert.Equal(new ParsedReadEntry(book, start, book, end), entry);
    }


    public record TestCaseEntry(string StartBook, int Start, string EndBook, int End);

    public record TestCase(string Source, TestCaseEntry[] Entries);
}