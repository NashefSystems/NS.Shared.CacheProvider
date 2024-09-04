using Microsoft.Extensions.DependencyInjection;
using NS.Shared.CacheProvider.Extensions;
using NS.Shared.CacheProvider.Interfaces;
using System;

namespace NS.Shared.CacheProvider.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        private static Guid _runId = Guid.NewGuid();
        private static DateTimeOffset _startAt = DateTimeOffset.Now;
        public static ServiceProvider? ServiceProvider { get; private set; }

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            Console.WriteLine("Starting Tests - Global Setup");
            ServiceProvider ??= GetServiceProvider();
        }

        private static ServiceProvider GetServiceProvider()
        {
            Console.WriteLine("> GetServiceProvider");
            var services = new ServiceCollection();
            services.AddNSCacheProvider();
            return services.BuildServiceProvider();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardownAsync()
        {
            Console.WriteLine("Completed Tests - Global Teardown");
            await AddRunInfoCache();
            ServiceProvider?.Dispose();
            ServiceProvider = null;
        }

        private async Task AddRunInfoCache()
        {
            var finishAt = DateTimeOffset.Now;
            var runInfo = new
            {
                RunId = _runId,
                StartAt = _startAt,
                FinishAt = finishAt,
                Duration = finishAt - _startAt,
            };
            var cache = ServiceProvider.GetRequiredService<INSCacheProvider>();
            var key = $"Tests:Runs:NS.Shared.CacheProvider.Tests:{_startAt:yyyy-MM-dd}:{_runId}";
            await cache.SetOrUpdateAsync(key, runInfo);
        }
    }
}
