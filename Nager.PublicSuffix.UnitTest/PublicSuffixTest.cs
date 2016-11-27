using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class PublicSuffixTest
    {
        private DomainParser _domainParser;

        //Run tests as specified here:
        //https://raw.githubusercontent.com/publicsuffix/list/master/tests/test_psl.txt

        [TestInitialize()]
        public void Initialize()
        {
            var domainParser = new DomainParser();
            var ruleData = File.ReadAllText("effective_tld_names.dat");
            var rules = domainParser.ParseRules(ruleData);
            domainParser.AddRules(rules);

            this._domainParser = domainParser;
        }

        private void CheckPublicSuffix(string domain, string expected)
        {
            Assert.IsNotNull(this._domainParser, "_domainParser is null");

            if (!string.IsNullOrEmpty(domain))
            {
                domain = domain.ToLowerInvariant();
            }

            var domainData = this._domainParser.Get(domain);
            if (domainData == null)
            {
                Assert.IsNull(expected);
            }
            else
            {
                Assert.AreEqual(expected, domainData.RegistrableDomain);
            }
        }

        [TestMethod]
        public void ComprehensiveCheck()
        {
            // null input.
            this.CheckPublicSuffix(null, null);

            // Mixed case.
            this.CheckPublicSuffix("COM", null);
            this.CheckPublicSuffix("example.COM", "example.com");
            this.CheckPublicSuffix("WwW.example.COM", "example.com");

            // Leading dot.
            this.CheckPublicSuffix(".com", null);
            this.CheckPublicSuffix(".example", null);
            this.CheckPublicSuffix(".example.com", null);
            this.CheckPublicSuffix(".example.example", null);

            // Unlisted TLD.
            this.CheckPublicSuffix("example", null);
            this.CheckPublicSuffix("example.example", "example.example");
            this.CheckPublicSuffix("b.example.example", "example.example");
            this.CheckPublicSuffix("a.b.example.example", "example.example");

            // Listed, but non-Internet, TLD.
            //this.CheckPublicSuffix("local", null);
            //this.CheckPublicSuffix("example.local", null);
            //this.CheckPublicSuffix("b.example.local", null);
            //this.CheckPublicSuffix("a.b.example.local", null);

            // TLD with only 1 rule.
            this.CheckPublicSuffix("biz", null);
            this.CheckPublicSuffix("domain.biz", "domain.biz");
            this.CheckPublicSuffix("b.domain.biz", "domain.biz");
            this.CheckPublicSuffix("a.b.domain.biz", "domain.biz");

            // TLD with some 2-level rules.
            this.CheckPublicSuffix("com", null);
            this.CheckPublicSuffix("example.com", "example.com");
            this.CheckPublicSuffix("b.example.com", "example.com");
            this.CheckPublicSuffix("a.b.example.com", "example.com");
            this.CheckPublicSuffix("uk.com", null);
            this.CheckPublicSuffix("example.uk.com", "example.uk.com");
            this.CheckPublicSuffix("b.example.uk.com", "example.uk.com");
            this.CheckPublicSuffix("a.b.example.uk.com", "example.uk.com");
            this.CheckPublicSuffix("test.ac", "test.ac");

            // TLD with only 1 (wildcard) rule.
            this.CheckPublicSuffix("mm", null);
            this.CheckPublicSuffix("c.mm", null);
            this.CheckPublicSuffix("b.c.mm", "b.c.mm");
            this.CheckPublicSuffix("a.b.c.mm", "b.c.mm");

            // More complex TLD.
            this.CheckPublicSuffix("jp", null);
            this.CheckPublicSuffix("test.jp", "test.jp");
            this.CheckPublicSuffix("www.test.jp", "test.jp");
            this.CheckPublicSuffix("ac.jp", null);
            this.CheckPublicSuffix("test.ac.jp", "test.ac.jp");
            this.CheckPublicSuffix("www.test.ac.jp", "test.ac.jp");
            this.CheckPublicSuffix("kyoto.jp", null);
            this.CheckPublicSuffix("test.kyoto.jp", "test.kyoto.jp");
            this.CheckPublicSuffix("ide.kyoto.jp", null);
            this.CheckPublicSuffix("b.ide.kyoto.jp", "b.ide.kyoto.jp");
            this.CheckPublicSuffix("a.b.ide.kyoto.jp", "b.ide.kyoto.jp");
            this.CheckPublicSuffix("c.kobe.jp", null);
            this.CheckPublicSuffix("b.c.kobe.jp", "b.c.kobe.jp");
            this.CheckPublicSuffix("a.b.c.kobe.jp", "b.c.kobe.jp");
            this.CheckPublicSuffix("city.kobe.jp", "city.kobe.jp");
            this.CheckPublicSuffix("www.city.kobe.jp", "city.kobe.jp");

            // TLD with a wildcard rule and exceptions.
            this.CheckPublicSuffix("ck", null);
            this.CheckPublicSuffix("test.ck", null);
            this.CheckPublicSuffix("b.test.ck", "b.test.ck");
            this.CheckPublicSuffix("a.b.test.ck", "b.test.ck");
            this.CheckPublicSuffix("www.ck", "www.ck");
            this.CheckPublicSuffix("www.www.ck", "www.ck");

            // US K12.
            this.CheckPublicSuffix("us", null);
            this.CheckPublicSuffix("test.us", "test.us");
            this.CheckPublicSuffix("www.test.us", "test.us");
            this.CheckPublicSuffix("ak.us", null);
            this.CheckPublicSuffix("test.ak.us", "test.ak.us");
            this.CheckPublicSuffix("www.test.ak.us", "test.ak.us");
            this.CheckPublicSuffix("k12.ak.us", null);
            this.CheckPublicSuffix("test.k12.ak.us", "test.k12.ak.us");
            this.CheckPublicSuffix("www.test.k12.ak.us", "test.k12.ak.us");
        }

        [TestMethod]
        public void IdnDomainCheck()
        {
            // IDN labels.
            this.CheckPublicSuffix("食狮.com.cn", "食狮.com.cn");
            this.CheckPublicSuffix("食狮.公司.cn", "食狮.公司.cn");
            this.CheckPublicSuffix("www.食狮.公司.cn", "食狮.公司.cn");
            this.CheckPublicSuffix("shishi.公司.cn", "shishi.公司.cn");
            this.CheckPublicSuffix("公司.cn", null);
            this.CheckPublicSuffix("食狮.中国", "食狮.中国");
            this.CheckPublicSuffix("www.食狮.中国", "食狮.中国");
            this.CheckPublicSuffix("shishi.中国", "shishi.中国");
            this.CheckPublicSuffix("中国", null);

            // Same as above, but punycoded.
            this.CheckPublicSuffix("xn--85x722f.com.cn", "xn--85x722f.com.cn");
            this.CheckPublicSuffix("xn--85x722f.xn--55qx5d.cn", "xn--85x722f.xn--55qx5d.cn");
            this.CheckPublicSuffix("www.xn--85x722f.xn--55qx5d.cn", "xn--85x722f.xn--55qx5d.cn");
            this.CheckPublicSuffix("shishi.xn--55qx5d.cn", "shishi.xn--55qx5d.cn");
            this.CheckPublicSuffix("xn--55qx5d.cn", null);
            this.CheckPublicSuffix("xn--85x722f.xn--fiqs8s", "xn--85x722f.xn--fiqs8s");
            this.CheckPublicSuffix("www.xn--85x722f.xn--fiqs8s", "xn--85x722f.xn--fiqs8s");
            this.CheckPublicSuffix("shishi.xn--fiqs8s", "shishi.xn--fiqs8s");
            this.CheckPublicSuffix("xn--fiqs8s", null);
        }

        [TestMethod]
        public void TreeSplitCheck()
        {
            //Extra tests (Added due to avoid regression bugs)
            this.CheckPublicSuffix("co.ke", null);
            this.CheckPublicSuffix("blogspot.co.ke", null);
            this.CheckPublicSuffix("web.co.ke", "web.co.ke");
            this.CheckPublicSuffix("a.b.web.co.ke", "web.co.ke");
            this.CheckPublicSuffix("blogspot.co.ke", null);
            this.CheckPublicSuffix("web.blogspot.co.ke", "web.blogspot.co.ke");
            this.CheckPublicSuffix("a.b.web.blogspot.co.ke", "web.blogspot.co.ke");
        }
    }
}
