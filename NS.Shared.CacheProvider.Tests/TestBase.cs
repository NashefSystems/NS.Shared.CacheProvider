using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace NS.Shared.CacheProvider.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        //protected static ServiceProvider? ServiceProvider => TestSetup.ServiceProvider;

        [SetUp]
        public void TestSetupMethod()
        {
            OnEntry();
        }

        [TearDown]
        public void TestTeardownMethod()
        {
            OnExit();
        }

        protected virtual void OnEntry([CallerMemberName] string? memberName = null)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | Entry | {memberName}");
        }

        protected virtual void OnExit([CallerMemberName] string? memberName = null)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | Exit  | {memberName}");
        }
    }
}
