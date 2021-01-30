using System;
using System.Linq;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// TldRule
    /// </summary>
    public class TldRule
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Type
        /// </summary>
        public TldRuleType Type { get; private set; }
        /// <summary>
        /// LabelCount
        /// </summary>
        public int LabelCount { get; private set; }
        /// <summary>
        /// Division
        /// </summary>
        public TldRuleDivision Division { get; private set; }

        /// <summary>
        /// TldRule
        /// </summary>
        /// <param name="ruleData"></param>
        /// <param name="division"></param>
        public TldRule(string ruleData, TldRuleDivision division = TldRuleDivision.Unknown)
        {
            if (string.IsNullOrEmpty(ruleData))
            {
                throw new ArgumentException("RuleData is empty");
            }

            this.Division = division;

            var parts = ruleData.Split('.').Select(x => x.Trim()).ToList();
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    throw new FormatException("Rule contains empty part");
                }

                if (part.Contains("*") && part != "*")
                {
                    throw new FormatException("Wildcard syntax not correct");
                }
            }

            if (ruleData.StartsWith("!", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Type = TldRuleType.WildcardException;
                this.Name = ruleData.Substring(1).ToLower();
                this.LabelCount = parts.Count - 1; //Left-most label is removed for Wildcard Exceptions
            }
            else if (ruleData.Contains("*"))
            {
                this.Type = TldRuleType.Wildcard;
                this.Name = ruleData.ToLower();
                this.LabelCount = parts.Count;
            }
            else
            {
                this.Type = TldRuleType.Normal;
                this.Name = ruleData.ToLower();
                this.LabelCount = parts.Count;
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
