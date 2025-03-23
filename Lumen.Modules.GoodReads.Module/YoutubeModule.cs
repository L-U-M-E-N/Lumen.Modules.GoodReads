using Lumen.Modules.Sdk;
using Lumen.Modules.GoodReads.Business;
using Lumen.Modules.GoodReads.Common.Models;
using Lumen.Modules.GoodReads.Data;

using Microsoft.EntityFrameworkCore;

namespace Lumen.Modules.GoodReads.Module {
    public class GoodReadsModule(IEnumerable<ConfigEntry> configEntries, ILogger<LumenModuleBase> logger, IServiceProvider provider) : LumenModuleBase(configEntries, logger, provider) {
        public override Task InitAsync(LumenModuleRunsOnFlag currentEnv) {
            // Nothing to do
            return Task.CompletedTask;
        }

        public override async Task RunAsync(LumenModuleRunsOnFlag currentEnv) {
            try {
                logger.LogTrace($"[{nameof(GoodReadsModule)}] Running tasks ...");
                throw new NotImplementedException();
            } catch (Exception ex) {
                logger.LogError(ex, $"[{nameof(GoodReadsModule)}] Error when running tasks.");
            }
        }

        public override bool ShouldRunNow(LumenModuleRunsOnFlag currentEnv) {
            return currentEnv switch {
                LumenModuleRunsOnFlag.UI => DateTime.UtcNow.Second == 0 && DateTime.UtcNow.Minute == 27,
                LumenModuleRunsOnFlag.API => DateTime.UtcNow.Second == 0 && DateTime.UtcNow.Minute % 5 == 0,
                _ => false,
            };
        }

        public override Task ShutdownAsync() {
            // Nothing to do
            return Task.CompletedTask;
        }

        public static new void SetupServices(LumenModuleRunsOnFlag currentEnv, IServiceCollection serviceCollection, string? postgresConnectionString) {
            if (currentEnv == LumenModuleRunsOnFlag.API) {
                serviceCollection.AddDbContext<GoodReadsContext>(o => o.UseNpgsql(postgresConnectionString, x => x.MigrationsHistoryTable("__EFMigrationsHistory", GoodReadsContext.SCHEMA_NAME)));
            }
        }

        public override Type GetDatabaseContextType() {
            return typeof(GoodReadsContext);
        }
    }
}
