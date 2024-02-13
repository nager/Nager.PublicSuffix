using Nager.PublicSuffix.Extensions;
using Nager.PublicSuffix.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders
{
    /// <summary>
    /// StaticRuleProvider
    /// </summary>
    public class StaticRuleProvider : IRuleProvider
    {
        private readonly DomainDataStructure _domainDataStructure;

        /// <summary>
        /// StaticRuleProvider
        /// </summary>
        /// <param name="domainDataStructure"></param>
        public StaticRuleProvider(DomainDataStructure domainDataStructure)
        {
            this._domainDataStructure = domainDataStructure;
        }

        /// <summary>
        /// StaticRuleProvider
        /// </summary>
        /// <param name="tldRules"></param>
        public StaticRuleProvider(IEnumerable<TldRule> tldRules)
        {
            var domainDataStructure = new DomainDataStructure("*", new TldRule("*"));
            domainDataStructure.AddRules(tldRules);

            this._domainDataStructure = domainDataStructure;
        }

        /// <inheritdoc/>
        public Task<bool> BuildAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public DomainDataStructure GetDomainDataStructure()
        {
            return this._domainDataStructure;
        }
    }
}
