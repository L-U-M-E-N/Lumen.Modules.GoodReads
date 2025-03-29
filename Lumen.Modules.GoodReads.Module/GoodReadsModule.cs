using Lumen.Modules.GoodReads.Common.Models;
using Lumen.Modules.GoodReads.Data;
using Lumen.Modules.Sdk;

using Microsoft.EntityFrameworkCore;

using System.ServiceModel.Syndication;
using System.Xml;

namespace Lumen.Modules.GoodReads.Module {
    public class GoodReadsModule(IEnumerable<ConfigEntry> configEntries, ILogger<LumenModuleBase> logger, IServiceProvider provider) : LumenModuleBase(configEntries, logger, provider) {
        public const string GOODREADS_RSS_URL = nameof(GOODREADS_RSS_URL);

        private string GetRssUrl() {
            var configEntry = configEntries.FirstOrDefault(x => x.ConfigKey == GOODREADS_RSS_URL);
            if (configEntry is null || configEntry.ConfigValue is null) {
                logger.LogError($"[{nameof(GoodReadsModule)}] Config key \"{GOODREADS_RSS_URL}\" is missing!");
                throw new NullReferenceException(nameof(configEntry));
            }

            return configEntry.ConfigValue;
        }

        public override async Task InitAsync(LumenModuleRunsOnFlag currentEnv) {
            await RunAsync(currentEnv, DateTime.UtcNow);
        }

        public override async Task RunAsync(LumenModuleRunsOnFlag currentEnv, DateTime date) {
            try {
                logger.LogTrace($"[{nameof(GoodReadsModule)}] Running tasks ...");
                var items = ParseAndSaveFeedItems();
                await SyncDataToDb(items);
            } catch (Exception ex) {
                logger.LogError(ex, $"[{nameof(GoodReadsModule)}] Error when running tasks.");
            }
        }

        public IEnumerable<GoodReadsItem> ParseAndSaveFeedItems() {
            string url = GetRssUrl();
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            List<GoodReadsItem> items = [];
            ExtractPercentageAdvancementProgressFromFeed(feed, items);
            ExtractFinishedBooksFromFeed(feed, items);

            return items;
        }

        private static void ExtractPercentageAdvancementProgressFromFeed(SyndicationFeed feed, List<GoodReadsItem> items) {
            var PERCENTAGE_DISCRIMINATOR_STR = "% done with ";
            var progressItemsPercentage = feed.Items.Where(x => x.Title.Text.Contains(PERCENTAGE_DISCRIMINATOR_STR)).ToList();
            foreach (var item in progressItemsPercentage) {
                var selectedLine = item.Title.Text.Split('\n').First(line => line.Contains(PERCENTAGE_DISCRIMINATOR_STR));
                var percentageStr = selectedLine.Split(PERCENTAGE_DISCRIMINATOR_STR)[0].Replace("is ", "").Trim();
                var bookName = selectedLine.Split(PERCENTAGE_DISCRIMINATOR_STR)[1].Trim();
                var percentage = int.Parse(percentageStr);

                items.Add(new GoodReadsItem {
                    BookName = bookName,
                    Date = item.PublishDate.ToUniversalTime().UtcDateTime,
                    Percentage = percentage,
                    PagesRead = null,
                    ProgressText = percentageStr + "%",
                    BookSize = null,
                });
            }
        }

        private static void ExtractFinishedBooksFromFeed(SyndicationFeed feed, List<GoodReadsItem> items) {
            var FINISHED_DISCRIMINATOR_STR = " finished reading ";
            var progressItemsFinished = feed.Items.Where(x => x.Title.Text.Contains(FINISHED_DISCRIMINATOR_STR)).ToList();
            foreach (var item in progressItemsFinished) {
                var selectedLine = item.Title.Text.Split('\n').First(line => line.Contains(FINISHED_DISCRIMINATOR_STR));
                var bookName = selectedLine.Split(FINISHED_DISCRIMINATOR_STR)[1].Replace("'", "").Trim();

                items.Add(new GoodReadsItem {
                    BookName = bookName,
                    Date = item.PublishDate.ToUniversalTime().UtcDateTime,
                    Percentage = 100,
                    PagesRead = null,
                    ProgressText = "100%",
                    BookSize = null,
                });
            }
        }

        private async Task SyncDataToDb(IEnumerable<GoodReadsItem> items) {
            using var scope = provider.CreateScope();
            var context = provider.GetRequiredService<GoodReadsContext>();
            items = items.OrderBy(x => x.BookName);

            var prevName = "";
            int? prevNameRelatedBookSize = null;
            foreach (var item in items) {
                if (context.GoodReadsItems.Any(x => x.Date == item.Date)) {
                    continue;
                }

                if (prevName != item.BookName) {
                    prevNameRelatedBookSize = context.GoodReadsItems.FirstOrDefault(x => x.BookName == item.BookName && x.BookSize != null)?.BookSize;
                }

                item.BookSize = prevNameRelatedBookSize;

                context.GoodReadsItems.Add(item);
            }

            await context.SaveChangesAsync();
        }

        public override bool ShouldRunNow(LumenModuleRunsOnFlag currentEnv, DateTime date) {
            return currentEnv switch {
                LumenModuleRunsOnFlag.API => date.Second == 0 && date.Minute == 40 && date.Hour == 6,
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
