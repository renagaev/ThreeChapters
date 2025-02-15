using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Implementation.DataAccess.Configurations;

public class SeriesMessageConfiguration: IEntityTypeConfiguration<SeriesMessage>
{
    public void Configure(EntityTypeBuilder<SeriesMessage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Date).IsUnique();
    }
}