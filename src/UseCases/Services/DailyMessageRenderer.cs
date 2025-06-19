using System.Globalization;
using System.Text;
using Domain.Entities;

namespace UseCases.Services;

public class DailyMessageRenderer(IntervalMerger merger)
{
    public string RenderDailyMessage(DateOnly date, ICollection<Participant> participantsWithReadEntries, ICollection<Book> books)
    {
        var users = participantsWithReadEntries.OrderBy(x => x.Id).ToList();
        var culture = CultureInfo.CreateSpecificCulture("ru-RU");
        var todayStr = date.ToString("dd MMMM yyyy", culture);

        var post = new StringBuilder(todayStr);
        post.Append("\n\n");

        var userStrings = new List<string>();
        foreach (var user in users)
        {
            if (user.ReadEntries.Count == 0)
            {
                userStrings.Add($"{Constants.UnreadMark} {user.Name}");
            }
            else
            {
                var readIntervals = user.ReadEntries
                    .Select(x => new ReadInterval(x.BookId, x.StartChapter, x.BookId, x.EndChapter))
                    .ToList();
                var merged = merger.Merge(readIntervals, books);
                var bookById = books.ToDictionary(x => x.Id, x => x.Title);
                var renderedIntervals = string.Join(", ", merged.Select(x => x switch
                {
                    _ when x.StartBook == x.EndBook && x.StartChapter == x.EndChapter =>
                        $"{bookById[x.StartBook]} {x.StartChapter}",
                    _ when x.StartBook == x.EndBook => $"{bookById[x.StartBook]} {x.StartChapter}-{x.EndChapter}",
                    _ => $"{bookById[x.StartBook]} {x.StartChapter} - {bookById[x.EndBook]} {x.EndChapter}"
                }));

                userStrings.Add($"{Constants.ReadMark} {user.Name}: {renderedIntervals}");
            }
        }

        post.Append(string.Join("\n", userStrings));
        if (userStrings.Count != 0)
        {
            post.Append("\n\n");
        }

        post.Append(RenderStats(users));

        return post.ToString();
    }

    private string RenderStats(ICollection<Participant> participants)
    {
        var readUsers = participants.Count(x => x.ReadEntries.Count != 0);
        var totalUsers = participants.Count;
        var totalChapters = participants.SelectMany(x => x.ReadEntries).Sum(x => x.EndChapter - x.StartChapter + 1);

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