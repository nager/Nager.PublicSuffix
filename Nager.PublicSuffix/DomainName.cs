using System.Linq;

namespace Nager.PublicSuffix
{
    public class DomainName
    {
        public string Domain { get; set; }
        public string TLD { get; set; }
        public string SubDomain { get; set; }
        public string RegistrableDomain { get; set; }
        public string Hostname { get; set; }
        public TldRule TLDRule { get; set; }

        public DomainName()
        {
        }

        public DomainName(string domain, TldRule tldRule)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return;
            }

            if (tldRule == null)
            {
                return;
            }

            if (domain.Equals(tldRule.Name))
            {
                return;
            }

            var domainWithoutTld = domain.Substring(0, domain.Length - tldRule.Name.Length - 1);
            var parts = domainWithoutTld.Split('.');
            var registrableDomain = $"{parts.Last()}.{tldRule.Name}";

            this.Domain = parts.Last();
            this.TLD = tldRule.Name;
            this.RegistrableDomain = registrableDomain;
            this.Hostname = domain;
            this.TLDRule = tldRule;

            if (parts.Length > 1)
            {
                var subDomain = domain.Substring(0, domain.Length - registrableDomain.Length - 1);
                this.SubDomain = subDomain;
            }
        }
    }
}
