using System;
using System.IO;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public class FileCacheProvider : ICacheProvider
    {
        private readonly string _cacheName;
        private readonly TimeSpan _timeToLive;

        public FileCacheProvider(string fileCacheName = "publicsuffixcache.dat", TimeSpan? cacheTimeToLive = null)
        {
            if (cacheTimeToLive.HasValue)
            {
                this._timeToLive = cacheTimeToLive.Value;
            }
            else
            {
                this._timeToLive = TimeSpan.FromDays(1);
            }

            this._cacheName = fileCacheName;
        }

        public bool IsCacheValid()
        {
            var cacheInvalid = true;

            var fileInfo = new FileInfo(this._cacheName);
            if (fileInfo.Exists)
            {
                if (fileInfo.LastWriteTimeUtc > DateTime.UtcNow.Subtract(this._timeToLive))
                {
                    cacheInvalid = false;
                }
            }

            return !cacheInvalid;
        }

        public Task<string> GetValueAsync()
        {
            if (!this.IsCacheValid())
            {
                return Task.FromResult<string>(null);
            }
            return Task.FromResult(File.ReadAllText(this._cacheName));
        }

        public async Task SetValueAsync(string val)
        {
            using (var streamWriter = File.CreateText(this._cacheName))
            {
                await streamWriter.WriteAsync(val).ConfigureAwait(false);
            }
        }
    }
}
