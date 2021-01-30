using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Nager.PublicSuffix.UnitTest
{
    public abstract class DomainNameTest
    {
        protected abstract DomainParser GetParserForRules(List<TldRule> rules);

        [TestMethod]
        public void CheckDomainName1()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com")
            };

            var domainParser = this.GetParserForRules(rules);

            var domainName = domainParser.Parse("test.com");

            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("com", domainName.TLD);
            Assert.AreEqual("test.com", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.SubDomain);
            Assert.AreEqual("com", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void CheckDomainName2()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk")
            };

            var domainParser = this.GetParserForRules(rules);

            var domainName = domainParser.Parse("test.co.uk");

            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("co.uk", domainName.TLD);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.SubDomain);
            Assert.AreEqual("co.uk", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void CheckDomainName3()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk")
            };

            var domainParser = this.GetParserForRules(rules);

            var domainName = domainParser.Parse("sub.test.co.uk");

            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("co.uk", domainName.TLD);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual("sub", domainName.SubDomain);
            Assert.AreEqual("co.uk", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void CheckDomainName4()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk"),
                new TldRule("*.sch.uk")
            };

            var domainParser = this.GetParserForRules(rules);

            var domainName = domainParser.Parse("sub.test1.test2.sch.uk");

            Assert.AreEqual("test1", domainName.Domain);
            Assert.AreEqual("test2.sch.uk", domainName.TLD);
            Assert.AreEqual("test1.test2.sch.uk", domainName.RegistrableDomain);
            Assert.AreEqual("sub", domainName.SubDomain);
            Assert.AreEqual("*.sch.uk", domainName.TLDRule.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void CheckDomainName5()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk")
            };

            var domainParser = this.GetParserForRules(rules);

            domainParser.Parse("co.uk");
        }

        [TestMethod]
        public void CheckDomainNameForUnlistedTld()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk")
            };

            var domainParser = this.GetParserForRules(rules);

            var domainName = domainParser.Parse("unlisted.domain.example");

            Assert.AreEqual("domain", domainName.Domain);
            Assert.AreEqual("example", domainName.TLD);
            Assert.AreEqual("domain.example", domainName.RegistrableDomain);
            Assert.AreEqual("unlisted", domainName.SubDomain);
            Assert.AreEqual("*", domainName.TLDRule.Name);
        }
    }
}
