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
        //private readonly string _fileCacheName;
        //private readonly TimeSpan _cacheTimeToLive;
        public ITldCacheProvider CacheProvider { get; }

        public WebTldRuleProvider(string url = "https://publicsuffix.org/list/public_suffix_list.dat", ITldCacheProvider cacheProvider = null/*, string fileCacheName = "publicsuffixcache.dat", TimeSpan? cacheTimeToLive = null*/)
        {
            this._fileUrl = url;

            if (cacheProvider == null)
            {
                CacheProvider = new FileCacheProvider();
            }
            else
            {
                CacheProvider = cacheProvider;
            }
        }

        public async Task<IEnumerable<TldRule>> BuildAsync()
        {
            var ruleParser = new TldRuleParser();

            var ruleData = await CacheProvider.GetValueAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(ruleData))
            {
                ruleData = await this.LoadFromUrl(this._fileUrl).ConfigureAwait(false);
                await CacheProvider.SetValueAsync(ruleData).ConfigureAwait(false);
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
