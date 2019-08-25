using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nager.PublicSuffix.Exceptions;

namespace Nager.PublicSuffix {
    public class UriNormalizer : IDomainNormalizer {
        static readonly Regex urlChecker = new Regex (@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$");
        static readonly Regex htProtocolChecker = new Regex (@"^(http|https)://");
        public List<string> PartlyNormalizeDomainAndExtractFullyNormalizedParts (string domain, out string partlyNormalizedDomain) {
            partlyNormalizedDomain = null;

            if (string.IsNullOrEmpty (domain)) {
                return null;
            }

            if (!htProtocolChecker.IsMatch (domain))
                domain = $"http://{domain}";

            //We use Uri methods to normalize host (So Punycode is converted to UTF-8)
            if (!urlChecker.IsMatch (domain))
                throw new InvalidUrlException ("This url is broken or invalid");

            if (!Uri.TryCreate (domain, UriKind.RelativeOrAbsolute, out Uri uri)) {
                return null;
            }

            partlyNormalizedDomain = uri.Host;
            var normalizedHost = uri.GetComponents (UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode
            return normalizedHost
                .Split ('.')
                .Reverse ()
                .ToList ();
        }
    }
}