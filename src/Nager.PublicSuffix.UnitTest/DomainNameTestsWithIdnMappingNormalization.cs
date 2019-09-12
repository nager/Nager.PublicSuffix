using System.Collections.Generic;
using Xunit;

namespace Nager.PublicSuffix.UnitTest {

    public class DomainNameTestsWithIdnMappingNormalization : DomainNameTest {
        protected override DomainParser GetParserForRules (List<TldRule> rules) {
            return new DomainParser (rules, new IdnMappingNormalizer ());
        }
    }
}