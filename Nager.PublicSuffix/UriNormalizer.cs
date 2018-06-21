using System;
using System.Collections.Generic;
using System.Linq;

namespace Nager.PublicSuffix
{
    public class UriNormalizer : IDomainNormalizer
    {
        public List<string> NormalizeDomainAndExtractParts(string domain, out string normalizedDomain)
        {
            normalizedDomain = null;

            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            //We use Uri methods to normalize host (So Punycode is converted to UTF-8 
            if (!domain.Contains("https://"))
            {
                domain = string.Concat("https://", domain);
            }

            if (!Uri.TryCreate(domain, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                return null;
            }

            normalizedDomain = uri.Host;
            var normalizedHost = uri.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode

            return normalizedHost
                .Split('.')
                .Reverse()
                .ToList();
        }
    }
}
