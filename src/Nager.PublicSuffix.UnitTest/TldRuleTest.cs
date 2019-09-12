using System;
using Xunit;

namespace Nager.PublicSuffix.UnitTest {

    public class TldRuleTest {
        [Fact]
        public void InvalidRuleTest1 () {
            var exception = Assert.Throws<ArgumentException> (() => new TldRule (""));
            Assert.Equal ("RuleData is empty", exception.Message);
        }

        [Fact]
        public void InvalidRuleTest2 () {
            var exception = Assert.Throws<ArgumentException> (() => new TldRule (null));
            Assert.Equal ("RuleData is empty", exception.Message);
        }

        [Fact]
        public void InvalidRuleTest3 () {
            var exception = Assert.Throws<FormatException> (() => new TldRule ("*com"));
            Assert.Equal ("Wildcard syntax not correct", exception.Message);
        }

        [Fact]
        public void InvalidRuleTest4 () {
            var exception = Assert.Throws<FormatException> (() => new TldRule ("*bar.foo"));
            Assert.Equal ("Wildcard syntax not correct", exception.Message);
        }

        [Fact]
        public void InvalidRuleTest5 () {
            var exception = Assert.Throws<FormatException> (() => new TldRule (".com"));
            Assert.Equal ("Rule contains empty part", exception.Message);
        }

        [Fact]
        public void InvalidRuleTest6 () {
            var exception = Assert.Throws<FormatException> (() => new TldRule ("www..com"));
            Assert.Equal ("Rule contains empty part", exception.Message);
        }

        [Fact]
        public void ValidRuleTest1 () {
            var tldRule = new TldRule ("com");
            Assert.Equal ("com", tldRule.Name);
            Assert.Equal (TldRuleType.Normal, tldRule.Type);
        }

        [Fact]
        public void ValidRuleTest2 () {
            var tldRule = new TldRule ("*.com");
            Assert.Equal ("*.com", tldRule.Name);
            Assert.Equal (TldRuleType.Wildcard, tldRule.Type);
        }

        [Fact]
        public void ValidRuleTest3 () {
            var tldRule = new TldRule ("!com");
            Assert.Equal ("com", tldRule.Name);
            Assert.Equal (TldRuleType.WildcardException, tldRule.Type);
        }

        [Fact]
        public void ValidRuleTest4 () {
            var tldRule = new TldRule ("co.uk");
            Assert.Equal ("co.uk", tldRule.Name);
            Assert.Equal (TldRuleType.Normal, tldRule.Type);
        }

        [Fact]
        public void ValidRuleTest5 () {
            var tldRule = new TldRule ("*.*.foo");
            Assert.Equal ("*.*.foo", tldRule.Name);
            Assert.Equal (TldRuleType.Wildcard, tldRule.Type);
        }

        [Fact]
        public void ValidRuleTest6 () {
            var tldRule = new TldRule ("a.b.web.*.foo", TldRuleDivision.Private);
            Assert.Equal ("a.b.web.*.foo", tldRule.Name);
            Assert.Equal (TldRuleDivision.Private, tldRule.Division);
        }
    }
}