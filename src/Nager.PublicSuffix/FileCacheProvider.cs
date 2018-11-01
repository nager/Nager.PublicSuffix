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

        public Task<string> GetValueAsync()
        {
            if (!this.IsCacheValid())
            {
                return Task.FromResult<string>(null);
            }
            return Task.FromResult(File.ReadAllText(this._cacheFilePath));
        }

        public async Task SetValueAsync(string val)
        {
            using (var streamWriter = File.CreateText(this._cacheFilePath))
            {
                await streamWriter.WriteAsync(val).ConfigureAwait(false);
            }
        }
    }
}
