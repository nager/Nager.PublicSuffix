using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.Extensions;
using Nager.PublicSuffix.Models;
using System.Linq;

namespace Nager.PublicSuffix.UnitTest;

[TestClass]
public class DomainDataStructureExtensionsTests
{
    [TestMethod]
    public void GetRulesTest()
    {
        var structure = new DomainDataStructure("");
        var rulesIn = new[]
        {
            new TldRule("foo", TldRuleDivision.Private),
            new TldRule("bar", TldRuleDivision.Private ),
            new TldRule("to.gov.br", TldRuleDivision.ICANN),
            new TldRule("*.bd", TldRuleDivision.ICANN),
            new TldRule("un.known", TldRuleDivision.Unknown)
        };
        
        structure.AddRules(rulesIn);

        var rulesOut = structure.GetRules().ToArray();
        
        CollectionAssert.AreEqual(rulesIn,  rulesOut);
    }
}