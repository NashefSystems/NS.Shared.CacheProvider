using Microsoft.Extensions.DependencyInjection;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.CacheProvider.Services;
using StackExchange.Redis;

namespace NS.Shared.CacheProvider.Extensions
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddNSCacheProvider(this IServiceCollection services)
        {
            var configurationOptions = ConfigurationOptions.Parse($"{Consts.REDIS_HOST}:{Consts.REDIS_PORT}");
            configurationOptions.User = Consts.REDIS_USERNAME;
            configurationOptions.Password = Consts.REDIS_PASSWORD;
            configurationOptions.Ssl = false;
            configurationOptions.AbortOnConnectFail = false; // Ensure retry on connection failure

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configurationOptions));
            services.AddSingleton<INSCacheProvider, NSCacheProvider>();
            return services;
        }
    }
}
