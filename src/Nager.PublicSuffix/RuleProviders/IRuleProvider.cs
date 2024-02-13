using Nager.PublicSuffix.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders
{
    /// <summary>
    /// Interface RuleProvider
    /// </summary>
    public interface IRuleProvider
    {
        /// <summary>
        /// Loads the plain text data from a source and parse the public suffix rules
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns the TldRules</returns>
        Task<bool> BuildAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the DomainDataStructure
        /// </summary>
        /// <returns></returns>
        DomainDataStructure? GetDomainDataStructure();
    }
}
