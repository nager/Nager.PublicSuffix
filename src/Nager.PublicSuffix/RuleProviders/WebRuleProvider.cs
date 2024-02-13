using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nager.PublicSuffix.CacheProviders;
using Nager.PublicSuffix.Exceptions;
using Nager.PublicSuffix.Extensions;
using Nager.PublicSuffix.Models;
using Nager.PublicSuffix.RuleParsers;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders
{
    /// <summary>
    /// WebRuleProvider
    /// </summary>
    public class WebRuleProvider : IRuleProvider
    {
        private readonly string _dataFileUrl;
        private readonly ILogger<WebRuleProvider> _logger;
        private readonly ICacheProvider _cacheProvider;
        private readonly HttpClient _httpClient;
        private DomainDataStructure _domainDataStructure;

        /// <summary>
        /// Returns the cache provider
        /// </summary>
        public ICacheProvider CacheProvider { get { return this._cacheProvider; } }

        /// <summary>
        /// WebRuleProvider<br/>
        /// Loads the public suffix definition file from the official website
        /// </summary>
        /// <remarks>It is possible to overwrite the url via configuration parameters <c>Nager:PublicSuffix:DataUrl</c></remarks>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="cacheProvider"></param>
        /// <param name="httpClient"></param>
        public WebRuleProvider(
            ILogger<WebRuleProvider> logger,
            IConfiguration configuration,
            ICacheProvider cacheProvider,
            HttpClient httpClient
            )
        {
            this._logger = logger;
            this._cacheProvider = cacheProvider;
            this._httpClient = httpClient;

            var url = configuration["Nager:PublicSuffix:DataUrl"];
            if (string.IsNullOrEmpty(url))
            {
                url = "https://publicsuffix.org/list/public_suffix_list.dat";
            }

            this._dataFileUrl = url;
        }

        /// <inheritdoc/>
        public async Task<bool> BuildAsync(
            CancellationToken cancellationToken = default)
        {
            var ruleParser = new TldRuleParser();

            string ruleData;
            if (this._cacheProvider.IsCacheValid())
            {
                this._logger.LogInformation($"{nameof(BuildAsync)} - Use data from cache");
                ruleData = await this._cacheProvider.GetAsync().ConfigureAwait(false);
            }
            else
            {
                this._logger.LogInformation($"{nameof(BuildAsync)} - Start downloading data from url");

                try
                {
                    ruleData = await this.LoadFromUrlAsync(this._dataFileUrl, cancellationToken).ConfigureAwait(false);
                    await this._cacheProvider.SetAsync(ruleData).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    this._logger.LogError(exception, $"{nameof(BuildAsync)} - Failure on download");

                    return false;
                }
            }

            var rules = ruleParser.ParseRules(ruleData);

            var domainDataStructure = new DomainDataStructure("*", new TldRule("*"));
            domainDataStructure.AddRules(rules);

            this._domainDataStructure  = domainDataStructure;

            return true;
        }

        /// <inheritdoc/>
        public DomainDataStructure? GetDomainDataStructure()
        {
            return this._domainDataStructure;
        }

        /// <summary>
        /// Load the public suffix data from the given url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> LoadFromUrlAsync(
            string url,
            CancellationToken cancellationToken)
        {
            using var response = await this._httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new RuleLoadException($"Cannot load from {url} {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}