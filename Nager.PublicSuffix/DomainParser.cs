using System;
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

        public DomainName Get(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            //We use Uri methods to normalize host (So Punycode is converted to UTF-8
            Uri uri;
            if (!Uri.TryCreate(string.Concat("https://", domain), UriKind.RelativeOrAbsolute, out uri))
            {
                return null;
            }

            var normalizedDomain = uri.Host;
            var normalizedHost = uri.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode

            var parts = normalizedHost
                .Split('.')
                .Reverse()
                .ToList();

            if (parts.Count == 0 || parts.Any(x => x.Equals(string.Empty)))
            {
                return null;
            }

            var structure = this._domainDataStructure;
            var matches = new List<TldRule>();
            this.FindMatches(parts, structure, matches);

            //Sort so exceptions are first, then by biggest label count (with wildcards at bottom) 
            var sortedMatches = matches.OrderByDescending(x => x.Type == TldRuleType.WildcardException?1:0)
                .ThenByDescending(x => x.LabelCount)
                .ThenByDescending(x => x.Name);

            var winningRule = sortedMatches.FirstOrDefault();
            if (winningRule == null)
            {
                winningRule = new TldRule("*");
            }

            //Domain is TLD
            if (parts.Count == winningRule.LabelCount)
            {
                return null;
            }

            var domainName = new DomainName(normalizedDomain, winningRule);
            return domainName;
        }

        private void FindMatches(IEnumerable<string> parts, DomainDataStructure structure, List<TldRule> matches)
        {
            if (structure.TldRule != null)
            {
                matches.Add(structure.TldRule);
            }

            var part = parts.FirstOrDefault();
            if (string.IsNullOrEmpty(part))
            {
                return;
            }

            DomainDataStructure foundStructure;
            if (structure.Nested.TryGetValue(part, out foundStructure))
            {
                FindMatches(parts.Skip(1), foundStructure, matches);
            }

            if (structure.Nested.TryGetValue("*", out foundStructure))
            {
                FindMatches(parts.Skip(1), foundStructure, matches);
            }
        }
    }
}
