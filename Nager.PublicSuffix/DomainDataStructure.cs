using System.Collections.Generic;

namespace Nager.PublicSuffix
{
    internal class DomainDataStructure
    {
        public string Domain;
        public TldRule TldRule;
        public Dictionary<string, DomainDataStructure> Nested;

        public DomainDataStructure(string domain)
        {
            this.Domain = domain;
            this.Nested = new Dictionary<string, DomainDataStructure>();
        }

        public DomainDataStructure(string domain, TldRule tldRule)
        {
            this.Domain = domain;
            this.TldRule = tldRule;
            this.Nested = new Dictionary<string, DomainDataStructure>();
        }
    }
}
