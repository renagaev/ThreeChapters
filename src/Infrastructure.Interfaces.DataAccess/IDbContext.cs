﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Interfaces.DataAccess;

public interface IDbContext
{
    public DbSet<Book> Books { get; }
    public DbSet<Participant> Participants { get; }
    public DbSet<ReadEntry> ReadEntries { get; }
    public DbSet<SeriesMessage> SeriesMessages { get; }
    public DbSet<DailyPost> DailyPosts { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}