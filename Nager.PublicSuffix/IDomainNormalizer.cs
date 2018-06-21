using System.Collections.Generic;

namespace Nager.PublicSuffix
{
    public interface IDomainNormalizer
    {
        List<string> NormalizeDomainAndExtractParts(string domain, out string normalizedDomain);
    }
}
