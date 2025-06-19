using System.Globalization;
using System.Text;
using Domain.Entities;

namespace UseCases.Services;

public class DailyPostRenderer(IntervalMerger merger)
{
    public string RenderDailyMessage(DateOnly date, ICollection<Participant> participantsWithReadEntries, ICollection<Book> books)
    {
        var users = participantsWithReadEntries.OrderBy(u => u.Id).ToList();
        var culture = CultureInfo.CreateSpecificCulture("ru-RU");
        var todayStr = date.ToString("d MMMM yyyy", culture);

        var post = new StringBuilder(todayStr);
        post.AppendLine();
        post.AppendLine();

        var userLines = new List<string>();
        foreach (var user in users)
        {
            userLines.Add(RenderUserLine(user, books));
        }

        post.Append(string.Join("\n", userLines));
        if (userLines.Any())
        {
            post.AppendLine();
            post.AppendLine();
        }
        post.Append(RenderStats(users));

        return post.ToString();
    }

    private string RenderUserLine(Participant user, ICollection<Book> books)
    {
        if (user.ReadEntries.Count == 0)
        {
            return $"{Constants.UnreadMark} {user.Name}";
        }

        var readIntervals = user.ReadEntries
            .Select(x => new ReadInterval(x.BookId, x.StartChapter, x.BookId, x.EndChapter))
            .ToList();

        var mergedIntervals = merger.Merge(readIntervals, books);
        var renderedIntervals = string.Join(", ", mergedIntervals.Select(i => FormatInterval(i, books)));

        return $"{Constants.ReadMark} {user.Name}: {renderedIntervals}";
    }

    private static string FormatInterval(ReadInterval interval, ICollection<Book> books)
    {
        var bookById = books.ToDictionary(b => b.Id, b => b.Title);
        if (interval.StartBook == interval.EndBook)
        {
            var bookTitle = bookById[interval.StartBook];
            return interval.StartChapter == interval.EndChapter
                ? $"{bookTitle} {interval.StartChapter}"
                : $"{bookTitle} {interval.StartChapter}-{interval.EndChapter}";
        }

        var startBookTitle = bookById[interval.StartBook];
        var endBookTitle = bookById[interval.EndBook];
        return $"{startBookTitle} {interval.StartChapter} - {endBookTitle} {interval.EndChapter}";
    }

    private string RenderStats(ICollection<Participant> participants)
    {
        var readUsers = participants.Count(x => x.ReadEntries.Count != 0);
        var totalUsers = participants.Count;
        var totalChapters = participants
            .SelectMany(x => x.ReadEntries)
            .Sum(x => x.EndChapter - x.StartChapter + 1);

        return FormatStats(readUsers, totalUsers, totalChapters);
    }

    private static string FormatStats(int readUsers, int totalUsers, int totalChapters)
    {
        var lastDigit = totalChapters % 10;
        var lastTwoDigits = totalChapters % 100;
        var (chapters, read) = (lastTwoDigits, lastDigit) switch
        {
            (>= 11 and <= 14, _) => ("глав", "прочитано"),
            (_, 1) => ("глава", "прочитана"),
            (_, 2) or (_, 3) or (_, 4) => ("главы", "прочитано"),
            _ => ("глав", "прочитано")
        };

        return $"{readUsers}/{totalUsers}, {read} {totalChapters} {chapters}";
    }
}
