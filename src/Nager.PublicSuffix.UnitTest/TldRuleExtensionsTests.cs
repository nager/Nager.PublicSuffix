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
    public void UnParseWithExceptionTest()
    {
        const string rulesInText = """
                                   foo.com
                                   !bar.com
                                   !foo.bar.com
                                   """;
        var (rules1, rules2) = ParseUnParseRules(rulesInText);
        
        Assert.IsTrue(rules1.SequenceEqual(rules2));
        Assert.IsTrue(rules2[1].Type == TldRuleType.WildcardException);
    }

    [TestMethod]
    public void UnParseWithWildCardTest()
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
        var (rules1, rules2) = ParseUnParseRules(rulesInText);
        Assert.IsTrue(rules1.SequenceEqual(rules2));
        Assert.IsTrue(rules2[3].Type == TldRuleType.Wildcard);
    }

    private static (TldRule[] rules1, TldRule[] rules2) ParseUnParseRules(string rulesText)
    {
        var ruleParser = new TldRuleParser();

        var rules1 = ruleParser.ParseRules(rulesText).ToArray();
        var rulesUnParsedText = rules1.UnParseRules();
        var rules2 = ruleParser.ParseRules(rulesUnParsedText).ToArray();

        return (rules1, rules2);
    }
}