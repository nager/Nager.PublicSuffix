﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// FileCacheProvider
    /// Write the data to a cache file in the temp directory
    /// </summary>
    public class FileCacheProvider : ICacheProvider
    {
        private readonly string _cacheFilePath;
        private readonly TimeSpan _timeToLive;

        /// <summary>
        /// FileCacheProvider
        /// </summary>
        /// <param name="cacheFileName"></param>
        /// <param name="cacheTimeToLive"></param>
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

            var poolId = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);

            if (!string.IsNullOrEmpty(poolId))
            {
                Directory.CreateDirectory(Path.Combine(tempPath, poolId));

                this._cacheFilePath = Path.Combine(tempPath, poolId, cacheFileName);
            }
        }

        ///<inheritdoc/>
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

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public async Task SetAsync(string data)
        {
            using (var streamWriter = File.CreateText(this._cacheFilePath))
            {
                await streamWriter.WriteAsync(data).ConfigureAwait(false);
            }
        }
    }
}
