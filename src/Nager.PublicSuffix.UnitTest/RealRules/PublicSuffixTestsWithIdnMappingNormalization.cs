using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.DomainNormalizers;
using Nager.PublicSuffix.RuleProviders;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.UnitTest.RealRules
{
    [TestClass]
    public class PublicSuffixTestsWithIdnMappingNormalization : PublicSuffixTest
    {
        [TestInitialize()]
        public async Task Initialize()
        {
            var ruleProvider = new LocalFileRuleProvider("public_suffix_list.dat");
            await ruleProvider.BuildAsync();

            var domainParser = new DomainParser(ruleProvider, new IdnMappingDomainNormalizer());

            this._domainParser = domainParser;
        }
    }
}
