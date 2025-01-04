using Microsoft.Extensions.DependencyInjection;
using NS.Shared.CacheProvider.Interfaces;

namespace NS.Shared.CacheProvider.Tests
{
    public class CacheProviderTests : TestBase
    {
        private INSCacheProvider _cacheProvider;
        private const string KEY_PREFIX = "Tests:NS_Shared_CacheProvider:CacheProviderTests";
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
            try
            {
                await _cacheProvider.SetOrUpdateAsync($"{KEY_PREFIX}:list", _list, TimeSpan.FromMinutes(1));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
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
            try
            {
                await _cacheProvider.SetOrUpdateAsync<object>($"{KEY_PREFIX}:null_value", null);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test, Order(1)]
        public async Task SetValueWithTTLAsync()
        {
            try
            {
                var rnd = new Random();
                await _cacheProvider.SetOrUpdateAsync<object>($"{KEY_PREFIX}:ttl_value", rnd.Next(10000), TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test, Order(2)]
        public async Task GetNullValueAsync()
        {
            var res = await _cacheProvider.GetAsync<object>($"{KEY_PREFIX}:null_value");
            Assert.That(res, Is.Null);
        }

        [Test, Order(1)]
        public async Task GetNotExistsKeyWithDefaultValue()
        {
            var defaultValue = 5588;
            var res = await _cacheProvider.GetAsync<int>($"{KEY_PREFIX}:default_value", defaultValue);
            Assert.That(res, Is.EqualTo(defaultValue));
        }
    }
}