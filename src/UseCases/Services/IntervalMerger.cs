using Domain.Entities;

namespace UseCases.Services;

public record ReadInterval(int StartBook, int StartChapter, int EndBook, int EndChapter);

public class IntervalMerger
{
    public ICollection<ReadInterval> Merge(ICollection<ReadInterval> intervals, ICollection<Book> books)
    {
        var booksDict = books.ToDictionary(b => b.Id);
        return intervals
            .OrderBy(x => x.StartBook)
            .Aggregate(new Stack<ReadInterval>(), (acc, interval) =>
        {
            if (acc.Count == 0)
            {
                acc.Push(interval);
                return acc;
            }

            var last = acc.Pop();
            var lastBook = booksDict[last.EndBook];
            if (last.EndBook == interval.StartBook && last.EndChapter + 1 == interval.StartChapter)
            {
                acc.Push(new ReadInterval(last.StartBook, last.StartChapter, interval.EndBook, interval.EndChapter));
                return acc;
            }

            if (last.EndChapter == lastBook.ChaptersCount && interval.StartBook == last.EndBook + 1 &&
                interval.StartChapter == 1)
            {
                acc.Push(new ReadInterval(last.StartBook, last.StartChapter, interval.EndBook, interval.EndChapter));
                return acc;
            }

            acc.Push(last);
            acc.Push(interval);
            return acc;
        }).Reverse().ToList();
    }
}