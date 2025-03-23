using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lumen.Modules.GoodReads.Data {
    public class GoodReadsDbContextFactory : IDesignTimeDbContextFactory<GoodReadsContext> {
        public GoodReadsContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<GoodReadsContext>();
            optionsBuilder.UseNpgsql();

            return new GoodReadsContext(optionsBuilder.Options);
        }
    }
}
