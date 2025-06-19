using Domain.Entities;
using UseCases.Services;

namespace UseCases.UnitTests;

public class DailyPostRendererTests
{
    private readonly IntervalMerger _intervalMerger = new();

    [Fact]
    public void Should_RenderSingleUserWithSingleChapter()
    {
        var book = CreateBook(0, "Книга 1");
        var user = CreateParticipant("Иван", [CreateReadEntry(1, book.Id, 3, 3)]);

        var renderer = new DailyPostRenderer(_intervalMerger);
        var result = renderer.RenderDailyMessage(new DateOnly(2025, 6, 19), [user], [book]);

        var expected = """
                       19 июня 2025

                       ✅ Иван: Книга 1 3

                       1/1, прочитана 1 глава
                       """.Trim();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_RenderSingleUserWithChapterRange()
    {
        var book = CreateBook(0, "Книга 1");
        var user = CreateParticipant("Иван", [CreateReadEntry(1, book.Id, 3, 5)]);

        var renderer = new DailyPostRenderer(_intervalMerger);
        var result = renderer.RenderDailyMessage(new DateOnly(2025, 6, 19), [user], [book]);

        var expected = """
                       19 июня 2025

                       ✅ Иван: Книга 1 3-5

                       1/1, прочитано 3 главы
                       """.Trim();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_RenderUserWithCrossBookInterval()
    {
        var book1 = CreateBook(0, "Бытие");
        var book2 = CreateBook(1, "Исход");

        var user = CreateParticipant("Иван", [
            CreateReadEntry(1, book1.Id, 48, 50),
            CreateReadEntry(2, book2.Id, 1, 3)
        ]);

        var renderer = new DailyPostRenderer(_intervalMerger);
        var result = renderer.RenderDailyMessage(new DateOnly(2025, 6, 19), [user], [book1, book2]);

        var expected = """
                       19 июня 2025

                       ✅ Иван: Бытие 48 - Исход 3

                       1/1, прочитано 6 глав
                       """.Trim();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_RenderMultipleUsersWithMixedStatus()
    {
        var book = CreateBook(0, "Книга 1");

        var user1 = CreateParticipant("Иван", [CreateReadEntry(1, book.Id, 1, 2)]);
        var user2 = CreateParticipant("Мария", []);
        var user3 = CreateParticipant("Петр", [CreateReadEntry(2, book.Id, 3, 3)]);

        var renderer = new DailyPostRenderer(_intervalMerger);
        var result = renderer.RenderDailyMessage(new DateOnly(2025, 6, 19), [user1, user2, user3], [book]);

        var expected = """
                       19 июня 2025

                       ✅ Иван: Книга 1 1-2
                       ❔ Мария
                       ✅ Петр: Книга 1 3

                       2/3, прочитано 3 главы
                       """.Trim();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_RenderEmptyParticipantList()
    {
        var renderer = new DailyPostRenderer(_intervalMerger);
        var result = renderer.RenderDailyMessage(new DateOnly(2025, 6, 19), [], []);

        var expected = """
                       19 июня 2025

                       0/0, прочитано 0 глав
                       """.Trim();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_RenderMultipleIntervals()
    {
        var book1 = CreateBook(0, "Бытие");
        var book2 = CreateBook(1, "Исход");

        var user = CreateParticipant("Иван", [
            CreateReadEntry(1, book1.Id, 48, 49),
            CreateReadEntry(2, book2.Id, 1, 3)
        ]);

        var renderer = new DailyPostRenderer(_intervalMerger);
        var result = renderer.RenderDailyMessage(new DateOnly(2025, 6, 19), [user], [book1, book2]);

        var expected = """
                       19 июня 2025

                       ✅ Иван: Бытие 48-49, Исход 1-3

                       1/1, прочитано 5 глав
                       """.Trim();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_RenderCorrectPluralizationForChapters()
    {
        var book = CreateBook(0, "Книга 1");

        var user1 = CreateParticipant("Иван", [CreateReadEntry(1, book.Id, 1, 1)]); // 1 глава
        var user2 = CreateParticipant("Мария", [CreateReadEntry(2, book.Id, 2, 3)]); // 2 главы
        var user3 = CreateParticipant("Петр", [CreateReadEntry(3, book.Id, 4, 6)]); // 3 главы

        var renderer = new DailyPostRenderer(_intervalMerger);
        var result = renderer.RenderDailyMessage(new DateOnly(2025, 6, 19), [user1, user2, user3], [book]);

        var expected = """
                       19 июня 2025

                       ✅ Иван: Книга 1 1
                       ✅ Мария: Книга 1 2-3
                       ✅ Петр: Книга 1 4-6

                       3/3, прочитано 6 глав
                       """.Trim();

        Assert.Equal(expected, result);
    }

    private static Participant CreateParticipant(string name, ICollection<ReadEntry> entries)
    {
        return new Participant
        {
            Id = 1,
            Name = name,
            MemberFrom = new DateOnly(2025, 1, 1),
            TelegramId = null,
            ReadEntries = entries,
            IsActive = true
        };
    }

    private static Book CreateBook(int id, string title)
    {
        return new Book
        {
            Id = id,
            Title = title,
            ChaptersCount = 50,
            TitleVariants = [title],
            Testament = Testament.Old,
            GroupTitle = "Группа 1"
        };
    }

    private static ReadEntry CreateReadEntry(int id, int bookId, int startChapter, int endChapter)
    {
        return new ReadEntry
        {
            Id = id,
            ParticipantId = 1,
            Date = new DateOnly(2025, 6, 19),
            BookId = bookId,
            StartChapter = startChapter,
            EndChapter = endChapter
        };
    }
}