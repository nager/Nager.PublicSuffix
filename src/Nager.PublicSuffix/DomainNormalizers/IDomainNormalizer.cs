using System.Collections.Generic;

namespace Nager.PublicSuffix.DomainNormalizers
{
    /// <summary>
    /// Interface DomainNormalizer
    /// </summary>
    public interface IDomainNormalizer
    {
        /// <summary>
        /// Partly normalize domain and extract FullyNormalizedParts
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="partlyNormalizedDomain"></param>
        /// <returns></returns>
        List<string>? PartlyNormalizeDomainAndExtractFullyNormalizedParts(string domain, out string? partlyNormalizedDomain);
    }
}
