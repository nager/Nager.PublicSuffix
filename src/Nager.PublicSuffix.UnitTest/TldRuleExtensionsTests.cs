using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.Extensions;
using Nager.PublicSuffix.Models;
using Nager.PublicSuffix.RuleParsers;

namespace Nager.PublicSuffix.UnitTest;

[TestClass]
public class TldRuleExtensionsTests
{
    [TestMethod]
    public void UnparseWithExceptionTest()
    {
        const string rulesInText = """
                                   foo.com
                                   !bar.com
                                   !foo.bar.com
                                   """;

        var (rules1, rules2) = ParseUnparseRules(rulesInText);

        CollectionAssert.AreEqual(rules1, rules2);
        Assert.AreEqual(TldRuleType.WildcardException, rules2[1].Type);
    }

    [TestMethod]
    public void UnparseWithWildCardTest()
    {
        const string rulesInText = """
                                   natal.br
                                   net.br
                                   niteroi.br
                                   *.nom.br
                                   not.br
                                   ntr.br
                                   odo.br
                                   ong.br
                                   org.br
                                   """;
        var (rules1, rules2) = ParseUnparseRules(rulesInText);

        CollectionAssert.AreEqual(rules1, rules2);
        Assert.AreEqual(TldRuleType.Wildcard, rules2[3].Type);
    }

    private static (TldRule[] rules1, TldRule[] rules2) ParseUnparseRules(string rulesText)
    {
        var ruleParser = new TldRuleParser();

        var rules1 = ruleParser.ParseRules(rulesText).ToArray();
        var rulesUnParsedText = rules1.UnparseRules();
        var rules2 = ruleParser.ParseRules(rulesUnParsedText).ToArray();

        return (rules1, rules2);
    }
}
