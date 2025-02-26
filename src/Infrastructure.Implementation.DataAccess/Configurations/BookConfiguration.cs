using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Implementation.DataAccess.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.TitleVariants)
            .HasColumnType("jsonb");

        builder.Property(x => x.Testament)
            .HasConversion(new EnumToStringConverter<Testament>());
    }
}