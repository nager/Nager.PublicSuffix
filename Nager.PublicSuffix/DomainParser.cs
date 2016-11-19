using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    public class DomainParser
    {
        private DomainDataStructure _domainDataStructure = new DomainDataStructure(".");

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
            var lines = data.Split(new char[] { '\n', '\r' });
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

                //LL: Detect whitespace in middle of line

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
            var structure = this._domainDataStructure;
            var domainPart = string.Empty;

            var parts = tldRule.Name.Split('.').Reverse().ToList();
            for (var i = 0; i < parts.Count; i++)
            {
                domainPart = parts[i];

                if (parts.Count - 1 > i)
                {
                    //Check if domain exists
                    if (!structure.Nested.ContainsKey(domainPart))
                    {
                        structure.Nested.Add(domainPart, new DomainDataStructure(domainPart));
                    }

                    structure = structure.Nested[domainPart];
                    continue;
                }

                //Check if domain exists
                if (structure.Nested.ContainsKey(domainPart))
                {
                    structure.Nested[domainPart].TldRule = tldRule;
                    continue;
                }

                structure.Nested.Add(domainPart, new DomainDataStructure(domainPart, tldRule));
            }
        }

        public DomainName Get(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            //TODO:besser implementieren IdnMapping wird auch aufgerufen wenn es nicht benötigt wird

            var idnMapping = new IdnMapping();
            var parts = domain
                .ToLowerInvariant().Split('.')
                .Select(x => x.StartsWith("xn--")?idnMapping.GetUnicode(x).Trim():x.Trim()) //punycode
                .Reverse().ToList();

            if (parts.Count == 0 || parts.Any(x => x.Equals("")))
            {
                return null;
            }

            var structure = this._domainDataStructure;

            foreach (var part in parts)
            {
                if (structure.Nested.ContainsKey(part))
                {
                    structure = structure.Nested[part];
                    continue;
                }
                else if (structure.Nested.ContainsKey("*"))
                {
                    structure = structure.Nested["*"];
                    continue;
                }
                else break;
            }

            if (structure.TldRule == null)
            {
                return null;
            }

            //Domain is TLD
            if (parts.Count == structure.TldRule.LabelCount)
            {
                return null;
            }

            var domainName = new DomainName(domain, structure.TldRule);
            return domainName;
        }
    }
}
