using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nager.PublicSuffix.Models;

namespace Nager.PublicSuffix.Extensions
{
    /// <summary>
    /// TldRule Extensions
    /// </summary>
    public static class TldRuleExtensions
    {
        /// <summary>
        /// Converts the collection of <see cref="TldRule"/> <paramref name="rules"/> to text.
        /// </summary>
        /// <param name="rules">The collection of <see cref="TldRule"/> rules</param>
        /// <returns></returns>
        public static string UnparseRules(this IEnumerable<TldRule> rules)
        {
            var rulesData = new StringBuilder();
            foreach (var division in rules.GroupBy(rule => rule.Division))
            {
                switch (division.Key)
                {
                    case TldRuleDivision.ICANN:
                        rulesData.Append("\n// ===BEGIN ICANN DOMAINS===\n");
                        break;
                    case TldRuleDivision.Private:
                        rulesData.Append("\n// ===BEGIN PRIVATE DOMAINS===\n");
                        break;
                }

                foreach (var rule in division)
                {
                    rulesData.Append("\n");

                    if (rule.Type == TldRuleType.WildcardException)
                    {
                        rulesData.Append("!");
                    }
                    rulesData.Append(rule.Name);
                }
            
                switch (division.Key)
                {
                    case TldRuleDivision.ICANN:
                        rulesData.Append("\n// ===END ICANN DOMAINS===\n");
                        break;
                    case TldRuleDivision.Private:
                        rulesData.Append("\n// ===END PRIVATE DOMAINS===\n");
                        break;
                }
            }

            return rulesData.ToString();
        }
    }}