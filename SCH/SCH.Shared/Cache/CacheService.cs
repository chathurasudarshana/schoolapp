namespace SCH.Shared.Cache
{
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using System.Diagnostics.CodeAnalysis;

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public CacheService(
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _cache = cache;
            this._configuration = configuration;
        }

        #region Instance Methods (for Dependency Injection)

        [return: MaybeNull]
        T ICacheService.Get<T>(string key)
        {
            return Get<T>(_cache, key);
        }

        void ICacheService.Add<T>(
            string key, 
            T value,
            TimeSpan? absoluteExpirationRelativeToNow,
            TimeSpan? slidingExpiration,
            DateTimeOffset? absoluteExpiration)
        {
            Add(
                _cache, 
                _configuration, 
                key, 
                value, 
                absoluteExpirationRelativeToNow, 
                slidingExpiration, 
                absoluteExpiration);
        }

        void ICacheService.Remove(string key)
        {
            Remove(_cache, key);
        }

        void ICacheService.Clear()
        {
            Clear(_cache);
        }

        #endregion

        #region Static Methods (for direct access when IMemoryCache is available)

        [return: MaybeNull]
        public static T Get<T>(IMemoryCache cache, string key)
        {
            if (cache.TryGetValue(key, out object? rawValue) && rawValue is T typed)
            {
                return typed;
            }
            else
            {
                return default!;
            }

        }

        public static void Add<T>(
            IMemoryCache cache,
            IConfiguration configuration,
            string key, 
            T value,
            TimeSpan? absoluteExpirationRelativeToNow = null,
            TimeSpan? slidingExpiration = null,
            DateTimeOffset? absoluteExpiration = null)
        {
            MemoryCacheEntryOptions options = BuildOptions(
                configuration, 
                absoluteExpirationRelativeToNow, 
                slidingExpiration, 
                absoluteExpiration);
            cache.Set(key, value, options);
        }

        public static void Remove(IMemoryCache cache, string key)
        {
            cache.Remove(key);
        }

        /// <summary>
        /// Clears all entries. Requires the default MemoryCache implementation.
        /// </summary>
        public static void Clear(IMemoryCache cache)
        {
            (cache as MemoryCache)?.Clear();
        }

        #endregion

        private static MemoryCacheEntryOptions BuildOptions(
            IConfiguration configuration,
            TimeSpan? absoluteExpirationRelativeToNow,
            TimeSpan? slidingExpiration,
            DateTimeOffset? absoluteExpiration)
        {
            MemoryCacheEntryOptions options = new();

            if (absoluteExpirationRelativeToNow.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
            }


            if (slidingExpiration.HasValue)
            {
                options.SlidingExpiration = slidingExpiration;
            }


            if (absoluteExpiration.HasValue)
            {
                options.AbsoluteExpiration = absoluteExpiration;
            }

            if (!absoluteExpirationRelativeToNow.HasValue 
                && !slidingExpiration.HasValue
                && !absoluteExpiration.HasValue)
            {
                int cacheExpirationSeconds = configuration.GetValue<int>("AppSettings:CacheExpirationSeconds");
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheExpirationSeconds);
            }


            return options;
        }
    }
}
