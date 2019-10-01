using System;
using System.Collections.Generic;
using System.Linq;

namespace Nager.PublicSuffix
{
    public class UriNormalizer : IDomainNormalizer
    {
        public List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts(string domain, out string partlyNormalizedDomain)
        {
            partlyNormalizedDomain = null;

            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            //We use Uri methods to normalize host (So Punycode is converted to UTF-8)
            if (!domain.Contains("https://"))
            {
                domain = string.Concat("https://", domain);
            }

            if (!Uri.TryCreate(domain, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                throw new ParseException("Cannot parse domain to an uri");
            }

            partlyNormalizedDomain = uri.Host;
            var normalizedHost = uri.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode

            return normalizedHost
                .Split('.')
                .Reverse()
                .ToList();
        }
    }
}
