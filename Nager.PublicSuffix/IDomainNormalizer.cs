using System.Collections.Generic;

namespace Nager.PublicSuffix
{
    public interface IDomainNormalizer
    {
        List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts(string domain, out string partlyNormalizedDomain);
    }
}
