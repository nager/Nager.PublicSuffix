using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nager.PublicSuffix.UnitTest;

[TestClass]
public class TldRuleProviderCombinatorTest
{
    [TestMethod]
    public void DistinctByTest()
    {
        var r1 = new TldRule("foo");
        var r3 = new TldRule("bar");
        var r4 = new TldRule("foo.bar");

        var seq = new[] { r1, r1, r3, r4, r3 };
        var seqDistinct = seq.DistinctBy(x => x.Name).ToArray();

        Assert.IsTrue(seqDistinct.Count() == 3);
    }

    [TestMethod]
    public async Task CombinatorIdempotenceTest()
    {
        const string rulesInText = @"
                !bar.com
                !foo.bar.com
                foo.com";

        var provider1 = new StringTldRuleProvider(rulesInText);
        var provider2 = new StringTldRuleProvider(rulesInText);
        var provider = new TldRuleProviderCombinator(provider1, provider2);

        var rules1 = (await provider1.BuildAsync()).ToArray();
        var rules2 = (await provider2.BuildAsync()).ToArray();
        var rules = (await provider.BuildAsync()).ToArray();

        Assert.IsNotNull(rules);
        Assert.IsTrue(rules.SequenceEqual(rules1));
        Assert.IsTrue(rules.SequenceEqual(rules2));
    }
    [TestMethod]
    public async Task CombinatorSetExtensionTest()
    {
        const string rulesInText1 = @"
                !bar.com
                foo.com";
        const string rulesInText2 = @"
                !foo.bar.com
                foo.com";

        var provider1 = new StringTldRuleProvider(rulesInText1);
        var provider2 = new StringTldRuleProvider(rulesInText2);
        var provider = new TldRuleProviderCombinator(provider1, provider2);
        
        var rules = (await provider.BuildAsync()).ToArray();

        Assert.IsNotNull(rules);
        Assert.IsTrue(rules.Count() == 3);
        Assert.IsTrue(rules.Count(x => x.Name == "bar.com")==1);
        Assert.IsTrue(rules.Count(x => x.Name == "foo.com")==1);
        Assert.IsTrue(rules.Count(x => x.Name == "foo.bar.com")==1);
    }
}