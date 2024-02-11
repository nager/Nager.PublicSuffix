using Nager.PublicSuffix.CacheProviders;
using Nager.PublicSuffix.Exceptions;
using Nager.PublicSuffix.Models;
using Nager.PublicSuffix.RuleParsers;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders
{
    /// <summary>
    /// Web2TldRuleProvider
    /// </summary>
    public class Web2TldRuleProvider : ITopLevelDomainRuleProvider
    {
        private readonly string _fileUrl;
        private readonly ICacheProvider _cacheProvider;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Returns the cache provider
        /// </summary>
        public ICacheProvider CacheProvider { get { return this._cacheProvider; } }

        /// <summary>
        /// Web2TldRuleProvider<br/>
        /// Loads the public suffix definition file from a given url
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <param name="cacheProvider">default is <see cref="FileCacheProvider"/></param>
        public Web2TldRuleProvider(
            HttpClient httpClient,
            string url = "https://publicsuffix.org/list/public_suffix_list.dat",
            ICacheProvider cacheProvider = null)
        {
            this._httpClient = httpClient;
            this._fileUrl = url;

            if (cacheProvider == null)
            {
                this._cacheProvider = new FileCacheProvider();
                return;
            }

            this._cacheProvider = cacheProvider;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<TldRule>> BuildAsync()
        {
            var ruleParser = new TldRuleParser();

            string ruleData;
            if (this._cacheProvider.IsCacheValid())
            {
                ruleData = await this._cacheProvider.GetAsync().ConfigureAwait(false);
            }
            else
            {
                ruleData = await this.LoadFromUrlAsync(this._fileUrl).ConfigureAwait(false);
                await this._cacheProvider.SetAsync(ruleData).ConfigureAwait(false);
            }

            var rules = ruleParser.ParseRules(ruleData);
            return rules;
        }

        /// <summary>
        /// Load the public suffix data from the given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> LoadFromUrlAsync(string url)
        {
            using var response = await this._httpClient.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new RuleLoadException($"Cannot load from {url} {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}