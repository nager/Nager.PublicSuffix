using System;
using System.Collections.Generic;

namespace Nager.PublicSuffix {
    public interface IDomainNormalizer {
        List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts (string url, out Uri partlyNormalizedDomain);
    }
}