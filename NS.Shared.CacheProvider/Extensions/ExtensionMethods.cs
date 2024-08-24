using Microsoft.Extensions.DependencyInjection;
using NS.Shared.CacheProvider.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            services.AddScoped<INSCacheProvider, NSCacheProvider>();

            return services;
        }
    }
}
