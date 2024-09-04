using Newtonsoft.Json;
using NS.Shared.CacheProvider.Interfaces;
using StackExchange.Redis;

namespace NS.Shared.CacheProvider.Services
{
    internal class NSCacheProvider : INSCacheProvider
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        private const string HASH_CREATE_AT_KEY = "CreateAt";
        private const string HASH_EXPIRY_AT_KEY = "ExpiryAt";
        private const string HASH_MACHINE_NAME_KEY = "MachineName";
        private const string HASH_DATA_KEY = "Data";

        public NSCacheProvider(IConnectionMultiplexer connectionMultiplexer)
        {
            _redis = connectionMultiplexer;
            _database = _redis.GetDatabase();
        }

        public async Task SetOrUpdateAsync<T>(string key, T value, TimeSpan? expiryTime = null)
        {
            if (value == null)
            {
                await DeleteAsync<T>(key);
                return;
            }

            var createAt = DateTimeOffset.Now;
            var json = JsonConvert.SerializeObject(value, Formatting.Indented);
            await _database.HashSetAsync(key, HASH_CREATE_AT_KEY, createAt.ToString(), When.Always);
            await _database.HashSetAsync(key, HASH_MACHINE_NAME_KEY, Environment.MachineName, When.Always);
            await _database.HashSetAsync(key, HASH_DATA_KEY, json, When.Always);
            if (expiryTime != null)
            {
                await _database.HashSetAsync(key, HASH_EXPIRY_AT_KEY, createAt.Add(expiryTime.Value).ToString(), When.Always);
                await _database.KeyExpireAsync(key, expiryTime.Value);
            }
        }

        public async Task DeleteAsync<T>(string key)
        {
            await _database.HashDeleteAsync(key, HASH_DATA_KEY);
            await _database.HashDeleteAsync(key, HASH_MACHINE_NAME_KEY);
            await _database.HashDeleteAsync(key, HASH_CREATE_AT_KEY);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var redisValue = await _database.HashGetAsync(key, HASH_DATA_KEY);
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
