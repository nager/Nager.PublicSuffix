using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class DomainNameTest
    {
        [TestMethod]
        public void CheckDomainName1()
        {
            var domainParser = new DomainParser();
            domainParser.AddRule(new TldRule("com"));

            var domainName = domainParser.Get("test.com");
            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("com", domainName.TLD);
            Assert.AreEqual("test.com", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.SubDomain);
        }

        [TestMethod]
        public void CheckDomainName2()
        {
            var domainParser = new DomainParser();
            domainParser.AddRule(new TldRule("uk"));
            domainParser.AddRule(new TldRule("co.uk"));

            var domainName = domainParser.Get("test.co.uk");
            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("co.uk", domainName.TLD);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.SubDomain);
        }

        [TestMethod]
        public void CheckDomainName3()
        {
            var domainParser = new DomainParser();
            domainParser.AddRule(new TldRule("uk"));
            domainParser.AddRule(new TldRule("co.uk"));

            var domainName = domainParser.Get("sub.test.co.uk");
            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("co.uk", domainName.TLD);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual("sub", domainName.SubDomain);
        }
    }
}
