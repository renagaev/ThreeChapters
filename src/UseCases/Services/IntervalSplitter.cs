using Domain.Entities;

namespace UseCases.Services;

public record SingleBookInterval(int Book, int StartChapter, int EndChapter);

public class IntervalSplitter
{
    public ICollection<SingleBookInterval> Split(ReadInterval interval, ICollection<Book> books)
    {
        if (interval.StartBook == interval.EndBook)
        {
            return new List<SingleBookInterval>()
            {
                new(interval.StartBook, interval.StartChapter, interval.EndChapter)
            };
        }

        var startBookChapters = books.First(x => x.Id == interval.StartBook).ChaptersCount;

        return books
            .Where(x => x.Id > interval.StartBook && x.Id < interval.EndBook)
            .Select(x => new SingleBookInterval(x.Id, 1, x.ChaptersCount))
            .Append(new SingleBookInterval(interval.StartBook, interval.StartChapter, startBookChapters))
            .Append(new SingleBookInterval(interval.EndBook, 1, interval.EndChapter))
            .OrderBy(x => x.Book)
            .ToList();
    }
}