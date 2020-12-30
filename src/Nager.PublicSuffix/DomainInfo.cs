using System.Linq;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// Domain Info
    /// </summary>
    public class DomainInfo
    {
        /// <summary>
        /// Domain Name without the TLD
        /// </summary>
        /// <example>microsoft, google</example>
        public string Domain { get; private set; }
        /// <summary>
        /// The TLD
        /// </summary>
        /// <example>com, net, de, co.uk</example>
        public string TLD { get; private set; }
        /// <summary>
        /// The sub domain
        /// </summary>
        /// <example>www, mail, </example>
        public string SubDomain { get; private set; }
        /// <summary>
        /// The Registrable Domain
        /// </summary>
        /// <example>microsoft.com, amazon.co.uk</example>
        public string RegistrableDomain { get; private set; }
        /// <summary>
        /// Fully qualified hostname (FQDN)
        /// </summary>
        public string Hostname { get; private set; }
        /// <summary>
        /// The matching public suffix rule
        /// </summary>
        public TldRule TLDRule { get; private set; }

        public DomainInfo()
        {
        }

        public DomainInfo(string domain, TldRule tldRule)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return;
            }

            if (tldRule == null)
            {
                return;
            }

            var domainParts = domain.Split('.').Reverse().ToList();
            var ruleParts = tldRule.Name.Split('.').Skip(tldRule.Type == TldRuleType.WildcardException ? 1 : 0).Reverse().ToList();
            var tld = string.Join(".", domainParts.Take(ruleParts.Count).Reverse());
            var registrableDomain = string.Join(".", domainParts.Take(ruleParts.Count + 1).Reverse());

            if (domain.Equals(tld))
            {
                return;
            }

            this.TLDRule = tldRule;
            this.Hostname = domain;
            this.TLD = tld;
            this.RegistrableDomain = registrableDomain;

            this.Domain = domainParts.Skip(ruleParts.Count).FirstOrDefault();
            var subDomain = string.Join(".", domainParts.Skip(ruleParts.Count + 1).Reverse());
            this.SubDomain = string.IsNullOrEmpty(subDomain) ? null : subDomain;
        }
    }
}
