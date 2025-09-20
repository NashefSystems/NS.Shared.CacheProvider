namespace NS.Shared.CacheProvider.models
{
    public class NSCacheProviderConfigs
    {
        //internal const string REDIS_HOST = "int.nashefsys.com";
        //internal const int REDIS_PORT = 6379;
        //internal const string REDIS_USERNAME = "NS_Shared_CacheProvider";
        //internal const string REDIS_PASSWORD = "wZKOdLjkCLxp5Lt$AqKzlztefbGXUELN";
        public string Host { get; set; }
        public int? Port { get; set; }
        public string? UserName { get; set; }
        public string? Password{ get; set; }
        public string? CachePrefix { get; set; }
        public bool Ssl { get; set; }
        public bool AbortOnConnectFail { get; set; }
    }
}
