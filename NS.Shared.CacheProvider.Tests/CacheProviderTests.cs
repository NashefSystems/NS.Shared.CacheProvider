using Microsoft.Extensions.DependencyInjection;
using NS.Shared.CacheProvider.Services;

namespace NS.Shared.CacheProvider.Tests
{
    public class CacheProviderTests : TestFixture
    {
        private const string KEY_PREFIX = "Tests:NS_Shared_CacheProvider:CacheProviderTests";
        private INSCacheProvider _cacheProvider;
        private readonly List<string> _list = ["NashefSystems", "CacheProvider", "Test"];

        [SetUp]
        public void Setup()
        {
            _cacheProvider = ServiceProvider.GetRequiredService<INSCacheProvider>();
        }

        [Test, Order(2)]
        public async Task GetKeysAsync()
        {
            var keys = await _cacheProvider.GetKeysAsync();
            Assert.That(keys, Is.Not.Null);
            Console.WriteLine($"{keys.Count} total keys:");
            for (var i = 0; i < keys.Count; i++)
            {
                Console.WriteLine($"[{i}] {keys[i]}");
            }
        }


        [Test, Order(1)]
        public async Task SetValueAsync()
        {
            var res = await _cacheProvider.SetOrUpdateAsync($"{KEY_PREFIX}:list", _list);
            Assert.That(res, Is.True);
        }

        [Test, Order(2)]
        public async Task GetValueAsync()
        {
            var res = await _cacheProvider.GetAsync<List<string>>($"{KEY_PREFIX}:list");
            Assert.That(res, Is.Not.Null);
            Assert.That(res, Has.Count.EqualTo(_list.Count));
        }

        [Test, Order(1)]
        public async Task SetNullValueAsync()
        {
            var res = await _cacheProvider.SetOrUpdateAsync<object>($"{KEY_PREFIX}:null_value", null);
            Assert.That(res, Is.True);
        }

        [Test, Order(2)]
        public async Task GetNullValueAsync()
        {
            var res = await _cacheProvider.GetAsync<object>($"{KEY_PREFIX}:null_value");
            Assert.That(res, Is.Null);
        }
    }
}