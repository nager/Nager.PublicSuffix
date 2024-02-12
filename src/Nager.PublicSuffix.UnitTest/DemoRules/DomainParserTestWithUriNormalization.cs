using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.DomainNormalizers;
using Nager.PublicSuffix.Models;
using Nager.PublicSuffix.RuleProviders;
using System.Collections.Generic;

namespace Nager.PublicSuffix.UnitTest.DemoRules
{
    [TestClass]
    public class DomainParserTestWithUriNormalization : DomainParserTest
    {
        protected override IDomainParser GetDomainParser(List<TldRule> rules)
        {
            var ruleProvider = new StaticRuleProvider(rules);
            var domainNormalizer = new UriDomainNormalizer();

            return new DomainParser(ruleProvider, domainNormalizer);
        }
    }
}
