using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nager.PublicSuffix.Exceptions;

namespace Nager.PublicSuffix {
    public class UriNormalizer : IDomainNormalizer {

        public List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts (string url, out Uri partlyNormalizedDomain) {
            partlyNormalizedDomain = UrlFixer.Repair (url);

            if (string.IsNullOrEmpty (url) || partlyNormalizedDomain == null)
                return default;

            var normalizedHost = partlyNormalizedDomain.GetComponents (UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode
            return normalizedHost
                .Split ('.')
                .Reverse ()
                .ToList ();
        }

    }
}