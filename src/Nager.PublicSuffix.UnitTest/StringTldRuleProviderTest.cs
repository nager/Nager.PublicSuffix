using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nager.PublicSuffix.UnitTest;

[TestClass]
public class StringTldRuleProviderTest
{
    [TestMethod]
    public async Task SimpleTldStringListTest()
    {
        const string rulesInText = @"
                foo.com
                !bar.com
                !foo.bar.com";

        var tldRuleProvider = new StringTldRuleProvider(rulesInText);
        var rules = await tldRuleProvider.BuildAsync();
        Assert.IsNotNull(rules);
        Assert.AreEqual(3, rules.Count());
    }
}