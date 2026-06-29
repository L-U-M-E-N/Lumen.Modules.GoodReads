using Lumen.Modules.Sdk;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lumen.Modules.GoodReads.Module.Tests
{
    public class GoodReadsModuleTests
    {
        public const string RSS_URL = "PUTURLHERE";

        [Fact]
        public async Task EnsureItWorks()
        {
            var items = await GoodReadsModule.ParseAndSaveFeedItems(RSS_URL, NullLogger<LumenModuleBase>.Instance, CancellationToken.None);
            Assert.NotEmpty(items);
        }
    }
}
