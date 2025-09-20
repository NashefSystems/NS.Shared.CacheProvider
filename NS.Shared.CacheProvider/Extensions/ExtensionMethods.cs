using Microsoft.Extensions.DependencyInjection;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.CacheProvider.models;
using NS.Shared.CacheProvider.Services;
using StackExchange.Redis;

namespace NS.Shared.CacheProvider.Extensions
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddNSCacheProvider(this IServiceCollection services, NSCacheProviderConfigs configs)
        {
            if (configs == null)
            {
                throw new ArgumentNullException(nameof(configs));
            }

            // defaults
            configs.CachePrefix ??= string.Empty;
            configs.Port ??= 6379;

            var configurationOptions = ConfigurationOptions.Parse($"{configs.Host}:{configs.Port}");
            if (!string.IsNullOrWhiteSpace(configs.UserName) || !string.IsNullOrWhiteSpace(configs.Password))
            {
                configurationOptions.User = configs.UserName;
                configurationOptions.Password = configs.Password;
            }
            configurationOptions.Ssl = configs.Ssl;
            configurationOptions.AbortOnConnectFail = configs.AbortOnConnectFail; // Ensure retry on connection failure

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configurationOptions));
            services.AddSingleton(configs);
            services.AddSingleton<INSCacheProvider, NSCacheProvider>();
            return services;
        }
    }
}
