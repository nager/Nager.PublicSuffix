using System;
using System.Linq;

namespace Nager.PublicSuffix
{
    public class TldRule
    {
        public string Name { get; private set; }
        public bool IsException { get; private set; }
        public TldRuleType Type { get; private set; }
        public int LabelCount { get; private set; }

        //TODO:Wieder umstellen auf TldRuleType ist sauberer

        public TldRule(string ruleData)
        {
            if (string.IsNullOrEmpty(ruleData))
            {
                throw new ArgumentException("RuleData is emtpy");
            }

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
                this.Name = ruleData.Substring(1).ToLower();
                this.IsException = true;
                this.LabelCount = parts.Count - 1; //Left-most label is removed for Wildcard Exceptions
            }
            else
            {
                this.Name = ruleData.ToLower();
                this.IsException = false;
                this.LabelCount = parts.Count;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
