using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nager.PublicSuffix.DomainNormalizers
{
    /// <summary>
    /// IdnMapping DomainNormalizer
    /// </summary>
    public class IdnMappingDomainNormalizer : IDomainNormalizer
    {
        private readonly IdnMapping _idnMapping = new IdnMapping();

        /// <inheritdoc/>
        public List<string>? PartlyNormalizeDomainAndExtractFullyNormalizedParts(string domain, out string? partlyNormalizedDomain)
        {
            partlyNormalizedDomain = null;

            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            partlyNormalizedDomain = domain.ToLowerInvariant();

            string punycodeConvertedDomain = partlyNormalizedDomain;
            if (partlyNormalizedDomain.Contains("xn--"))
            {
                punycodeConvertedDomain = this._idnMapping.GetUnicode(partlyNormalizedDomain);
            }

            return punycodeConvertedDomain
                .Split('.')
                .Reverse()
                .ToList();
        }
    }
}
