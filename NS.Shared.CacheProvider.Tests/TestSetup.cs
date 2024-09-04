using Microsoft.Extensions.DependencyInjection;
using NS.Shared.CacheProvider.Extensions;

namespace NS.Shared.CacheProvider.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
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
        public void GlobalTeardown()
        {
            Console.WriteLine("Completed Tests - Global Teardown");
            ServiceProvider?.Dispose();
            ServiceProvider = null;
        }
    }
}
