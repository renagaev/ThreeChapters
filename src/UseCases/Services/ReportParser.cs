using System.Text.RegularExpressions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace UseCases.Services;

public record ReportItem(string User, ICollection<ReadInterval> Intervals);

public record Report(DateOnly Date, ICollection<ReportItem> Items);

public partial class ReportParser(IIntervalParser intervalParser, ILogger<ReportParser> logger)
{
    public Report? Parse(string reportText, ICollection<Book> books)
    {
        var date = ParseDate(reportText);
        if (date == null)
        {
            logger.LogInformation("failed to parse date");
            return null;
        }

        var read = ParseRead(reportText, books);
        var unread = ParseUnread(reportText);
        return new Report(date.Value, read.Concat(unread).ToList());
    }

    private ICollection<ReportItem> ParseUnread(string text) =>
        Regex.Matches(text, @$"{Constants.UnreadMark}\s*(\w.*\w)")
            .Select(x => new ReportItem(x.Groups[1].Value, []))
            .ToList();

    private ICollection<ReportItem> ParseRead(string text, ICollection<Book> books) =>
        Regex.Matches(text, @$"{Constants.ReadMark}\s*(\w.*\w): (.*\d)").Select(match =>
        {
            var name = match.Groups[1].Value;
            var intervals = intervalParser.Parse(match.Groups[2].Value, books);
            if (intervals.Count == 0)
            {
                throw new Exception($"failed to parse intervals for user {name}");
            }
            return new ReportItem(name, intervals);
        }).ToList();

    private DateOnly? ParseDate(string text)
    {
        var dateMatch = DateRegex().Match(text);
        if (!dateMatch.Success)
            return null;

        var day = int.Parse(dateMatch.Groups[1].Value);
        if (!Constants.Months.TryGetValue(dateMatch.Groups[2].Value, out var month))
        {
            return null;
        }

        var year = int.Parse(dateMatch.Groups[3].Value);

        return new DateOnly(year, month, day);
    }

    [GeneratedRegex(@"(\d+) (\w+) (\d\d\d\d)")]
    private static partial Regex DateRegex();
}