using Nager.PublicSuffix.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders
{
    /// <summary>
    /// ITopLevelDomainRuleProvider
    /// </summary>
    public interface ITopLevelDomainRuleProvider
    {
        /// <summary>
        /// Loads the plain text data from a source and parse the public suffix rules
        /// </summary>
        /// <returns>Returns the TldRules</returns>
        Task<IEnumerable<TldRule>> BuildAsync();
    }
}
