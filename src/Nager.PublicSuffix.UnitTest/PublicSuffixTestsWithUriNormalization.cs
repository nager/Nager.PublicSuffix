using Xunit;

namespace Nager.PublicSuffix.UnitTest {

    public class PublicSuffixTestsWithUriNormalization : PublicSuffixTest {

        public PublicSuffixTestsWithUriNormalization () {
            var domainParser = new DomainParser (new FileTldRuleProvider ("effective_tld_names.dat"), new UriNormalizer ());
            this._domainParser = domainParser;
        }
    }
}