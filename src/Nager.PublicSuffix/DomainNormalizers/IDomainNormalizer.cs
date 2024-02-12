using System.Collections.Generic;

namespace Nager.PublicSuffix.DomainNormalizers
{
    /// <summary>
    /// Interface DomainNormalizer
    /// </summary>
    public interface IDomainNormalizer
    {
        List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts(string domain, out string partlyNormalizedDomain);
    }
}
