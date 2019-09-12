using Xunit;

namespace Nager.PublicSuffix.UnitTest {
    public class PublicSuffixTestsWithIdnMappingNormalization : PublicSuffixTest {
        public PublicSuffixTestsWithIdnMappingNormalization () {
            var domainParser = new DomainParser (new FileTldRuleProvider ("effective_tld_names.dat"), new IdnMappingNormalizer ());
            this._domainParser = domainParser;
        }
    }
}