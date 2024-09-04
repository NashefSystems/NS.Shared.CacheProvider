using Microsoft.Extensions.DependencyInjection;
using NS.Shared.CacheProvider.Extensions;
using NS.Shared.CacheProvider.Interfaces;
using System;
using System.Reflection;

namespace NS.Shared.CacheProvider.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        private readonly static Guid _runId = Guid.NewGuid();
        private readonly static DateTimeOffset _startAt = DateTimeOffset.Now;
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

        private static async Task AddRunInfoCache()
        {
            var projectName = Assembly.GetExecutingAssembly().GetName().Name;
            var finishAt = DateTimeOffset.Now;
            var runInfo = new
            {
                RunId = _runId,
                StartAt = _startAt,
                FinishAt = finishAt,
                Duration = finishAt - _startAt,
            };
            var cache = ServiceProvider.GetRequiredService<INSCacheProvider>();
            var key = $"Tests:Runs:{projectName}:{_startAt:yyyy-MM-dd}:{_runId}";
            await cache.SetOrUpdateAsync(key, runInfo);
        }
    }
}
