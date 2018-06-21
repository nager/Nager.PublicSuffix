using System;
using System.Collections.Generic;
using System.Linq;

namespace Nager.PublicSuffix
{
    public class DomainParser
    {
        private DomainDataStructure _domainDataStructure;
        private readonly ITldRuleProvider _ruleProvider;

        public DomainParser(IEnumerable<TldRule> rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }

            this.AddRules(rules);
        }

        public DomainParser(ITldRuleProvider ruleProvider)
        {
            this._ruleProvider = ruleProvider ?? throw new ArgumentNullException("ruleProvider");
            this.AddRules(ruleProvider.BuildAsync().Result);
        }

        public void AddRules(IEnumerable<TldRule> tldRules)
        {
            this._domainDataStructure = new DomainDataStructure("*", new TldRule("*"));

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

        public DomainName Get(Uri domain)
        {
            var normalizedDomain = domain.Host;
            var normalizedHost = domain.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode

            var parts = normalizedHost
                .Split('.')
                .Reverse()
                .ToList();

            return this.GetDomainFromParts(normalizedDomain, parts);
        }

        public DomainName Get(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            //We use Uri methods to normalize host (So Punycode is converted to UTF-8
            if (!domain.Contains("https://"))
            {
                domain = string.Concat("https://", domain);
            }

            Uri uri;
            if (!Uri.TryCreate(domain, UriKind.RelativeOrAbsolute, out uri))
            {
                return null;
            }

            var normalizedDomain = uri.Host;
            var normalizedHost = uri.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode

            var parts = normalizedHost
                .Split('.')
                .Reverse()
                .ToList();

            return this.GetDomainFromParts(normalizedDomain, parts);
        }

        private DomainName GetDomainFromParts(string domain, List<string> parts)
        {
            if (parts == null || parts.Count == 0 || parts.Any(x => x.Equals(string.Empty)))
            {
                return null;
            }

            var structure = this._domainDataStructure;
            var matches = new List<TldRule>();
            this.FindMatches(parts, structure, matches);

            //Sort so exceptions are first, then by biggest label count (with wildcards at bottom) 
            var sortedMatches = matches.OrderByDescending(x => x.Type == TldRuleType.WildcardException ? 1 : 0)
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

            var domainName = new DomainName(domain, winningRule);
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
