﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SolutionTemplate.Infrastructure.EF.Data;

#nullable disable

namespace SolutionTemplate.Infrastructure.EF.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230721150948_OutboxTweaks")]
    partial class OutboxTweaks
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SolutionTemplate.Domain.Models.Article", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("SolutionTemplate.Domain.Models.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("SolutionTemplate.Infrastructure.EF.Outbox.Entities.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<string>("Error")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OccurredOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ProcessedDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.HasKey("Id");

                    b.HasIndex("OccurredOnUtc");

                    b.ToTable("Outbox", (string)null);
                });

            modelBuilder.Entity("SolutionTemplate.Infrastructure.EF.Outbox.Entities.OutboxMessageConsumer", b =>
                {
                    b.Property<Guid>("OutboxMessageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.HasKey("OutboxMessageId", "Name");

                    b.ToTable("OutboxConsumer", (string)null);
                });

            modelBuilder.Entity("SolutionTemplate.Domain.Models.Article", b =>
                {
                    b.HasOne("SolutionTemplate.Domain.Models.Author", null)
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("SolutionTemplate.Domain.Models.Tag", "Tags", b1 =>
                        {
                            b1.Property<string>("Name")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<Guid>("ArticleId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("Name");

                            b1.HasIndex("ArticleId");

                            b1.ToTable("Tag");

                            b1.WithOwner()
                                .HasForeignKey("ArticleId");
                        });

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("SolutionTemplate.Infrastructure.EF.Outbox.Entities.OutboxMessageConsumer", b =>
                {
                    b.HasOne("SolutionTemplate.Infrastructure.EF.Outbox.Entities.OutboxMessage", null)
                        .WithMany()
                        .HasForeignKey("OutboxMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
