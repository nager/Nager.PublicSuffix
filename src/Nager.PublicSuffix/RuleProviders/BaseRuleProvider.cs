using Nager.PublicSuffix.Extensions;
using Nager.PublicSuffix.Models;
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
        protected DomainDataStructure _domainDataStructure;

        /// <inheritdoc/>
        public abstract Task<bool> BuildAsync(
            bool ignoreCache = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a DomainDataStructure
        /// </summary>
        /// <param name="rules"></param>
        protected void CreareDomainDataStructure(IEnumerable<TldRule> rules)
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
    }
}
