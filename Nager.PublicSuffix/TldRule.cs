using System;

namespace Nager.PublicSuffix
{
    public class TldRule
    {
        public string Name { get; set; }
        public TldRuleType Type { get; set; }

        public TldRule(string ruleData)
        {
            if (string.IsNullOrEmpty(ruleData))
            {
                throw new ArgumentException("RuleData is emtpy");
            }

            if (ruleData.StartsWith("*", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ruleData.Length < 3 || ruleData[1] != '.')
                {
                    throw new FormatException("Wildcard syntax not correct");
                }

                this.Type = TldRuleType.Wildcard;
                this.Name = ruleData.Substring(2).ToLower();
                return;
            }
            else if (ruleData.StartsWith("!", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Type = TldRuleType.WildcardException;
                this.Name = ruleData.Substring(1).ToLower();
                return;
            }

            this.Type = TldRuleType.Normal;
            this.Name = ruleData.ToLower();
        }

        public override string ToString()
        {
            return $"TldRule Name:{this.Name} Type:{this.Type}";
        }
    }
}
