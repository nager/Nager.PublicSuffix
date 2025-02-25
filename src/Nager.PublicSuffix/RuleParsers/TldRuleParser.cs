using Nager.PublicSuffix.Models;
using System;
using System.Collections.Generic;

namespace Nager.PublicSuffix.RuleParsers
{
    /// <summary>
    /// TldRuleParser
    /// </summary>
    public class TldRuleParser
    {
        private readonly char[] _newlineSeparators = ['\n', '\r'];
        private readonly TldRuleDivisionFilter _tldRuleDivisionFilter;

        /// <summary>
        /// TldRuleParser
        /// </summary>
        /// <param name="tldRuleDivisionFilter"></param>
        public TldRuleParser(TldRuleDivisionFilter tldRuleDivisionFilter)
        {
            this._tldRuleDivisionFilter = tldRuleDivisionFilter;
        }

        /// <summary>
        /// ParseRules
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IEnumerable<TldRule> ParseRules(string data)
        {
            var lines = data.Split(this._newlineSeparators);
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
            var activeDivision = TldRuleDivision.Unknown;

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
                        activeDivision = TldRuleDivision.ICANN;
                    }
                    else if (line.StartsWith("// ===END ICANN DOMAINS===", StringComparison.OrdinalIgnoreCase))
                    {
                        activeDivision = TldRuleDivision.Unknown;
                    }
                    else if (line.StartsWith("// ===BEGIN PRIVATE DOMAINS===", StringComparison.OrdinalIgnoreCase))
                    {
                        activeDivision = TldRuleDivision.Private;
                    }
                    else if (line.StartsWith("// ===END PRIVATE DOMAINS===", StringComparison.OrdinalIgnoreCase))
                    {
                        activeDivision = TldRuleDivision.Unknown;
                    }

                    continue;
                }

                if (activeDivision == TldRuleDivision.Private &&
                    this._tldRuleDivisionFilter == TldRuleDivisionFilter.ICANNOnly)
                {
                    continue;
                }

                if (activeDivision == TldRuleDivision.ICANN &&
                    this._tldRuleDivisionFilter == TldRuleDivisionFilter.PrivateOnly)
                {
                    continue;
                }

                var tldRule = new TldRule(line.Trim(), activeDivision);
                items.Add(tldRule);
            }

            return items;
        }
    }
}
