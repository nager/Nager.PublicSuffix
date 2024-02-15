using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
    /// Simple Http RuleProvider
    /// </summary>
    public class SimpleHttpRuleProvider : IRuleProvider, IDisposable
    {
        private bool _disposed;
        private readonly string _dataFileUrl;
        private readonly ILogger<SimpleHttpRuleProvider> _logger;
        private readonly HttpClient _httpClient;
        private readonly bool _disposeHttpClient;
        private DomainDataStructure? _domainDataStructure;

        /// <summary>
        /// Simple Http RuleProvider<br/>
        /// Loads the public suffix definition file from the official website
        /// </summary>
        /// <remarks>It is possible to overwrite the url via configuration parameters <c>Nager:PublicSuffix:DataUrl</c></remarks>
        /// <param name="configuration"></param>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        public SimpleHttpRuleProvider(
            IConfiguration configuration,
            HttpClient? httpClient = null,
            ILogger<SimpleHttpRuleProvider>? logger = null)
        {
            this._logger = logger ?? new NullLogger<SimpleHttpRuleProvider>();

            this._disposeHttpClient = httpClient == null;
            this._httpClient = httpClient ?? new HttpClient();

            var url = configuration["Nager:PublicSuffix:DataUrl"];
            if (string.IsNullOrEmpty(url))
            {
                url = "https://publicsuffix.org/list/public_suffix_list.dat";
            }

            this._dataFileUrl = url;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this._disposeHttpClient)
                {
                    this._httpClient.Dispose();
                }
            }

            this._disposed = true;
        }

        /// <inheritdoc/>
        public async Task<bool> BuildAsync(
            bool ignoreCache = false,
            CancellationToken cancellationToken = default)
        {
            string? ruleData;

            this._logger.LogInformation($"{nameof(BuildAsync)} - Start downloading data from url");

            try
            {
                ruleData = await this.LoadFromUrlAsync(this._dataFileUrl, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, $"{nameof(BuildAsync)} - Failure on download");

                return false;
            }

            if (string.IsNullOrEmpty(ruleData))
            {
                return false;
            }

            var ruleParser = new TldRuleParser();
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
                throw new RuleLoadException($"Cannot load file from {url} StatusCode:{(int)response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}