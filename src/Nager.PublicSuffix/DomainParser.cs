using Nager.PublicSuffix.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// A TLD Domain parser
    /// </summary>
    public class DomainParser
    {
        private DomainDataStructure _domainDataStructure;
        private IDomainNormalizer _domainNormalizer;
        private readonly TldRule _rootTldRule = new TldRule("*");

        /// <summary>
        /// Creates and Initializes a DomainParse.
        /// </summary>
        /// <param name="rules">The list of rules.</param>
        /// <param name="domainNormalizer">An <see cref="IDomainNormalizer"/>.</param>
        public DomainParser(IEnumerable<TldRule> rules, IDomainNormalizer domainNormalizer = null)
            : this(domainNormalizer)
        {
            if (rules == null)
            {
                throw new ArgumentNullException("rules");
            }

            this.AddRules(rules);
        }

        /// <summary>
        /// Creates and initializes a DomainParser.
        /// </summary>
        /// <param name="ruleProvider">A <see cref="TldRule"/> provider.</param>
        /// <param name="domainNormalizer">An <see cref="IDomainNormalizer"/>.</param>
        public DomainParser(ITldRuleProvider ruleProvider, IDomainNormalizer domainNormalizer = null)
            : this(domainNormalizer)
        {
            var rules = ruleProvider.BuildAsync().GetAwaiter().GetResult();
            this.AddRules(rules);
        }

        /// <summary>
        /// Creates a DomainParser based on an already initialzed tree.
        /// </summary>
        /// <param name="initializedDataStructure">An already initialized tree.</param>
        /// <param name="domainNormalizer">An <see cref="IDomainNormalizer"/>.</param>
        public DomainParser(DomainDataStructure initializedDataStructure, IDomainNormalizer domainNormalizer = null)
            : this(domainNormalizer)
        {
            this._domainDataStructure = initializedDataStructure;
        }

        private DomainParser(IDomainNormalizer domainNormalizer)
        {
            this._domainNormalizer = domainNormalizer ?? new UriNormalizer();
        }

        /// <summary>
        /// Tries to get a Domain from <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain to parse.</param>
        /// <returns><strong>null</strong> if <paramref name="domain"/> it's invalid.</returns>
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

        /// <summary>
        /// Tries to get a Domain from <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The domain to parse.</param>
        /// <returns><strong>null</strong> if <paramref name="domain"/> it's invalid.</returns>
        public DomainName Get(string domain)
        {
            var parts = this._domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(domain, out string partlyNormalizedDomain);
            return this.GetDomainFromParts(partlyNormalizedDomain, parts);
        }

        /// <summary>
        /// Return whether <paramref name="domain"/> is valid or not.
        /// </summary>
        /// <param name="domain">The domain to check.</param>
        /// <returns><strong>true</strong> if <paramref name="domain"/> it's valid.</returns>
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

        private void AddRules(IEnumerable<TldRule> tldRules)
        {
            this._domainDataStructure = this._domainDataStructure ?? new DomainDataStructure("*", this._rootTldRule);

            this._domainDataStructure.AddRules(tldRules);
        }

        private DomainName GetDomainFromParts(string domain, List<string> parts)
        {
            if (parts == null || parts.Count == 0 || parts.Any(x => x.Equals(string.Empty)))
            {
                throw new ParseException("Invalid domain part detected");
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
                parts.Reverse();
                var tld = string.Join(".", parts);

                if (winningRule.Type == TldRuleType.Wildcard)
                {
                    if (tld.EndsWith(winningRule.Name.Substring(1)))
                    {
                        throw new ParseException("Domain is a TLD according publicsuffix", winningRule);
                    }
                }
                else
                {
                    if (tld.Equals(winningRule.Name))
                    {
                        throw new ParseException("Domain is a TLD according publicsuffix", winningRule);
                    }
                }

                throw new ParseException($"Unknown domain {domain}");
            }

            return new DomainName(domain, winningRule);
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
