﻿// <auto-generated />
using System;
using Infrastructure.Implementation.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Implementation.DataAccess.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ChaptersCount")
                        .HasColumnType("integer")
                        .HasColumnName("chapters_count");

                    b.Property<string>("GroupTitle")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("group_title");

                    b.Property<string>("Testament")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("testament");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<string[]>("TitleVariants")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("title_variants");

                    b.HasKey("Id")
                        .HasName("pk_books");

                    b.ToTable("books", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.DailyPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_id");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date")
                        .HasColumnName("date");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer")
                        .HasColumnName("message_id");

                    b.HasKey("Id")
                        .HasName("pk_daily_posts");

                    b.HasIndex("Date")
                        .IsUnique()
                        .HasDatabaseName("ix_daily_posts_date");

                    b.ToTable("daily_posts", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Participant", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<long?>("TelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_id");

                    b.HasKey("Id")
                        .HasName("pk_participants");

                    b.ToTable("participants", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.ReadEntry", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("BookId")
                        .HasColumnType("integer")
                        .HasColumnName("book_id");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date")
                        .HasColumnName("date");

                    b.Property<int>("EndChapter")
                        .HasColumnType("integer")
                        .HasColumnName("end_chapter");

                    b.Property<long>("ParticipantId")
                        .HasColumnType("bigint")
                        .HasColumnName("participant_id");

                    b.Property<int>("StartChapter")
                        .HasColumnType("integer")
                        .HasColumnName("start_chapter");

                    b.HasKey("Id")
                        .HasName("pk_read_entries");

                    b.HasIndex("BookId")
                        .HasDatabaseName("ix_read_entries_book_id");

                    b.HasIndex("ParticipantId")
                        .HasDatabaseName("ix_read_entries_participant_id");

                    b.ToTable("read_entries", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.SeriesMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_id");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date")
                        .HasColumnName("date");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer")
                        .HasColumnName("message_id");

                    b.HasKey("Id")
                        .HasName("pk_series_messages");

                    b.HasIndex("Date")
                        .IsUnique()
                        .HasDatabaseName("ix_series_messages_date");

                    b.ToTable("series_messages", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.ReadEntry", b =>
                {
                    b.HasOne("Domain.Entities.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_read_entries_books_book_id");

                    b.HasOne("Domain.Entities.Participant", "Participant")
                        .WithMany("ReadEntries")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_read_entries_participants_participant_id");

                    b.Navigation("Book");

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("Domain.Entities.Participant", b =>
                {
                    b.Navigation("ReadEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
