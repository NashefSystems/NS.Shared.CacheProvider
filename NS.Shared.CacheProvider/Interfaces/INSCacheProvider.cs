using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Shared.CacheProvider.Interfaces
{
    public interface INSCacheProvider
    {
        Task<List<string>> GetKeysAsync();
        Task<T?> GetAsync<T>(string key, T defaultValue = default);
        Task SetOrUpdateAsync<T>(string key, T value, TimeSpan? expiryTime = null);
        Task DeleteAsync(string key);
    }
}
