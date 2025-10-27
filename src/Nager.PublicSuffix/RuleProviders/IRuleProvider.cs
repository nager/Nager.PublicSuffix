using Nager.PublicSuffix.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.RuleProviders
{
    /// <summary>
    /// Rule Provider Interface
    /// </summary>
    public interface IRuleProvider
    {
        /// <summary>
        /// Loads and parses the public suffix rules from the data source.
        /// </summary>
        /// <param name="ignoreCache">
        /// If set to <c>true</c>, bypasses the cached data — only applicable when the provider supports caching.
        /// </param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns><c>true</c> if the rules were successfully built; otherwise, <c>false</c>.</returns>
        Task<bool> BuildAsync(
            bool ignoreCache = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the DomainDataStructure
        /// </summary>
        /// <returns></returns>
        DomainDataStructure? GetDomainDataStructure();
    }
}
