﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class TldRuleProviderTest
    {
        [TestMethod]
        public async Task WebTldRuleProviderTest()
        {
            var tldRuleProvider = new WebTldRuleProvider();
            var rules = await tldRuleProvider.BuildAsync();
            Assert.IsNotNull(rules);
        }

        [TestMethod]
        public async Task FileTldRuleProviderTest()
        {
            var tldRuleProvider = new FileTldRuleProvider("effective_tld_names.dat");
            var rules = await tldRuleProvider.BuildAsync();
            Assert.AreEqual(8818, rules.Count());
            Assert.IsNotNull(rules);
        }
    }
}
