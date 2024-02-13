using System;
using System.IO;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.CacheProviders
{
    /// <summary>
    /// LocalFileSystemCacheProvider, write the data to a cache file in the temp directory
    /// </summary>
    public class LocalFileSystemCacheProvider : ICacheProvider
    {
        private readonly string _cacheFilePath;
        private readonly TimeSpan _timeToLive;

        /// <summary>
        /// LocalFileSystemCacheProvider
        /// </summary>
        /// <param name="cacheFileName"></param>
        /// <param name="cachePath">The path of the cache file, default use the temp path of the os</param>
        /// <param name="cacheTimeToLive"></param>
        public LocalFileSystemCacheProvider(
            string cacheFileName = "publicsuffixcache.dat",
            string? cachePath = null,
            TimeSpan? cacheTimeToLive = null)
        {
            if (cacheTimeToLive.HasValue)
            {
                this._timeToLive = cacheTimeToLive.Value;
            }
            else
            {
                this._timeToLive = TimeSpan.FromDays(1);
            }

            var tempPath = cachePath ?? Path.GetTempPath();
            this._cacheFilePath = Path.Combine(tempPath, cacheFileName);
        }

        /// <inheritdoc/>
        public bool IsCacheValid()
        {
            var cacheInvalid = true;

            var fileInfo = new FileInfo(this._cacheFilePath);
            if (fileInfo.Exists)
            {
                if (fileInfo.LastWriteTimeUtc > DateTime.UtcNow.Subtract(this._timeToLive))
                {
                    cacheInvalid = false;
                }
            }

            return !cacheInvalid;
        }

        /// <inheritdoc/>
        public async Task<string> GetAsync()
        {
            if (!this.IsCacheValid())
            {
                return null;
            }

            using var reader = File.OpenText(this._cacheFilePath);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task SetAsync(string data)
        {
            using var streamWriter = File.CreateText(this._cacheFilePath);
            await streamWriter.WriteAsync(data).ConfigureAwait(false);
        }
    }
}
