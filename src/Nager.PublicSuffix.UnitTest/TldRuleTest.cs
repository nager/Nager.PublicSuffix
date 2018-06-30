using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class TldRuleTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "RuleData is empty")]
        public void InvalidRuleTest1()
        {
            new TldRule("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "RuleData is empty")]
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
            new TldRule("*bar.foo");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "Rule contains invalid empty part")]
        public void InvalidRuleTest5()
        {
            new TldRule(".com");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "Rule contains invalid empty part")]
        public void InvalidRuleTest6()
        {
            new TldRule("www..com");
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
            Assert.AreEqual("*.com", tldRule.Name);
            Assert.AreEqual(TldRuleType.Wildcard, tldRule.Type);
        }

        [TestMethod]
        public void ValidRuleTest3()
        {
            var tldRule = new TldRule("!com");
            Assert.AreEqual("com", tldRule.Name);
            Assert.AreEqual(TldRuleType.WildcardException, tldRule.Type);
        }

        [TestMethod]
        public void ValidRuleTest4()
        {
            var tldRule = new TldRule("co.uk");
            Assert.AreEqual("co.uk", tldRule.Name);
            Assert.AreEqual(TldRuleType.Normal, tldRule.Type);
        }

        [TestMethod]
        public void ValidRuleTest5()
        {
            var tldRule = new TldRule("*.*.foo");
            Assert.AreEqual("*.*.foo", tldRule.Name);
            Assert.AreEqual(TldRuleType.Wildcard, tldRule.Type);
        }

        [TestMethod]
        public void ValidRuleTest6()
        {
            var tldRule = new TldRule("a.b.web.*.foo", TldRuleDivision.Private);
            Assert.AreEqual("a.b.web.*.foo", tldRule.Name);
            Assert.AreEqual(TldRuleDivision.Private, tldRule.Division);
        }
    }
}
