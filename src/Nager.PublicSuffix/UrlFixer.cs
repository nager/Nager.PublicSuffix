using System;
using System.Text.RegularExpressions;
using Nager.PublicSuffix.Exceptions;

namespace Nager.PublicSuffix {
    public static class UrlFixer {
        static readonly Regex urlChecker = new Regex (@"^(?:(?:(?:https?):)?\/\/)?(?:\S+(?::\S*)?@)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(\d{1,10})|(0[xX][0-9a-fA-F]{1,12})|(?:(?:[a-z0-9\u00a1-\uffff][a-z0-9\u00a1-\uffff_-]{0,62})?[a-z0-9\u00a1-\uffff]\.)+(?:([a-z\u00a1-\uffff]{2,}\.?)|(xn--[0-9a-z]{6})))(?::\d{2,5})?(?:[/?#]\S*)?$");
        static readonly Regex htProtocolChecker = new Regex (@"https?:\/\/");
        static readonly Regex urlSchemeAuthority = new Regex (@"^((https?:\/\/)?([.a-zA-Z0-9\u00a1-\uffff_-]+)\/?)(.*)$");

        //Scheme-Based Normalization
        public static Uri Repair (string url) {
            if (string.IsNullOrEmpty (url))
                return null;
            url = LowerCaseUrl (url);

            if (!urlChecker.IsMatch (url))
                return null;
            //throw new InvalidUrlException ("This url is broken or invalid");

            if (!htProtocolChecker.IsMatch (url))
                url = $"http://{url}";

            if (!Uri.TryCreate (url, UriKind.RelativeOrAbsolute, out Uri partlyNormalizedDomain))
                url = (!string.IsNullOrEmpty (partlyNormalizedDomain.Scheme)) ? partlyNormalizedDomain.ToString () : $"http://{partlyNormalizedDomain.OriginalString}";

            return partlyNormalizedDomain;
        }

        public static string LowerCaseUrl (string url) {
            if (!string.IsNullOrEmpty (url) && urlSchemeAuthority.IsMatch (url)) {
                var groups = urlSchemeAuthority.Match (url).Groups;
                if (groups.Count > 1)
                    return groups[1].Value.ToLower () + groups[groups.Count - 1].Value;
            }
            return url;
        }

        public static bool IsValidUrl (string url) {
            return urlChecker.IsMatch (url) && htProtocolChecker.IsMatch (url);
        }
    }
}