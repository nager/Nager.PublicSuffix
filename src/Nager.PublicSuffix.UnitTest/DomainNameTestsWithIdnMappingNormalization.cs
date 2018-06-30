using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class DomainNameTestsWithIdnMappingNormalization : DomainNameTest
    {
        protected override DomainParser GetParserForRules(List<TldRule> rules)
        {
            return new DomainParser(rules, new IdnMappingNormalizer());
        }
    }
}
