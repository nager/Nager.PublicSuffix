using System;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders.CacheProviders
{
    /// <summary>
    /// MemoryCacheProvider, uses memory to cache the data, mostly used for testing Azure functions
    /// </summary>
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly TimeSpan _timeToLive;
        private DateTime _lastWriteTimeUtc;
        private string? _data;

        /// <summary>
        /// MemoryCacheProvider
        /// </summary>
        /// <param name="cacheTimeToLive"></param>
        public MemoryCacheProvider(TimeSpan? cacheTimeToLive = null)
        {
            _timeToLive = cacheTimeToLive ?? TimeSpan.FromDays(1);
        }

        /// <inheritdoc/>
        public bool IsCacheValid()
        {
            return _lastWriteTimeUtc > DateTime.UtcNow.Subtract(_timeToLive);
        }

        /// <inheritdoc/>
        public Task<string?> GetAsync()
        {
            return Task.FromResult(IsCacheValid() ? _data : null);
        }

        /// <inheritdoc/>
        public Task SetAsync(string data)
        {
            _data = data;
            _lastWriteTimeUtc = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}
