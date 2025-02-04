using System.Text;
using System.Text.RegularExpressions;
using Domain.Entities;

namespace UseCases.Services;

public record ParsedReadEntry(string StartBook, int StartChapter, string EndBook, int EndChapter);

public partial class ReadParser
{
    public List<ParsedReadEntry> Parse(string text, List<Book> books)
    {
        var numberDict = books.ToDictionary(x => x.Order, x => x);
        var result = new List<ParsedReadEntry>();
        text = ReplaceBooksWithIndexes(text, numberDict);

        result.AddRange(FindSingleBookEntries(text, numberDict));
        result.AddRange(FindSingleChapterEntries(text, numberDict));
        result.AddRange(FindMultiBookEntries(text, numberDict));
        result.AddRange(FindCommaSeparatedEntries(text, numberDict));

        return result
            .Distinct()
            .OrderBy(x => books.First(book => book.Title == x.StartBook).Order)
            .ThenBy(x => x.StartChapter)
            .ToList();
    }

    [GeneratedRegex(@"book_(?<book>\d+)_[\s\:\-]+(?<chapters>\d+(?:\s*,\s*\d+)+)")]
    private static partial Regex CommaSeparatedChaptersRegex();
    private static IEnumerable<ParsedReadEntry> FindCommaSeparatedEntries(string text, Dictionary<int, Book> books) =>
        CommaSeparatedChaptersRegex().Matches(text).SelectMany(x =>
        {
            var book = books[int.Parse(x.Groups["book"].Value)];
            var chapters = x.Groups["chapters"].Value.Split(',').Select(x => int.Parse(x.Trim())).ToList();
            return chapters.Select(chapter => new ParsedReadEntry(book.Title, chapter, book.Title, chapter));
        });

    [GeneratedRegex(@"book_(?<startbook>\d+)_\s+(?<start>\d+)\s*-\s*book_(?<endbook>\d+)_\s*(?<end>\d+)")]
    private static partial Regex MultibooksRegex();
    private static IEnumerable<ParsedReadEntry> FindMultiBookEntries(string text, Dictionary<int, Book> books) =>
        MultibooksRegex().Matches(text).Select(x =>
        {
            var startBook = int.Parse(x.Groups["startbook"].Value);
            var startChapter = int.Parse(x.Groups["start"].Value);
            var endBook = int.Parse(x.Groups["endbook"].Value);
            var endChapter = int.Parse(x.Groups["end"].Value);
            return new ParsedReadEntry(books[startBook].Title, startChapter, books[endBook].Title, endChapter);
        });


    [GeneratedRegex(@"(?<!-\s*)book_(?<book>\d+)_\s*(?<chapter>\d+)(?!\s*([:-]|[0-9]))")]
    private static partial Regex SingleChapterRegex();
    private static IEnumerable<ParsedReadEntry> FindSingleChapterEntries(string text, Dictionary<int, Book> books) =>
        SingleChapterRegex().Matches(text).Select(x =>
        {
            var book = books[int.Parse(x.Groups["book"].Value)];
            var chapter = int.Parse(x.Groups["chapter"].Value);
            return new ParsedReadEntry(book.Title, chapter, book.Title, chapter);
        });

    
    [GeneratedRegex(@"book_(?<book>\d+)_[\s\:]+(?<start>\d+)\s*-\s*(?<end>\d+)")]
    private static partial Regex SingleBookRegex();
    private static IEnumerable<ParsedReadEntry> FindSingleBookEntries(string text, Dictionary<int, Book> books) =>
        SingleBookRegex().Matches(text).Select(x =>
        {
            var book = books[int.Parse(x.Groups["book"].Value)];
            var start = int.Parse(x.Groups["start"].Value);
            var end = int.Parse(x.Groups["end"].Value);
            return new ParsedReadEntry(book.Title, start, book.Title, end);
        });

    private static string ReplaceBooksWithIndexes(string text, Dictionary<int, Book> books)
    {
        var sb = new StringBuilder(text.ToLower());
        foreach (var (_, book) in books.OrderByDescending(x => x.Value.Title.Length))
        {
            var variants = book.TitleVariants
                .Append(book.Title)
                .OrderByDescending(x => x.Length)
                .ToList();
            foreach (var variant in variants)
            {
                sb = sb.Replace(variant.ToLower(), $"book_{book.Order}_");
            }
        }

        return sb.ToString();
    }
}