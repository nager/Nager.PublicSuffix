using System;
using System.Linq;

namespace Nager.PublicSuffix {
    public class DomainName {
        public string Domain { get; private set; }
        public string TLD { get; private set; }
        public string SubDomain { get; private set; }
        public string RegistrableDomain { get; private set; }
        public string Hostname { get; private set; }
        public TldRule TLDRule { get; private set; }
        public string OriginalUrl { get; private set; }
        public string UnresolvedUrl { get; set; }

        public DomainName () { }

        public DomainName (Uri uri, TldRule tldRule) : this (uri, tldRule, string.Empty) { }
        public DomainName (Uri uri, TldRule tldRule, string unresolvedUrl) {
            if (uri != null && string.IsNullOrEmpty (uri.Host)) {
                return;
            }

            if (tldRule == null) {
                return;
            }

            var domainParts = uri.Host.Split ('.').Reverse ().ToList ();
            var ruleParts = tldRule.Name.Split ('.').Skip (tldRule.Type == TldRuleType.WildcardException ? 1 : 0).Reverse ().ToList ();
            var tld = string.Join (".", domainParts.Take (ruleParts.Count).Reverse ());
            var registrableDomain = string.Join (".", domainParts.Take (ruleParts.Count + 1).Reverse ());

            if (uri.Host.Equals (tld)) {
                return;
            }

            this.TLDRule = tldRule;
            this.Hostname = uri.Host;
            this.TLD = tld;
            this.RegistrableDomain = registrableDomain;

            this.Domain = domainParts.Skip (ruleParts.Count).FirstOrDefault ();
            var subDomain = string.Join (".", domainParts.Skip (ruleParts.Count + 1).Reverse ());
            this.SubDomain = string.IsNullOrEmpty (subDomain) ? null : subDomain;

            this.OriginalUrl = uri?.OriginalString;
            this.UnresolvedUrl = unresolvedUrl;
        }
    }
}