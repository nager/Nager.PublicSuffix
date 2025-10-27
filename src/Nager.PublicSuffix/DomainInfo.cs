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
        public string? Domain { get; private set; }

        /// <summary>
        /// The Top Level Domain (TLD)<para />
        /// e.g. com, net, de, co.uk
        /// </summary>
        public string? TopLevelDomain { get; private set; }

        /// <summary>
        /// The Subdomain<para />
        /// e.g. www, mail
        /// </summary>
        public string? Subdomain { get; private set; }

        /// <summary>
        /// The Registrable Domain<para />
        /// e.g. microsoft.com, amazon.co.uk, google.com
        /// </summary>
        public string? RegistrableDomain { get; private set; }

        /// <summary>
        /// Fully qualified domain name (FQDN)
        /// </summary>
        public string? FullyQualifiedDomainName { get; private set; }

        /// <summary>
        /// The matching Rule of the PublicSuffixList
        /// </summary>
        public TldRule? TopLevelDomainRule { get; private set; }

        /// <summary>
        /// Domain Info
        /// </summary>
        public DomainInfo()
        {
        }

        /// <summary>
        /// Domain Info
        /// </summary>
        /// <param name="tldRule"></param>
        public DomainInfo(
            TldRule tldRule)
        {
            this.TopLevelDomainRule = tldRule;
            this.TopLevelDomain = tldRule.Name;
        }

        /// <summary>
        /// Domain Info
        /// </summary>
        /// <param name="fullyQualifiedDomainName"></param>
        /// <param name="tldRule"></param>
        public DomainInfo(
            string fullyQualifiedDomainName,
            TldRule tldRule)
        {
            if (string.IsNullOrEmpty(fullyQualifiedDomainName))
            {
                return;
            }

            if (tldRule == null)
            {
                return;
            }

            var domainParts = fullyQualifiedDomainName.Split('.').Reverse();
            var ruleParts = tldRule.Name.Split('.').Skip(tldRule.Type == TldRuleType.WildcardException ? 1 : 0).Count();

            var topLevelDomain = string.Join(".", domainParts.Take(ruleParts).Reverse());
            var registrableDomain = string.Join(".", domainParts.Take(ruleParts + 1).Reverse());

            if (fullyQualifiedDomainName.Equals(topLevelDomain))
            {
                return;
            }

            this.TopLevelDomainRule = tldRule;
            this.FullyQualifiedDomainName = fullyQualifiedDomainName;
            this.TopLevelDomain = topLevelDomain;
            this.RegistrableDomain = registrableDomain;

            this.Domain = domainParts.Skip(ruleParts).FirstOrDefault();

            var subdomain = string.Join(".", domainParts.Skip(ruleParts + 1).Reverse());
            this.Subdomain = string.IsNullOrEmpty(subdomain) ? null : subdomain;
        }
    }
}
