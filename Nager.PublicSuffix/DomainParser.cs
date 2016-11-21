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
            var division = TldRuleDivision.Unknown;

            foreach (var line in lines)
            {
                //Ignore empty lines
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }


                //Ignore comments (and set Division)
                if (line.StartsWith("//"))
                {
                    //Detect Division
                    if (line.StartsWith("// ===BEGIN ICANN DOMAINS==="))
                    {
                        division = TldRuleDivision.ICANN;
                    }
                    else if (line.StartsWith("// ===END ICANN DOMAINS==="))
                    {
                        division = TldRuleDivision.Unknown;
                    }
                    else if (line.StartsWith("// ===BEGIN PRIVATE DOMAINS==="))
                    {
                        division = TldRuleDivision.Private;
                    }
                    else if (line.StartsWith("// ===END PRIVATE DOMAINS==="))
                    {
                        division = TldRuleDivision.Unknown;
                    }

                    continue;
                }

                var tldRule = new TldRule(line.Trim(), division);

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

        public List<string> GetDomainParts(string domain)
        {
            var parts = domain.ToLowerInvariant().Split('.');

            if (parts.Any(o=>o.StartsWith("xn--")))
            {
                var idnMapping = new IdnMapping();
                return parts.Select(o => idnMapping.GetUnicode(o).Trim()).Reverse().ToList();
            }

            return parts.Reverse().ToList();
        }

        public DomainName Get(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            var parts = this.GetDomainParts(domain);

            if (parts.Count == 0 || parts.Any(o => o.Equals("")))
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
                else
                {
                    break;
                }
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
