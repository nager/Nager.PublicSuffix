using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nager.PublicSuffix
{
    /// <summary>
    /// StringTldRuleProvider
    /// </summary>
    public class StringTldRuleProvider : ITldRuleProvider
    {
        private readonly string _rules;
    
        /// <summary>
        /// StringTldRuleProvider
        /// </summary>
        /// <param name="rules"></param>
        public StringTldRuleProvider(string rules)
        {
            _rules = rules;
        }

        ///<inheritdoc/>
        public Task<IEnumerable<TldRule>> BuildAsync()
        {
            var ruleParser = new TldRuleParser();
            var rules = ruleParser.ParseRules(_rules);
            return Task.FromResult(rules);
        }
    }
}