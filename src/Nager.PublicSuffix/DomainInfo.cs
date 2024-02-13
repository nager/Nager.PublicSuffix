using Nager.PublicSuffix.Models;
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
        /// The Top Level Domain (TLD)<para />
        /// e.g. com, net, de, co.uk
        /// </summary>
        public string TopLevelDomain { get; private set; }

        /// <summary>
        /// The Subdomain<para />
        /// e.g. www, mail
        /// </summary>
        public string Subdomain { get; private set; }

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
        public TldRule TopLevelDomainRule { get; private set; }

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
        public DomainInfo(
            string domain,
            TldRule tldRule)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return;
            }

            if (tldRule == null)
            {
                return;
            }

            var domainParts = domain.Split('.').Reverse();
            var ruleParts = tldRule.Name.Split('.').Skip(tldRule.Type == TldRuleType.WildcardException ? 1 : 0).Count();

            var topLevelDomain = string.Join(".", domainParts.Take(ruleParts).Reverse());
            var registrableDomain = string.Join(".", domainParts.Take(ruleParts + 1).Reverse());

            if (domain.Equals(topLevelDomain))
            {
                return;
            }

            this.TopLevelDomainRule = tldRule;
            this.Hostname = domain;
            this.TopLevelDomain = topLevelDomain;
            this.RegistrableDomain = registrableDomain;

            this.Domain = domainParts.Skip(ruleParts).FirstOrDefault();

            var subdomain = string.Join(".", domainParts.Skip(ruleParts + 1).Reverse());
            this.Subdomain = string.IsNullOrEmpty(subdomain) ? null : subdomain;
        }
    }
}
