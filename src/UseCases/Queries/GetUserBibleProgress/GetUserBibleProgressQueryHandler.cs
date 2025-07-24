using Infrastructure.Interfaces.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace UseCases.Queries.GetUserBibleProgress;

public class GetUserBibleProgressQueryHandler(IDbContext dbContext)
    : IRequestHandler<GetUserBibleProgressQuery, BibleProgressStats>
{
    public async Task<BibleProgressStats> Handle(GetUserBibleProgressQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Получаем количество глав в каждой книге: bookId -> totalChapters
        var bookChapterCounts = await dbContext.Books
            .AsNoTracking()
            .Select(b => new { b.Id, b.ChaptersCount })
            .ToDictionaryAsync(b => b.Id, b => b.ChaptersCount, cancellationToken);

        // 2. Инициализируем счётчики прочтений для каждой главы каждой книги
        var counters = new Dictionary<int, int[]>(bookChapterCounts.Count);
        foreach (var (bookId, totalChapters) in bookChapterCounts)
            counters[bookId] = new int[totalChapters];
        var chapterReadCounters = counters;

        // 3. Применяем записи пользователя к счётчикам
        await ApplyUserReadEntriesAsync(request.UserId, chapterReadCounters, cancellationToken);

        // 4. Определяем количество полностью прочитанных циклов (полных обходов книги)
        var completedFullReads = chapterReadCounters.Values.Min(counts => counts.Min());

        if (completedFullReads > 0)
            SubtractCompletedCycles(chapterReadCounters, completedFullReads);

        // 5. Вычисляем прогресс текущего незавершённого цикла
        var currentCycleProgress = CalculateCurrentCycleProgress(chapterReadCounters);

        // 6. Формируем DTO: для каждой книги список глав, прочитанных в текущем цикле
        var readBookChapters = chapterReadCounters
            .Select(kvp => new ReadBookChapters(
                kvp.Key,
                kvp.Value
                    .Select((readCount, chapterIndex) => (readCount, chapterNumber: chapterIndex + 1))
                    .Where(t => t.readCount > 0)
                    .Select(t => t.chapterNumber)
                    .ToList()))
            .ToList();

        return new BibleProgressStats(completedFullReads, currentCycleProgress, readBookChapters);
    }

    private async Task ApplyUserReadEntriesAsync(int userId,
        Dictionary<int, int[]> chapterReadCounters,
        CancellationToken cancellationToken)
    {
        var readEntries = await dbContext.ReadEntries
            .AsNoTracking()
            .Where(entry => entry.Participant.Id == userId)
            .Select(entry => new { entry.BookId, entry.StartChapter, entry.EndChapter })
            .ToListAsync(cancellationToken);

        foreach (var entry in readEntries)
        {
            var chapterCounts = chapterReadCounters[entry.BookId];
            for (int chapter = entry.StartChapter; chapter <= entry.EndChapter; chapter++)
            {
                if (chapterCounts.Length >= chapter)
                {
                    chapterCounts[chapter - 1]++;
                }
            }
        }
    }

    private static void SubtractCompletedCycles(Dictionary<int, int[]> chapterReadCounters, int completedCycles)
    {
        foreach (var chapterCounts in chapterReadCounters.Values)
            for (int i = 0; i < chapterCounts.Length; i++)
                chapterCounts[i] -= completedCycles;
    }

    private static float CalculateCurrentCycleProgress(Dictionary<int, int[]> chapterReadCounters)
    {
        int readCount = 0, totalCount = 0;

        foreach (var chapterCounts in chapterReadCounters.Values)
        {
            totalCount += chapterCounts.Length;
            readCount += chapterCounts.Count(count => count > 0);
        }

        return totalCount == 0 ? 0 : MathF.Round(readCount / (float)totalCount, 4);
    }
}