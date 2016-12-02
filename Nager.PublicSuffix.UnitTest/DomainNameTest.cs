using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class DomainNameTest
    {
        [TestMethod]
        public void CheckDomainName1()
        {
            var rules = new List<TldRule>();
            rules.Add(new TldRule("com"));

            var domainParser = new DomainParser(rules);

            var domainName = domainParser.Get("test.com");
            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("com", domainName.TLD);
            Assert.AreEqual("test.com", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.SubDomain);
        }

        [TestMethod]
        public void CheckDomainName2()
        {
            var rules = new List<TldRule>();
            rules.Add(new TldRule("uk"));
            rules.Add(new TldRule("co.uk"));

            var domainParser = new DomainParser(rules);

            var domainName = domainParser.Get("test.co.uk");
            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("co.uk", domainName.TLD);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.SubDomain);
        }

        [TestMethod]
        public void CheckDomainName3()
        {
            var rules = new List<TldRule>();
            rules.Add(new TldRule("uk"));
            rules.Add(new TldRule("co.uk"));

            var domainParser = new DomainParser(rules);

            var domainName = domainParser.Get("sub.test.co.uk");
            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("co.uk", domainName.TLD);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual("sub", domainName.SubDomain);
        }
    }
}
