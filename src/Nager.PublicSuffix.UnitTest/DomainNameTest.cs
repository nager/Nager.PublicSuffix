using System.Collections.Generic;
using Xunit;

namespace Nager.PublicSuffix.UnitTest {
    public abstract class DomainNameTest {
        protected abstract DomainParser GetParserForRules (List<TldRule> rules);

        [Fact]
        public void CheckDomainName1 () {
            var rules = new List<TldRule> ();
            rules.Add (new TldRule ("com"));
            var domainParser = this.GetParserForRules (rules);

            var domainName = domainParser.Get ("test.com");

            Assert.Equal ("test", domainName.Domain);
            Assert.Equal ("com", domainName.TLD);
            Assert.Equal ("test.com", domainName.RegistrableDomain);
            Assert.Equal (null, domainName.SubDomain);
            Assert.Equal ("com", domainName.TLDRule.Name);
        }

        [Fact]
        public void CheckDomainName2 () {
            var rules = new List<TldRule> ();
            rules.Add (new TldRule ("uk"));
            rules.Add (new TldRule ("co.uk"));
            var domainParser = this.GetParserForRules (rules);

            var domainName = domainParser.Get ("test.co.uk");

            Assert.Equal ("test", domainName.Domain);
            Assert.Equal ("co.uk", domainName.TLD);
            Assert.Equal ("test.co.uk", domainName.RegistrableDomain);
            Assert.Equal (null, domainName.SubDomain);
            Assert.Equal ("co.uk", domainName.TLDRule.Name);
        }

        [Fact]
        public void CheckDomainName3 () {
            var rules = new List<TldRule> ();
            rules.Add (new TldRule ("uk"));
            rules.Add (new TldRule ("co.uk"));
            var domainParser = this.GetParserForRules (rules);

            var domainName = domainParser.Get ("sub.test.co.uk");

            Assert.Equal ("test", domainName.Domain);
            Assert.Equal ("co.uk", domainName.TLD);
            Assert.Equal ("test.co.uk", domainName.RegistrableDomain);
            Assert.Equal ("sub", domainName.SubDomain);
            Assert.Equal ("co.uk", domainName.TLDRule.Name);
        }

        [Fact]
        public void CheckDomainName4 () {
            var rules = new List<TldRule> ();
            rules.Add (new TldRule ("uk"));
            rules.Add (new TldRule ("co.uk"));
            rules.Add (new TldRule ("*.sch.uk"));
            var domainParser = this.GetParserForRules (rules);

            var domainName = domainParser.Get ("sub.test1.test2.sch.uk");

            Assert.Equal ("test1", domainName.Domain);
            Assert.Equal ("test2.sch.uk", domainName.TLD);
            Assert.Equal ("test1.test2.sch.uk", domainName.RegistrableDomain);
            Assert.Equal ("sub", domainName.SubDomain);
            Assert.Equal ("*.sch.uk", domainName.TLDRule.Name);
        }

        [Fact]
        public void CheckDomainNameForUnlistedTld () {
            var rules = new List<TldRule> ();
            rules.Add (new TldRule ("uk"));
            rules.Add (new TldRule ("co.uk"));
            var domainParser = this.GetParserForRules (rules);

            var domainName = domainParser.Get ("unlisted.domain.example");

            Assert.Equal ("domain", domainName.Domain);
            Assert.Equal ("example", domainName.TLD);
            Assert.Equal ("domain.example", domainName.RegistrableDomain);
            Assert.Equal ("unlisted", domainName.SubDomain);
            Assert.Equal ("*", domainName.TLDRule.Name);
        }
    }
}