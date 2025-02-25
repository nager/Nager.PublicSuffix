using Nager.PublicSuffix.DomainNormalizers;
using Nager.PublicSuffix.Exceptions;
using Nager.PublicSuffix.Models;
using Nager.PublicSuffix.RuleProviders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// Domain Parser
    /// </summary>
    public class DomainParser : IDomainParser
    {
        private readonly IRuleProvider _ruleProvider;
        private readonly IDomainNormalizer _domainNormalizer;

        /// <summary>
        /// Creates and initializes a DomainParser
        /// </summary>
        /// <param name="ruleProvider">A rule provider from interface <see cref="IRuleProvider"/>.</param>
        /// <param name="domainNormalizer">An <see cref="IDomainNormalizer"/>.</param>
        public DomainParser(
            IRuleProvider ruleProvider,
            IDomainNormalizer? domainNormalizer = default)
        {
            this._ruleProvider = ruleProvider;
            this._domainNormalizer = domainNormalizer ?? new UriDomainNormalizer();
        }

        /// <inheritdoc/>
        public DomainInfo? Parse(Uri fullyQualifiedDomainName)
        {
            var partlyNormalizedDomain = fullyQualifiedDomainName.Host;
            var normalizedHost = fullyQualifiedDomainName.GetComponents(UriComponents.NormalizedHost, UriFormat.UriEscaped); //Normalize punycode

            var parts = normalizedHost
                .Split('.')
                .Reverse()
                .ToList();

            return this.GetDomainFromParts(partlyNormalizedDomain, parts);
        }

        /// <inheritdoc/>
        public DomainInfo? Parse(string fullyQualifiedDomainName)
        {
            var parts = this._domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(fullyQualifiedDomainName, out string? partlyNormalizedDomain);
            if (parts == null)
            {
                return null;
            }

            return this.GetDomainFromParts(partlyNormalizedDomain, parts);
        }

        /// <inheritdoc/>
        public bool IsValidDomain(string fullyQualifiedDomainName)
        {
            if (string.IsNullOrEmpty(fullyQualifiedDomainName))
            {
                return false;
            }

            if (Uri.TryCreate(fullyQualifiedDomainName, UriKind.Absolute, out _))
            {
                return false;
            }

            if (!Uri.TryCreate($"http://{fullyQualifiedDomainName}", UriKind.Absolute, out var uri))
            {
                return false;
            }

            if (!uri.DnsSafeHost.Equals(fullyQualifiedDomainName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (fullyQualifiedDomainName[0] == '*')
            {
                return false;
            }

            try
            {
                var parts = this._domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(fullyQualifiedDomainName, out string? partlyNormalizedDomain);
                if (parts == null)
                {
                    return false;
                }

                var domainInfo = this.GetDomainFromParts(partlyNormalizedDomain, parts);
                if (domainInfo == null)
                {
                    return false;
                }

                if (domainInfo.TopLevelDomainRule == null)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(domainInfo.Domain))
                {
                    return false;
                }

                return true;
            }
            catch (ParseException)
            {
                return false;
            }
        }

        private DomainInfo? GetDomainFromParts(
            string? domain,
            List<string> parts)
        {
            if (domain == null)
            {
                throw new ParseException("Invalid domain detected");
            }

            if (parts == null || parts.Count == 0 || parts.Any(x => x.Equals(string.Empty)))
            {
                throw new ParseException("Invalid domain part detected");
            }

            var domainDataStructure = this._ruleProvider.GetDomainDataStructure();
            if (domainDataStructure == null || domainDataStructure.TldRule == null)
            {
                throw new NullReferenceException("DomainDataStructure is not available");
            }

            var matches = new List<TldRule>();
            this.FindMatches(parts, domainDataStructure, matches);

            // Sort so exceptions are first, then by biggest label count (with wildcards at bottom) 
            var sortedMatches = matches.OrderByDescending(x => x.Type == TldRuleType.WildcardException ? 1 : 0)
                .ThenByDescending(x => x.LabelCount)
                .ThenByDescending(x => x.Name);

            var winningRule = sortedMatches.FirstOrDefault();
            if (winningRule == null)
            {
                return null;
            }

            // Domain is TLD
            if (parts.Count == winningRule.LabelCount)
            {
                parts.Reverse();
                var tld = string.Join(".", parts);

                if (winningRule.Type == TldRuleType.Wildcard)
                {
                    if (winningRule == domainDataStructure.TldRule)
                    {
                        return null;
                    }

                    if (tld.EndsWith(winningRule.Name.Substring(1)))
                    {
                        return new DomainInfo(winningRule);
                    }
                }
                else
                {
                    if (tld.Equals(winningRule.Name))
                    {
                        return new DomainInfo(winningRule);
                    }
                }

                throw new ParseException($"Unknown domain {domain}");
            }

            if (winningRule == domainDataStructure.TldRule)
            {
                return null;
            }

            return new DomainInfo(domain, winningRule);
        }

        private void FindMatches(
            IEnumerable<string> parts,
            DomainDataStructure domainDataStructure,
            List<TldRule> matches)
        {
            if (domainDataStructure.TldRule != null)
            {
                matches.Add(domainDataStructure.TldRule);
            }

            var part = parts.FirstOrDefault();
            if (string.IsNullOrEmpty(part))
            {
                return;
            }

            if (domainDataStructure.Nested.TryGetValue(part, out DomainDataStructure? nestedDomainDataStructure))
            {
                this.FindMatches(parts.Skip(1), nestedDomainDataStructure, matches);
            }

            if (domainDataStructure.Nested.TryGetValue("*", out nestedDomainDataStructure))
            {
                this.FindMatches(parts.Skip(1), nestedDomainDataStructure, matches);
            }
        }
    }
}
