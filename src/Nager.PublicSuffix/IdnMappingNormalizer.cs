using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nager.PublicSuffix.Exceptions;

namespace Nager.PublicSuffix {
    public class IdnMappingNormalizer : IDomainNormalizer {
        private readonly IdnMapping _idnMapping = new IdnMapping ();

        public List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts (string url, out Uri partlyNormalizedDomain) {
            partlyNormalizedDomain = UrlFixer.Repair (url);

            if (string.IsNullOrEmpty (url) || partlyNormalizedDomain == null)
                return default;

            string punycodeConvertedDomain = partlyNormalizedDomain.ToString ();
            if (partlyNormalizedDomain.OriginalString.Contains ("xn--")) {
                punycodeConvertedDomain = this._idnMapping.GetUnicode (partlyNormalizedDomain.ToString ());
            }

            return punycodeConvertedDomain
                .Split ('.')
                .Reverse ()
                .ToList ();
        }
    }
}