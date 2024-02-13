using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.RuleProviders;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class RuleProviderTest
    {
        [TestMethod]
        public async Task WebTldRuleProviderTest()
        {
            var builder = new ConfigurationBuilder();
            using var httpClient = new HttpClient();

            var configuration = builder.Build();

            var webRuleProvider = new WebRuleProvider(configuration, httpClient);
            var domainDataStructure = await webRuleProvider.BuildAsync();
            Assert.IsNotNull(domainDataStructure);
        }

        [TestMethod]
        public async Task FileTldRuleProviderTest()
        {
            var localFileRuleProvider = new LocalFileRuleProvider("public_suffix_list.dat");

            var buildSuccessful = await localFileRuleProvider.BuildAsync();
            Assert.IsTrue(buildSuccessful);

            var domainDataStructure = localFileRuleProvider.GetDomainDataStructure();

            Assert.AreEqual(1460, domainDataStructure.Nested.Count);
        }
    }
}
