using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class CachedTldRuleProviderTest
    {
        [TestMethod]
        public async Task CheckCachedRules()
        {
            string tmpFile = Path.Combine(Path.GetTempPath(), "UnitTestPublicSuffixList.dat");
            if (File.Exists(tmpFile))
            {
                File.Delete(tmpFile);
            }

            ITldRuleProvider provider = new CachedTldRuleProvider(fileName: tmpFile);
            var rules = await provider.BuildAsync();
            Assert.IsTrue(File.Exists(tmpFile));
            Assert.IsNotNull(rules);
            var ruleList = rules.ToList();
            Assert.IsTrue(ruleList.Count > 100); //Expecting lots of rules

            //Spot checks (If test fails here, verify that rules still exist on:
            //https://publicsuffix.org/list/public_suffix_list.dat
            var spotChecks = new string[] { "com", "*.bd", "blogspot.com" };
            var lookup = ruleList.ToDictionary(x => x.Name, x => x.Name);
            Assert.IsTrue(spotChecks.All(x => lookup.ContainsKey(x)));

            //Verify cache
            var fileDateBefore = File.GetLastWriteTimeUtc(tmpFile);
            var cachedRules = await provider.BuildAsync();
            var fileDateAfter = File.GetLastWriteTimeUtc(tmpFile);
            Assert.AreEqual(ruleList.Count, cachedRules.Count());
            Assert.AreEqual(fileDateBefore, fileDateAfter);
            Assert.IsTrue(cachedRules.All(x => lookup.ContainsKey(x.Name)));

            //Cleanup
            if (File.Exists(tmpFile))
            {
                File.Delete(tmpFile);
            }
        }
    }
}
