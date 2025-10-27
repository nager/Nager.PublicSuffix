using Microsoft.Extensions.Configuration;
using Nager.PublicSuffix.Extensions;
using Nager.PublicSuffix.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders
{
    /// <summary>
    /// BaseRuleProvider
    /// </summary>
    public abstract class BaseRuleProvider : IRuleProvider
    {
        /// <summary>
        /// Parsed public suffix list rules
        /// </summary>
        protected DomainDataStructure? _domainDataStructure;

        /// <inheritdoc/>
        public abstract Task<bool> BuildAsync(
            bool ignoreCache = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a DomainDataStructure
        /// </summary>
        /// <param name="rules"></param>
        protected void CreateDomainDataStructure(IEnumerable<TldRule> rules)
        {
            var domainDataStructure = new DomainDataStructure("*", new TldRule("*"));
            domainDataStructure.AddRules(rules);

            this._domainDataStructure = domainDataStructure;
        }

        /// <inheritdoc/>
        public DomainDataStructure? GetDomainDataStructure()
        {
            return this._domainDataStructure;
        }

        /// <summary>
        /// Determines the data source URL for the public suffix list.
        /// </summary>
        /// <param name="configuration">Optional configuration source to override the default URL.</param>
        /// <returns>
        /// Returns the configured URL if <c>Nager:PublicSuffix:DataUrl</c> is set; 
        /// otherwise returns the default URL "https://publicsuffix.org/list/public_suffix_list.dat".
        /// </returns>
        protected string GetDataUrl(
            IConfiguration? configuration)
        {
            var url = "https://publicsuffix.org/list/public_suffix_list.dat";

            if (configuration is null)
            {
                return url;
            }

            var tempUrl = configuration["Nager:PublicSuffix:DataUrl"];
            if (string.IsNullOrEmpty(tempUrl))
            {
                return url;
            }

            return tempUrl!;
        }

        /// <summary>
        /// Determines the applicable TLD rule division filter from the configuration.
        /// </summary>
        /// <param name="configuration">Optional configuration source to override the default filter.</param>
        /// <param name="tldRuleDivisionFilter">The default filter used when no configuration value is provided.</param>
        /// <returns>
        /// Returns the configured filter if <c>Nager:PublicSuffix:TldRuleDivisionFilter</c> is set and valid; 
        /// otherwise returns the provided default filter or <see cref="TldRuleDivisionFilter.All"/> if parsing fails.
        /// </returns>
        protected TldRuleDivisionFilter GetTldRuleDivisionFilter(
            IConfiguration? configuration,
            TldRuleDivisionFilter tldRuleDivisionFilter)
        {
            if (configuration is null)
            {
                return tldRuleDivisionFilter;
            }

            var tempTldRuleDivisionFilter = configuration["Nager:PublicSuffix:TldRuleDivisionFilter"];
            if (!string.IsNullOrEmpty(tempTldRuleDivisionFilter))
            {
                if (!Enum.TryParse(tempTldRuleDivisionFilter, out tldRuleDivisionFilter))
                {
                    return TldRuleDivisionFilter.All;
                }
            }

            return tldRuleDivisionFilter;
        }
    }
}
