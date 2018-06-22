using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class PublicSuffixTestsWithIdnMappingNormalization : PublicSuffixTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"), new IdnMappingNormalizer());
            this._domainParser = domainParser;
        }
    }
}
