using Newtonsoft.Json;
using StackExchange.Redis;

namespace NS.Shared.CacheProvider.Services
{
    public interface INSCacheProvider
    {
        Task<List<string>> GetKeysAsync();
        Task<T?> GetAsync<T>(string key);
        Task<T?> GetDeleteAsync<T>(string key);
        Task<bool> SetOrUpdateAsync<T>(string key, T value, TimeSpan? expiryTime = null);
    }

    internal class NSCacheProvider : INSCacheProvider
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public NSCacheProvider(IConnectionMultiplexer connectionMultiplexer)
        {
            _redis = connectionMultiplexer;
            _database = _redis.GetDatabase();
        }

        public async Task<bool> SetOrUpdateAsync<T>(string key, T value, TimeSpan? expiryTime = null)
        {
            if (value == null)
            {
                await GetDeleteAsync<T>(key);
                return true;
            }

            var json = JsonConvert.SerializeObject(value, Formatting.Indented);
            return await _database.StringSetAsync(key, json, expiryTime, When.Always);
        }

        public async Task<T?> GetDeleteAsync<T>(string key)
        {
            var redisValue = await _database.StringGetDeleteAsync(key);
            if (!redisValue.HasValue)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(redisValue.ToString());
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var redisValue = await _database.StringGetAsync(key);
            if (!redisValue.HasValue)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(redisValue.ToString());
        }

        public Task<List<string>> GetKeysAsync()
        {
            var server = _redis.GetServer(Consts.REDIS_HOST, Consts.REDIS_PORT);
            var res = server?
                .Keys(database: _database.Database)
                .Select(x => x.ToString())
                .ToList();
            return Task.FromResult(res ?? []);
        }
    }
}
