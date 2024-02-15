using Nager.PublicSuffix.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders
{
    /// <summary>
    /// Static RuleProvider
    /// </summary>
    public class StaticRuleProvider : BaseRuleProvider
    {
        /// <summary>
        /// Static RuleProvider
        /// </summary>
        /// <param name="domainDataStructure"></param>
        public StaticRuleProvider(DomainDataStructure domainDataStructure)
        {
            this._domainDataStructure = domainDataStructure;
        }

        /// <summary>
        /// Static RuleProvider
        /// </summary>
        /// <param name="rules"></param>
        public StaticRuleProvider(IEnumerable<TldRule> rules)
        {
            base.CreareDomainDataStructure(rules);
        }

        /// <inheritdoc/>
        public override Task<bool> BuildAsync(
            bool ignoreCache = false,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
    }
}
