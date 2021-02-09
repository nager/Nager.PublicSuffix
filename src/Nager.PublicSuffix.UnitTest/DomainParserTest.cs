using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Nager.PublicSuffix.UnitTest
{
    public abstract class DomainParserTest
    {
        protected abstract IDomainParser GetDomainParser(List<TldRule> rules);

        [TestMethod]
        public void Parse_ValidDomain1()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com")
            };

            var domainParser = this.GetDomainParser(rules);

            var domainName = domainParser.Parse("test.com");

            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("com", domainName.TLD);
            Assert.AreEqual("test.com", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.SubDomain);
            Assert.AreEqual("com", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void Parse_ValidDomain2()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk")
            };

            var domainParser = this.GetDomainParser(rules);

            var domainName = domainParser.Parse("test.co.uk");

            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("co.uk", domainName.TLD);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.SubDomain);
            Assert.AreEqual("co.uk", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void Parse_ValidDomainWithSubdomain1()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk")
            };

            var domainParser = this.GetDomainParser(rules);

            var domainName = domainParser.Parse("sub.test.co.uk");

            Assert.AreEqual("test", domainName.Domain);
            Assert.AreEqual("co.uk", domainName.TLD);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual("sub", domainName.SubDomain);
            Assert.AreEqual("co.uk", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void Parse_ValidDomainWithSubdomain2()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk"),
                new TldRule("*.sch.uk")
            };

            var domainParser = this.GetDomainParser(rules);

            var domainName = domainParser.Parse("sub.test1.test2.sch.uk");

            Assert.AreEqual("test1", domainName.Domain);
            Assert.AreEqual("test2.sch.uk", domainName.TLD);
            Assert.AreEqual("test1.test2.sch.uk", domainName.RegistrableDomain);
            Assert.AreEqual("sub", domainName.SubDomain);
            Assert.AreEqual("*.sch.uk", domainName.TLDRule.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void Parse_OnlyTopLevelDomain1()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk")
            };

            var domainParser = this.GetDomainParser(rules);

            domainParser.Parse("co.uk");
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void Parse_OnlyTopLevelDomain2()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com")
            };

            var domainParser = this.GetDomainParser(rules);

            domainParser.Parse("com");
        }

        [TestMethod]
        public void CheckDomainNameForUnlistedTld()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk")
            };

            var domainParser = this.GetDomainParser(rules);

            var domainName = domainParser.Parse("unlisted.domain.example");

            Assert.AreEqual("domain", domainName.Domain);
            Assert.AreEqual("example", domainName.TLD);
            Assert.AreEqual("domain.example", domainName.RegistrableDomain);
            Assert.AreEqual("unlisted", domainName.SubDomain);
            Assert.AreEqual("*", domainName.TLDRule.Name);
        }

        [TestMethod]
        public void IsValidDomain_ValidDomain1()
        {
            var rules = new List<TldRule>
            {
                new TldRule("at"),
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain("nager.at");

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsValidDomain_ValidDomain2()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain("ripe.net");

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsValidDomain_ValidDomain3()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("at")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain("österreich.at");

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsValidDomain_InvalidDomain1()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain("http://ripe.net");

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidDomain_InvalidDomain2()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain("https://ripe.net");

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidDomain_InvalidDomain3()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain("ftp://ripe.net");

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidDomain_InvalidDomain4()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain("....ripe.net");

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidDomain_InvalidDomain5()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain(".");

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidDomain_InvalidDomain6()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain("test");

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidDomain_InvalidDomain7()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain("*.ripe.net");

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValidDomain_InvalidDomain8()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain(" ripe.net");

            Assert.IsFalse(isValid);
        }
    }
}
