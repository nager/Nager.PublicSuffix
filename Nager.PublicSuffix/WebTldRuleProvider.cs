using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public class WebTldRuleProvider : ITldRuleProvider
    {
        private readonly string _fileUrl;
        private readonly string _fileCacheName;
        private readonly TimeSpan _cacheTimeToLive;

        public WebTldRuleProvider(string url = "https://publicsuffix.org/list/public_suffix_list.dat", string fileCacheName = "publicsuffixcache.dat", TimeSpan? cacheTimeToLive = null)
        {
            this._fileUrl = url;

            if (cacheTimeToLive.HasValue)
            {
                this._cacheTimeToLive = cacheTimeToLive.Value;
            }
            else
            {
                this._cacheTimeToLive = TimeSpan.FromDays(1);
            }

            this._fileCacheName = fileCacheName;
        }

        public async Task<IEnumerable<TldRule>> BuildAsync()
        {
            var reloadData = true;
            var ruleParser = new TldRuleParser();

            var fileInfo = new FileInfo(this._fileCacheName);
            if (fileInfo.Exists)
            {
                if (fileInfo.LastWriteTimeUtc > DateTime.UtcNow.Subtract(this._cacheTimeToLive))
                {
                    reloadData = false;
                }
            }

            string ruleData;
            if (reloadData)
            {
                ruleData = await this.LoadFromUrl(this._fileUrl).ConfigureAwait(false);
                using (var streamWriter = File.CreateText(this._fileCacheName))
                {
                    await streamWriter.WriteAsync(ruleData).ConfigureAwait(false);
                }
            }
            else
            {
                ruleData = File.ReadAllText(this._fileCacheName);
            }

            var rules = ruleParser.ParseRules(ruleData);

            return rules;
        }

        public async Task<string> LoadFromUrl(string url)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url).ConfigureAwait(false))
                {
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
