using Newtonsoft.Json;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.CacheProvider.models;
using StackExchange.Redis;

namespace NS.Shared.CacheProvider.Services
{
    internal class NSCacheProvider : INSCacheProvider
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly NSCacheProviderConfigs _cacheConfigs;

        private const string HASH_CREATE_AT_KEY = "CreateAt";
        private const string HASH_EXPIRY_AT_KEY = "ExpiryAt";
        private const string HASH_MACHINE_NAME_KEY = "MachineName";
        private const string HASH_DATA_KEY = "Data";

        public NSCacheProvider(IConnectionMultiplexer connectionMultiplexer, NSCacheProviderConfigs cacheConfigs)
        {
            _redis = connectionMultiplexer;
            _cacheConfigs = cacheConfigs;
            _database = _redis.GetDatabase();
        }

        private string GetFullKey(string key)
        {
            if (string.IsNullOrWhiteSpace(_cacheConfigs?.CachePrefix))
            {
                return key;
            }
            return $"{_cacheConfigs.CachePrefix}:{key}";
        }

        public Task<List<string>> GetKeysAsync()
        {
            var server = _redis.GetServer($"{_cacheConfigs.Host}:{_cacheConfigs.Port}");
            var res = server?
                .Keys(database: _database.Database)
                .Select(x => x.ToString())
                .Where(x => x.StartsWith(_cacheConfigs.CachePrefix ))
                .ToList();
            return Task.FromResult(res ?? []);
        }

        public async Task<T?> GetAsync<T>(string key, T defaultValue = default)
        {
            var fullKey = GetFullKey(key);
            var redisValue = await _database.HashGetAsync(fullKey, HASH_DATA_KEY);
            if (!redisValue.HasValue)
            {
                return defaultValue;
            }
            return JsonConvert.DeserializeObject<T>(redisValue.ToString());
        }

        public async Task SetOrUpdateAsync<T>(string key, T value, TimeSpan? expiryTime = null)
        {
            var fullKey = GetFullKey(key);
            if (value == null)
            {
                await DeleteAsync(key);
                return;
            }

            var createAt = DateTimeOffset.Now;
            var json = JsonConvert.SerializeObject(value, Formatting.Indented);
            await _database.HashSetAsync(fullKey, HASH_CREATE_AT_KEY, createAt.ToString(), When.Always);
            await _database.HashSetAsync(fullKey, HASH_MACHINE_NAME_KEY, Environment.MachineName, When.Always);
            await _database.HashSetAsync(fullKey, HASH_DATA_KEY, json, When.Always);
            await _database.HashDeleteAsync(fullKey, HASH_EXPIRY_AT_KEY);
            if (expiryTime != null)
            {
                await _database.HashSetAsync(fullKey, HASH_EXPIRY_AT_KEY, createAt.Add(expiryTime.Value).ToString(), When.Always);
                await _database.KeyExpireAsync(fullKey, expiryTime.Value);
            }
        }

        public async Task DeleteAsync(string key)
        {
            var fullKey = GetFullKey(key);
            await _database.HashDeleteAsync(fullKey, HASH_DATA_KEY);
            await _database.HashDeleteAsync(fullKey, HASH_MACHINE_NAME_KEY);
            await _database.HashDeleteAsync(fullKey, HASH_CREATE_AT_KEY);
            await _database.HashDeleteAsync(fullKey, HASH_EXPIRY_AT_KEY);
        }
    }
}
