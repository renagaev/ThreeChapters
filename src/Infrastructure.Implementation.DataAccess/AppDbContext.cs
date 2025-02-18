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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookConfiguration).Assembly);
    }
}