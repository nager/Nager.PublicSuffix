using System.Collections.Generic;
using System.Linq;
using Nager.PublicSuffix.Models;

namespace Nager.PublicSuffix.Extensions
{
    public static class TldRuleExtensions
    {
        /// <summary>
        /// Converts the collection of <see cref="TldRule"/> <paramref name="rules"/> to text.
        /// </summary>
        /// <param name="rules">The collection of <see cref="TldRule"/> rules</param>
        /// <returns></returns>
        public static string UnParseRules(this IEnumerable<TldRule> rules)
        {
            var rulesData = "";
            foreach (var division in rules.GroupBy(x=>x.Division))
            {
                switch (division.Key)
                {
                    case TldRuleDivision.ICANN:
                        rulesData += "\n// ===BEGIN ICANN DOMAINS===\n";
                        break;
                    case TldRuleDivision.Private:
                        rulesData += "\n// ===BEGIN PRIVATE DOMAINS===\n";
                        break;
                }

                foreach (var rule in division)
                {
                    rulesData += "\n";
                    if (rule.Type == TldRuleType.WildcardException)
                    {
                        rulesData += "!";
                    }
                    rulesData += rule.Name;
                }
            
                switch (division.Key)
                {
                    case TldRuleDivision.ICANN:
                        rulesData += "\n// ===END ICANN DOMAINS===\n";
                        break;
                    case TldRuleDivision.Private:
                        rulesData += "\n// ===END PRIVATE DOMAINS===\n";
                        break;
                }
            }

            return rulesData;
        }
    }}