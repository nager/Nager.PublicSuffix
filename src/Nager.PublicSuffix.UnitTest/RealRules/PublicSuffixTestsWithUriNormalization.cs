using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.DomainNormalizers;
using Nager.PublicSuffix.RuleProviders;

namespace Nager.PublicSuffix.UnitTest.RealRules
{
    [TestClass]
    public class PublicSuffixTestsWithUriNormalization : PublicSuffixTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            var domainParser = new DomainParser(new FileTldRuleProvider("public_suffix_list.dat"), new UriDomainNormalizer());
            this._domainParser = domainParser;
        }
    }
}
