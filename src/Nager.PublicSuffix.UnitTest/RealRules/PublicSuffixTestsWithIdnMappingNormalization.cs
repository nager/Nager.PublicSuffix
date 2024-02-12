using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.DomainNormalizers;
using Nager.PublicSuffix.RuleProviders;

namespace Nager.PublicSuffix.UnitTest.RealRules
{
    [TestClass]
    public class PublicSuffixTestsWithIdnMappingNormalization : PublicSuffixTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            var domainParser = new DomainParser(new LocalFileRuleProvider("public_suffix_list.dat"), new IdnMappingDomainNormalizer());
            this._domainParser = domainParser;
        }
    }
}
