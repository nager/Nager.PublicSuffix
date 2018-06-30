using System;
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
            }
            else
            {
                this._cacheProvider = cacheProvider;
            }
        }

        public async Task<IEnumerable<TldRule>> BuildAsync()
        {
            var ruleParser = new TldRuleParser();

            var ruleData = await this._cacheProvider.GetValueAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(ruleData))
            {
                ruleData = await this.LoadFromUrl(this._fileUrl).ConfigureAwait(false);
                await this._cacheProvider.SetValueAsync(ruleData).ConfigureAwait(false);
            }

            var rules = ruleParser.ParseRules(ruleData);
            return rules;
        }

        public async Task<string> LoadFromUrl(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(url).ConfigureAwait(false))
                    {
                        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception exception)
            {
                return await Task.FromResult("error");
            }
         }
    }
}
