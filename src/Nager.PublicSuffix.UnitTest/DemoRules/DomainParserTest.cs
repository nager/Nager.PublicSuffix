using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.Models;
using System.Collections.Generic;

namespace Nager.PublicSuffix.UnitTest.DemoRules
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
            Assert.AreEqual("com", domainName.TopLevelDomain);
            Assert.AreEqual("test.com", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.Subdomain);
            Assert.AreEqual("com", domainName.TopLevelDomainRule.Name);
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
            Assert.AreEqual("co.uk", domainName.TopLevelDomain);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual(null, domainName.Subdomain);
            Assert.AreEqual("co.uk", domainName.TopLevelDomainRule.Name);
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
            Assert.AreEqual("co.uk", domainName.TopLevelDomain);
            Assert.AreEqual("test.co.uk", domainName.RegistrableDomain);
            Assert.AreEqual("sub", domainName.Subdomain);
            Assert.AreEqual("co.uk", domainName.TopLevelDomainRule.Name);
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
            Assert.AreEqual("test2.sch.uk", domainName.TopLevelDomain);
            Assert.AreEqual("test1.test2.sch.uk", domainName.RegistrableDomain);
            Assert.AreEqual("sub", domainName.Subdomain);
            Assert.AreEqual("*.sch.uk", domainName.TopLevelDomainRule.Name);
        }

        [TestMethod]
        public void Parse_OnlyTopLevelDomain1()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk")
            };

            var domainParser = this.GetDomainParser(rules);

            var domainInfo = domainParser.Parse("co.uk");
            Assert.AreEqual("co.uk", domainInfo.TopLevelDomain);
            Assert.AreEqual(null, domainInfo.RegistrableDomain);
        }

        [TestMethod]
        public void Parse_OnlyTopLevelDomain2()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com")
            };

            var domainParser = this.GetDomainParser(rules);

            var domainInfo = domainParser.Parse("com");
            Assert.AreEqual("com", domainInfo.TopLevelDomain);
            Assert.AreEqual(null, domainInfo.RegistrableDomain);
        }

        [TestMethod]
        public void CheckDomainNameForNotListedTld()
        {
            var rules = new List<TldRule>
            {
                new TldRule("uk"),
                new TldRule("co.uk")
            };

            var domainParser = this.GetDomainParser(rules);

            var domainInfo = domainParser.Parse("unlisted.domain.example");

            Assert.IsNull(domainInfo);
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
        public void IsValidDomain_WithHttpScheme_ReturnsFalse()
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
        public void IsValidDomain_WithHttpsScheme_ReturnsFalse()
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
        public void IsValidDomain_WithUriScheme_ReturnsFalse()
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
        public void IsValidDomain_WithLeadingDots_ReturnsFalse()
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
        public void IsValidDomain_SingleDot_ReturnsFalse()
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
        public void IsValidDomain_WithoutTopLevelDomain_ReturnsFalse()
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
        public void IsValidDomain_WithWildcardCharacter_ReturnsFalse()
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
        public void IsValidDomain_WithLeadingWhitespace_ReturnsFalse()
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

        [TestMethod]
        public void IsValidDomain_WithEmailAddressInsteadOfDomain_ReturnsFalse()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            var isValid = domainParser.IsValidDomain("test@ripe.net");

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void TryParse_WithValidDomain_ReturnsDomainInfo()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            if (domainParser.TryParse("ripe.net", out var domainInfo))
            {
                Assert.IsNotNull(domainInfo);
                return;
            }

            Assert.Fail("Parse domain failure");
        }

        [TestMethod]
        public void TryParse_WithInvalidDomain_ReturnsNull()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com"),
                new TldRule("de"),
                new TldRule("net")
            };

            var domainParser = this.GetDomainParser(rules);

            if (domainParser.TryParse("ripe.86aa", out var domainInfo))
            {
                Assert.Fail("Invalid domain parsed");
                return;
            }

            Assert.IsNull(domainInfo);
        }
    }
}
