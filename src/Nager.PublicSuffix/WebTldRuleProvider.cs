using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public class WebTldRuleProvider : ITldRuleProvider
    {
        private readonly string _fileUrl;
        private readonly ICacheProvider _cacheProvider;

        public ICacheProvider CacheProvider { get { return this._cacheProvider; } }

        public WebTldRuleProvider(string url = "https://publicsuffix.org/list/public_suffix_list.dat", ICacheProvider cacheProvider = null)
        {
            this._fileUrl = url;

            if (cacheProvider == null)
            {
                this._cacheProvider = new FileCacheProvider();
                return;
            }

            this._cacheProvider = cacheProvider;
        }

        public async Task<IEnumerable<TldRule>> BuildAsync()
        {
            var ruleParser = new TldRuleParser();

            string ruleData;
            if (!this._cacheProvider.IsCacheValid())
            {
                ruleData = await this.LoadFromUrl(this._fileUrl).ConfigureAwait(false);
                await this._cacheProvider.SetAsync(ruleData).ConfigureAwait(false);
            }
            else
            {
                ruleData = await this._cacheProvider.GetAsync().ConfigureAwait(false);
            }

            var rules = ruleParser.ParseRules(ruleData);
            return rules;
        }

        public async Task<string> LoadFromUrl(string url)
        {
            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync(url).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new RuleLoadException($"Cannot load from {url} {response.StatusCode}");
                }

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }
}
