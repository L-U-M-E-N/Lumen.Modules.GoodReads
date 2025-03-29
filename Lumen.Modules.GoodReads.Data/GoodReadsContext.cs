using Lumen.Modules.GoodReads.Common.Models;

using Microsoft.EntityFrameworkCore;

namespace Lumen.Modules.GoodReads.Data {
    public class GoodReadsContext : DbContext {
        public const string SCHEMA_NAME = "GoodReads";

        public GoodReadsContext(DbContextOptions options) : base(options) {
        }

        public DbSet<GoodReadsItem> GoodReadsItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema(SCHEMA_NAME);

            var GoodReadsModelBuilder = modelBuilder.Entity<GoodReadsItem>();
            GoodReadsModelBuilder.HasKey(x => x.Date);
        }
    }
}
