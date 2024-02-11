using System.Collections.Generic;

namespace Nager.PublicSuffix.DomainNormalizers
{
    public interface IDomainNormalizer
    {
        List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts(string domain, out string partlyNormalizedDomain);
    }
}
