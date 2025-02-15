using System.Text;
using System.Text.RegularExpressions;
using Domain.Entities;

namespace UseCases.Services;

public interface IIntervalParser
{
    public ICollection<ReadInterval> Parse(string text, ICollection<Book> books);
}

public partial class IntervalParser : IIntervalParser
{
    public ICollection<ReadInterval> Parse(string text, ICollection<Book> books)
    {
        var numberDict = books.ToDictionary(x => x.Id, x => x);
        var result = new List<ReadInterval>();
        text = ReplaceBooksWithIndexes(text, numberDict);

        result.AddRange(FindSingleBookEntries(text));
        result.AddRange(FindSingleChapterEntries(text));
        result.AddRange(FindMultiBookEntries(text));
        result.AddRange(FindCommaSeparatedEntries(text));

        return result
            .Distinct()
            .OrderBy(x => x.StartBook)
            .ThenBy(x => x.StartChapter)
            .ToList();
    }

    [GeneratedRegex(@"book_(?<book>\d+)_[\s\:\-]+(?<chapters>\d+(?:\s*,\s*\d+)+)")]
    private static partial Regex CommaSeparatedChaptersRegex();

    private static IEnumerable<ReadInterval> FindCommaSeparatedEntries(string text) =>
        CommaSeparatedChaptersRegex().Matches(text).SelectMany(x =>
        {
            var book = int.Parse(x.Groups["book"].Value);
            var chapters = x.Groups["chapters"].Value.Split(',').Select(x => int.Parse(x.Trim())).ToList();
            return chapters.Select(chapter => new ReadInterval(book, chapter, book, chapter));
        });

    [GeneratedRegex(@"book_(?<startbook>\d+)_\s+(?<start>\d+)\s*-\s*book_(?<endbook>\d+)_\s*(?<end>\d+)")]
    private static partial Regex MultibooksRegex();

    private static IEnumerable<ReadInterval> FindMultiBookEntries(string text) =>
        MultibooksRegex().Matches(text).Select(x =>
        {
            var startBook = int.Parse(x.Groups["startbook"].Value);
            var startChapter = int.Parse(x.Groups["start"].Value);
            var endBook = int.Parse(x.Groups["endbook"].Value);
            var endChapter = int.Parse(x.Groups["end"].Value);
            return new ReadInterval(startBook, startChapter, endBook, endChapter);
        });


    [GeneratedRegex(@"(?<!-\s*)book_(?<book>\d+)_\s*(?<chapter>\d+)(?!\s*([:-]|[0-9]))")]
    private static partial Regex SingleChapterRegex();

    private static IEnumerable<ReadInterval> FindSingleChapterEntries(string text) =>
        SingleChapterRegex().Matches(text).Select(x =>
        {
            var book = int.Parse(x.Groups["book"].Value);
            var chapter = int.Parse(x.Groups["chapter"].Value);
            return new ReadInterval(book, chapter, book, chapter);
        });


    [GeneratedRegex(@"book_(?<book>\d+)_[\s\:]+(?<start>\d+)\s*-\s*(?<end>\d+)")]
    private static partial Regex SingleBookRegex();

    private static IEnumerable<ReadInterval> FindSingleBookEntries(string text) =>
        SingleBookRegex().Matches(text).Select(x =>
        {
            var book = int.Parse(x.Groups["book"].Value);
            var start = int.Parse(x.Groups["start"].Value);
            var end = int.Parse(x.Groups["end"].Value);
            return new ReadInterval(book, start, book, end);
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
                sb = sb.Replace(variant.ToLower(), $"book_{book.Id}_");
            }
        }

        return sb.ToString();
    }
}