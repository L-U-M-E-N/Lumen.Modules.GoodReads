﻿// <auto-generated />
using System;
using Lumen.Modules.GoodReads.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Lumen.Modules.GoodReads.Data.Migrations
{
    [DbContext(typeof(GoodReadsContext))]
    partial class GoodReadsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("GoodReads")
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Lumen.Modules.GoodReads.Common.Models.GoodReadsItem", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("BookName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("BookSize")
                        .HasColumnType("integer");

                    b.Property<int?>("PagesRead")
                        .HasColumnType("integer");

                    b.Property<int?>("Percentage")
                        .HasColumnType("integer");

                    b.Property<string>("ProgressText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Date");

                    b.ToTable("GoodReadsItems", "GoodReads");
                });
#pragma warning restore 612, 618
        }
    }
}
