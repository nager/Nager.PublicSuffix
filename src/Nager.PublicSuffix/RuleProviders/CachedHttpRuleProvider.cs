using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nager.PublicSuffix.Exceptions;
using Nager.PublicSuffix.RuleParsers;
using Nager.PublicSuffix.RuleProviders.CacheProviders;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders
{
    /// <summary>
    /// CachedHttp RuleProvider
    /// </summary>
    public class CachedHttpRuleProvider : BaseRuleProvider
    {
        private readonly string _dataFileUrl;
        private readonly ILogger<CachedHttpRuleProvider> _logger;
        private readonly ICacheProvider _cacheProvider;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Returns the cache provider
        /// </summary>
        public ICacheProvider CacheProvider { get { return this._cacheProvider; } }

        /// <summary>
        /// CachedHttp RuleProvider<br/>
        /// Loads the public suffix definition file from the official website and use a local cache for quicker initialization
        /// </summary>
        /// <remarks>It is possible to overwrite the url via configuration parameters <c>Nager:PublicSuffix:DataUrl</c></remarks>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="cacheProvider"></param>
        /// <param name="httpClient"></param>
        [ActivatorUtilitiesConstructor]
        public CachedHttpRuleProvider(
            ILogger<CachedHttpRuleProvider> logger,
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

        /// <summary>
        /// CachedHttp RuleProvider<br/>
        /// Loads the public suffix definition file from the official website and use a local cache for quicker initialization
        /// </summary>
        /// <remarks>It is possible to overwrite the url via configuration parameters <c>Nager:PublicSuffix:DataUrl</c></remarks>
        /// <param name="cacheProvider"></param>
        /// <param name="httpClient"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public CachedHttpRuleProvider(
            ICacheProvider cacheProvider,
            HttpClient httpClient,
            IConfiguration? configuration = default,
            ILogger<CachedHttpRuleProvider>? logger = default
            )
        {
            this._cacheProvider = cacheProvider;
            this._httpClient = httpClient;
            this._logger = logger ?? new NullLogger<CachedHttpRuleProvider>();

            var url = "https://publicsuffix.org/list/public_suffix_list.dat";
            if (configuration != default)
            {
                var tempUrl = configuration["Nager:PublicSuffix:DataUrl"];
                if (!string.IsNullOrEmpty(tempUrl))
                {
                    url = tempUrl;
                }
            }

            this._dataFileUrl = url;
        }

        /// <inheritdoc/>
        public override async Task<bool> BuildAsync(
            bool ignoreCache = false,
            CancellationToken cancellationToken = default)
        {
            string? ruleData;
            if (this._cacheProvider.IsCacheValid() && ignoreCache == false)
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

            if (string.IsNullOrEmpty(ruleData))
            {
                return false;
            }

            var ruleParser = new TldRuleParser();
            var rules = ruleParser.ParseRules(ruleData);

            base.CreateDomainDataStructure(rules);

            return true;
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
                throw new RuleLoadException($"Cannot load file from {url} StatusCode:{(int)response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}