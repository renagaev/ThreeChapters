using Domain;
using Domain.Entities;
using Infrastructure.Implementation.DataAccess.Configurations;
using Infrastructure.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementation.DataAccess;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IDbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<ReadEntry> ReadEntries { get; set; }
    public DbSet<SeriesMessage> SeriesMessages { get; set; }
    public DbSet<DailyPost> DailyPosts { get; set; }
    public async Task<ICollection<DateRange>> GetUserStreaks(int userId,
        CancellationToken cancellationToken)
    {
        var res =  await Database.SqlQuery<DateRange>(
                $"""
                 WITH user_dates AS (SELECT DISTINCT date
                               FROM read_entries
                               WHERE participant_id = {userId}
                                 AND date <= now()::date),
                 numbered AS (SELECT date,
                                    ROW_NUMBER() OVER (ORDER BY date) AS rn
                             FROM user_dates),
                 grouped AS (SELECT date,
                                   date - rn * INTERVAL '1 day' AS grp
                            FROM numbered)
                 SELECT MIN(date) AS from,
                  MAX(date) AS to
                 FROM grouped
                 GROUP BY grp
                 """)
            .ToListAsync(cancellationToken: cancellationToken);

        return res;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookConfiguration).Assembly);
    }
}