using System;
using System.IO;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public class FileCacheProvider : ICacheProvider
    {
        private readonly string _cacheFilePath;
        private readonly TimeSpan _timeToLive;

        public FileCacheProvider(string cacheFileName = "publicsuffixcache.dat", TimeSpan? cacheTimeToLive = null)
        {
            if (cacheTimeToLive.HasValue)
            {
                this._timeToLive = cacheTimeToLive.Value;
            }
            else
            {
                this._timeToLive = TimeSpan.FromDays(1);
            }

            var tempPath = Path.GetTempPath();
            this._cacheFilePath = Path.Combine(tempPath, cacheFileName);
        }

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

        public async Task<string> GetAsync()
        {
            if (!this.IsCacheValid())
            {
                return null;
            }

            using (var reader = File.OpenText(this._cacheFilePath))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        public async Task SetAsync(string data)
        {
            using (var streamWriter = File.CreateText(this._cacheFilePath))
            {
                await streamWriter.WriteAsync(data).ConfigureAwait(false);
            }
        }
    }
}
