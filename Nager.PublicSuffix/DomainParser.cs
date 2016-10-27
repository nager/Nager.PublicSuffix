using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public class DomainParser
    {
        private DomainDataStructure _domainDataStructure = new DomainDataStructure(".");
        private List<TldRule> _wildcardExceptions = new List<TldRule>();

        public async Task<string> LoadDataAsync(string url = "https://publicsuffix.org/list/effective_tld_names.dat")
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public List<TldRule> ParseRules(string data)
        {
            var lines = data.Split('\n');
            return this.ParseRules(lines);
        }

        public List<TldRule> ParseRules(string[] lines)
        {
            var items = new List<TldRule>();

            foreach (var line in lines)
            {
                //Ignore empty lines
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                //Ignore comments
                if (line.StartsWith("//"))
                {
                    continue;
                }

                var tldRule = new TldRule(line.Trim());

                items.Add(tldRule);
            }

            return items;
        }

        public void AddRules(List<TldRule> tldRules)
        {
            foreach (var tldRule in tldRules)
            {
                this.AddRule(tldRule);
            }
        }

        public void AddRule(TldRule tldRule)
        {
            if (tldRule.Type == TldRuleType.WildcardException)
            {
                this._wildcardExceptions.Add(tldRule);
                return;
            }

            var structure = this._domainDataStructure;
            var domain = string.Empty;

            var parts = tldRule.Name.Split('.').Reverse().ToList();
            for (var i = 0; i < parts.Count; i++)
            {
                domain = parts[i];

                if (parts.Count - 1 > i)
                {
                    //Check if domain exists
                    if (!structure.Nested.ContainsKey(domain))
                    {
                        structure.Nested.Add(domain, new DomainDataStructure(domain));
                    }

                    structure = structure.Nested[domain];
                    continue;
                }

                //Check if domain exists
                if (structure.Nested.ContainsKey(domain))
                {
                    structure.Nested[domain].TldRule = tldRule;
                    continue;
                }

                structure.Nested.Add(domain, new DomainDataStructure(domain, tldRule));
            }
        }

        public DomainName Get(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            var parts = domain.ToLower().Split('.').Reverse().ToList();
            if (parts.Count == 0)
            {
                return null;
            }

            var structure = this._domainDataStructure;

            foreach (var part in parts)
            {
                if (part.Equals(""))
                {
                    return null;
                }

                if (structure.Nested.ContainsKey("*"))
                {
                    structure = structure.Nested[part];
                    continue;
                }

                if (structure.Nested.ContainsKey(part))
                {
                    structure = structure.Nested[part];
                    continue;
                }
            }

            if (structure.TldRule == null)
            {
                return null;
            }

            //Domain is TLD
            if (domain == structure.TldRule.Name)
            {
                return null;
            }

            var domainName = new DomainName(domain, structure.TldRule);
            return domainName;
        }
    }
}
