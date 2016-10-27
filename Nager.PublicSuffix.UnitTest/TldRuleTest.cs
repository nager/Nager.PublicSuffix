using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class TldRuleTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "RuleData is emtpy")]
        public void InvalidRuleTest1()
        {
            new TldRule("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "RuleData is emtpy")]
        public void InvalidRuleTest2()
        {
            new TldRule(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "Wildcard syntax not correct")]
        public void InvalidRuleTest3()
        {
            new TldRule("*com");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "Wildcard syntax not correct")]
        public void InvalidRuleTest4()
        {
            new TldRule("*");
        }

        [TestMethod]
        public void ValidRuleTest1()
        {
            var tldRule = new TldRule("com");
            Assert.AreEqual("com", tldRule.Name);
            Assert.AreEqual(TldRuleType.Normal, tldRule.Type);
        }

        [TestMethod]
        public void ValidRuleTest2()
        {
            var tldRule = new TldRule("*.com");
            Assert.AreEqual("com", tldRule.Name);
            Assert.AreEqual(TldRuleType.Wildcard, tldRule.Type);
        }

        [TestMethod]
        public void ValidRuleTest3()
        {
            var tldRule = new TldRule("!com");
            Assert.AreEqual("com", tldRule.Name);
            Assert.AreEqual(TldRuleType.WildcardException, tldRule.Type);
        }
    }
}
