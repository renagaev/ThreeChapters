using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Implementation.DataAccess.Configurations;

public class ReadEntryConfiguration : IEntityTypeConfiguration<ReadEntry>
{
    public void Configure(EntityTypeBuilder<ReadEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Date).IsRequired();
        builder.HasOne(x => x.Book);
        builder.HasOne(x => x.Participant);
    }
}