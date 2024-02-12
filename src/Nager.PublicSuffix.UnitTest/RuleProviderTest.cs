using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.RuleProviders;
using System.Linq;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class RuleProviderTest
    {
        [TestMethod]
        public async Task WebTldRuleProviderTest()
        {
            var webRuleProvider = new WebRuleProvider();
            var rules = await webRuleProvider.BuildAsync();
            Assert.IsNotNull(rules);
        }

        [TestMethod]
        public async Task FileTldRuleProviderTest()
        {
            var localFileRuleProvider = new LocalFileRuleProvider("public_suffix_list.dat");
            var rules = await localFileRuleProvider.BuildAsync();
            Assert.AreEqual(9609, rules.Count());
            Assert.IsNotNull(rules);
        }
    }
}
