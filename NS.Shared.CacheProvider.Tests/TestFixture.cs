using Microsoft.Extensions.DependencyInjection;
using NS.Shared.CacheProvider.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NS.Shared.CacheProvider.Tests
{
    [TestFixture]
    public class TestFixture
    {
        public ServiceProvider ServiceProvider { get; private set; }

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            ServiceProvider = GetServiceProvider();
        }

        private static ServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddNSCacheProvider();
            return services.BuildServiceProvider();
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            ServiceProvider?.Dispose();
        }

        public void OnEntry([CallerMemberName] string? memberName = null)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | Entry | {memberName}");
        }

        public void OnExit([CallerMemberName] string? memberName = null)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | Exit  | {memberName}");
        }
    }
}
