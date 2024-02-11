using System;
using System.Collections.Generic;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// TldRuleParser
    /// </summary>
    public class TldRuleParser
    {
        private readonly char[] _lineBreak = new char[] { '\n', '\r' };

        /// <summary>
        /// ParseRules
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IEnumerable<TldRule> ParseRules(string data)
        {
            var lines = data.Split(this._lineBreak);
            return this.ParseRules(lines);
        }

        /// <summary>
        /// ParseRules
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public IEnumerable<TldRule> ParseRules(IEnumerable<string> lines)
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
                if (line.StartsWith("//", StringComparison.OrdinalIgnoreCase))
                {
                    //Detect Division
                    if (line.StartsWith("// ===BEGIN ICANN DOMAINS===", StringComparison.OrdinalIgnoreCase))
                    {
                        division = TldRuleDivision.ICANN;
                    }
                    else if (line.StartsWith("// ===END ICANN DOMAINS===", StringComparison.OrdinalIgnoreCase))
                    {
                        division = TldRuleDivision.Unknown;
                    }
                    else if (line.StartsWith("// ===BEGIN PRIVATE DOMAINS===", StringComparison.OrdinalIgnoreCase))
                    {
                        division = TldRuleDivision.Private;
                    }
                    else if (line.StartsWith("// ===END PRIVATE DOMAINS===", StringComparison.OrdinalIgnoreCase))
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
    }
}
