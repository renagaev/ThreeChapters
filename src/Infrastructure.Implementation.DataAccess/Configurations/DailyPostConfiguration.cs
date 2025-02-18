using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Implementation.DataAccess.Configurations;

public class DailyPostConfiguration: IEntityTypeConfiguration<DailyPost>
{
    public void Configure(EntityTypeBuilder<DailyPost> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Date).IsUnique();
    }
}