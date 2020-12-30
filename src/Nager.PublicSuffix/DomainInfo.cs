using System.Linq;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// Domain Info
    /// </summary>
    public class DomainInfo
    {
        /// <summary>
        /// Domain Name without the TLD<para />
        /// e.g. microsoft, google
        /// </summary>
        public string Domain { get; private set; }
        /// <summary>
        /// The TLD<para />
        /// e.g. com, net, de, co.uk
        /// </summary>
        public string TLD { get; private set; }
        /// <summary>
        /// The Sub Domain<para />
        /// e.g. www, mail
        /// </summary>
        public string SubDomain { get; private set; }
        /// <summary>
        /// The Registrable Domain<para />
        /// e.g. microsoft.com, amazon.co.uk
        /// </summary>
        public string RegistrableDomain { get; private set; }
        /// <summary>
        /// Fully qualified hostname (FQDN)
        /// </summary>
        public string Hostname { get; private set; }
        /// <summary>
        /// The matching public suffix Rule
        /// </summary>
        public TldRule TLDRule { get; private set; }

        /// <summary>
        /// Domain Info
        /// </summary>
        public DomainInfo()
        {
        }

        /// <summary>
        /// Domain Info
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="tldRule"></param>
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
