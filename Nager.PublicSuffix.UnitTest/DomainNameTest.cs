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
            var rules = new List<TldRule>();
            rules.Add(new TldRule("com"));
            var domainParser = this.GetParserForRules(rules);

            var domainName = domainParser.Get("test.com");

            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("com", domainName.TLD);
            Assert.AreEqual("test.com", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.SubDomain);
            Assert.AreEqual("com", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void CheckDomainName2()
        {
            var rules = new List<TldRule>();
            rules.Add(new TldRule("uk"));
            rules.Add(new TldRule("co.uk"));
            var domainParser = this.GetParserForRules(rules);

            var domainName = domainParser.Get("test.co.uk");

            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("co.uk", domainName.TLD);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.SubDomain);
            Assert.AreEqual("co.uk", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void CheckDomainName3()
        {
            var rules = new List<TldRule>();
            rules.Add(new TldRule("uk"));
            rules.Add(new TldRule("co.uk"));
            var domainParser = this.GetParserForRules(rules);

            var domainName = domainParser.Get("sub.test.co.uk");

            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("co.uk", domainName.TLD);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual("sub", domainName.SubDomain);
            Assert.AreEqual("co.uk", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void CheckDomainName4()
        {
            var rules = new List<TldRule>();
            rules.Add(new TldRule("uk"));
            rules.Add(new TldRule("co.uk"));
            rules.Add(new TldRule("*.sch.uk"));
            var domainParser = this.GetParserForRules(rules);

            var domainName = domainParser.Get("sub.test1.test2.sch.uk");

            Assert.AreEqual("test1", domainName.Domain);
            Assert.AreEqual("test2.sch.uk", domainName.TLD);
            Assert.AreEqual("test1.test2.sch.uk", domainName.RegistrableDomain);
            Assert.AreEqual("sub", domainName.SubDomain);
            Assert.AreEqual("*.sch.uk", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void CheckDomainNameForUnlistedTld()
        {
            var rules = new List<TldRule>();
            rules.Add(new TldRule("uk"));
            rules.Add(new TldRule("co.uk"));
            var domainParser = this.GetParserForRules(rules);

            var domainName = domainParser.Get("unlisted.domain.example");

            Assert.AreEqual("domain", domainName.Domain);
            Assert.AreEqual("example", domainName.TLD);
            Assert.AreEqual("domain.example", domainName.RegistrableDomain);
            Assert.AreEqual("unlisted", domainName.SubDomain);
            Assert.AreEqual("*", domainName.TLDRule.Name);
        }
    }
}
