using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.DomainNormalizers;
using Nager.PublicSuffix.Models;
using System.Collections.Generic;

namespace Nager.PublicSuffix.UnitTest.DemoRules
{
    [TestClass]
    public class DomainParserTestWithIdnMappingNormalization : DomainParserTest
    {
        protected override IDomainParser GetDomainParser(List<TldRule> rules)
        {
            var domainNormalizer = new IdnMappingDomainNormalizer();
            return new DomainParser(rules, domainNormalizer);
        }
    }
}
