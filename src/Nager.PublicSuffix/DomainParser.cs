using System;
using System.Collections.Generic;
using System.Linq;

namespace Nager.PublicSuffix
{
    public class DomainParser
    {
        private readonly ITldRuleProvider _ruleProvider;
        private DomainDataStructure _domainDataStructure;
        private IDomainNormalizer _domainNormalizer;
        private TldRule _rootTldRule = new TldRule("*");

        public DomainParser(IEnumerable<TldRule> rules, IDomainNormalizer domainNormalizer = null)
        {
            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }

            this.Initialize(rules, domainNormalizer);
        }

        public DomainParser(ITldRuleProvider ruleProvider, IDomainNormalizer domainNormalizer = null)
        {
            this._ruleProvider = ruleProvider ?? throw new ArgumentNullException("ruleProvider");

            var rules = ruleProvider.BuildAsync().GetAwaiter().GetResult();
            this.Initialize(rules, domainNormalizer);
        }

        private void Initialize(IEnumerable<TldRule> rules, IDomainNormalizer domainNormalizer)
        {
            this.AddRules(rules);
            this._domainNormalizer = domainNormalizer ?? new UriNormalizer();
        }

        private void AddRules(IEnumerable<TldRule> tldRules)
        {
            this._domainDataStructure = new DomainDataStructure("*", this._rootTldRule);

            foreach (var tldRule in tldRules)
            {
                this.AddRule(tldRule);
            }
        }

        private void AddRule(TldRule tldRule)
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
            var partlyNormalizedDomain = domain.Host;
            var normalizedHost = domain.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode

            var parts = normalizedHost
                .Split('.')
                .Reverse()
                .ToList();

            return this.GetDomainFromParts(partlyNormalizedDomain, parts);
        }

        public DomainName Get(string domain)
        {
            var parts = this._domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(domain, out string partlyNormalizedDomain);
            return this.GetDomainFromParts(partlyNormalizedDomain, parts);
        }

        public bool IsValidDomain(string domain)
        {
            var parts = this._domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(domain, out string partlyNormalizedDomain);
            var domainName = this.GetDomainFromParts(partlyNormalizedDomain, parts);
            if (domainName == null)
            {
                return false;
            }

            return !domainName.TLDRule.Equals(this._rootTldRule);
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

            if (structure.Nested.TryGetValue(part, out DomainDataStructure foundStructure))
            {
                this.FindMatches(parts.Skip(1), foundStructure, matches);
            }

            if (structure.Nested.TryGetValue("*", out foundStructure))
            {
                this.FindMatches(parts.Skip(1), foundStructure, matches);
            }
        }
    }
}
