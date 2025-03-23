using Lumen.Modules.GoodReads.Common.Models;

using Microsoft.EntityFrameworkCore;

namespace Lumen.Modules.GoodReads.Data {
    public class GoodReadsContext : DbContext {
        public const string SCHEMA_NAME = "GoodReads";

        public GoodReadsContext(DbContextOptions options) : base(options) {
        }

        public DbSet<GoodReadsPointInTime> GoodReads { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema(SCHEMA_NAME);

            var GoodReadsModelBuilder = modelBuilder.Entity<GoodReadsPointInTime>();
            GoodReadsModelBuilder.Property(x => x.Time)
                .HasColumnType("timestamp with time zone");

            GoodReadsModelBuilder.Property(x => x.AmountVideos)
                .HasColumnType("integer");

            GoodReadsModelBuilder.Property(x => x.SecondsDuration)
                .HasColumnType("integer");

            GoodReadsModelBuilder.HasKey(x => x.Time);
        }
    }
}
