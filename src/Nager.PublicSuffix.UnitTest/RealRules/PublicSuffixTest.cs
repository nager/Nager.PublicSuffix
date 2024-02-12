using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.Exceptions;

namespace Nager.PublicSuffix.UnitTest.RealRules
{
    public abstract class PublicSuffixTest
    {
        protected DomainParser _domainParser;

        //Run tests as specified here:
        //https://raw.githubusercontent.com/publicsuffix/list/master/tests/test_psl.txt

        protected void CheckPublicSuffix(string domain, string expected)
        {
            Assert.IsNotNull(this._domainParser, "_domainParser is null");

            var domainData = this._domainParser.Parse(domain);
            if (domainData == null)
            {
                Assert.IsNull(expected);
            }
            else
            {
                Assert.AreEqual(expected, domainData.RegistrableDomain);
            }
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(".com")]
        [DataRow(".example")]
        [DataRow(".example.com")]
        [DataRow(".example.example")]
        [DataRow("example")]
        [DataRow("mm")]
        [DataRow("c.mm")]
        [DataRow("c.kobe.jp")]
        [DataRow("ck")]
        [DataRow("test.ck")]
        [ExpectedException(typeof(ParseException))]
        public void ParseInvalidDomainCheck(string domain)
        {
            this.CheckPublicSuffix(domain, null);
        }

        [TestMethod]
        public void ComprehensiveCheck()
        {
            // Mixed case.
            this.CheckPublicSuffix("example.COM", "example.com");
            this.CheckPublicSuffix("WwW.example.COM", "example.com");

            // Unlisted TLD.
            this.CheckPublicSuffix("example.example", "example.example");
            this.CheckPublicSuffix("b.example.example", "example.example");
            this.CheckPublicSuffix("a.b.example.example", "example.example");

            // Listed, but non-Internet, TLD.
            //this.CheckPublicSuffix("local", null);
            //this.CheckPublicSuffix("example.local", null);
            //this.CheckPublicSuffix("b.example.local", null);
            //this.CheckPublicSuffix("a.b.example.local", null);

            // TLD with only 1 rule.
            this.CheckPublicSuffix("domain.biz", "domain.biz");
            this.CheckPublicSuffix("b.domain.biz", "domain.biz");
            this.CheckPublicSuffix("a.b.domain.biz", "domain.biz");

            // TLD with some 2-level rules.
            this.CheckPublicSuffix("example.com", "example.com");
            this.CheckPublicSuffix("b.example.com", "example.com");
            this.CheckPublicSuffix("a.b.example.com", "example.com");
            this.CheckPublicSuffix("example.uk.com", "example.uk.com");
            this.CheckPublicSuffix("b.example.uk.com", "example.uk.com");
            this.CheckPublicSuffix("a.b.example.uk.com", "example.uk.com");
            this.CheckPublicSuffix("test.ac", "test.ac");

            // TLD with only 1 (wildcard) rule.
            this.CheckPublicSuffix("b.c.mm", "b.c.mm");
            this.CheckPublicSuffix("a.b.c.mm", "b.c.mm");

            // More complex TLD.
            this.CheckPublicSuffix("test.jp", "test.jp");
            this.CheckPublicSuffix("www.test.jp", "test.jp");
            this.CheckPublicSuffix("test.ac.jp", "test.ac.jp");
            this.CheckPublicSuffix("www.test.ac.jp", "test.ac.jp");
            this.CheckPublicSuffix("test.kyoto.jp", "test.kyoto.jp");
            this.CheckPublicSuffix("b.ide.kyoto.jp", "b.ide.kyoto.jp");
            this.CheckPublicSuffix("a.b.ide.kyoto.jp", "b.ide.kyoto.jp");
            this.CheckPublicSuffix("b.c.kobe.jp", "b.c.kobe.jp");
            this.CheckPublicSuffix("a.b.c.kobe.jp", "b.c.kobe.jp");
            
            this.CheckPublicSuffix("city.kobe.jp", "city.kobe.jp");
            this.CheckPublicSuffix("www.city.kobe.jp", "city.kobe.jp");

            // TLD with a wildcard rule and exceptions.

            this.CheckPublicSuffix("b.test.ck", "b.test.ck");
            this.CheckPublicSuffix("a.b.test.ck", "b.test.ck");
            this.CheckPublicSuffix("www.ck", "www.ck");
            this.CheckPublicSuffix("www.www.ck", "www.ck");

            // US K12.

            this.CheckPublicSuffix("test.us", "test.us");
            this.CheckPublicSuffix("www.test.us", "test.us");
            this.CheckPublicSuffix("test.ak.us", "test.ak.us");
            this.CheckPublicSuffix("www.test.ak.us", "test.ak.us");
            this.CheckPublicSuffix("test.k12.ak.us", "test.k12.ak.us");
            this.CheckPublicSuffix("www.test.k12.ak.us", "test.k12.ak.us");
        }

        [DataTestMethod]
        [DataRow("COM")]
        [DataRow("com")]
        [DataRow("公司.cn")]
        [DataRow("中国")]
        [DataRow("xn--55qx5d.cn")]
        [DataRow("blogspot.co.ke")]
        [DataRow("xn--fiqs8s")]
        [DataRow("us")]
        [DataRow("ak.us")]
        [DataRow("k12.ak.us")]
        [DataRow("kyoto.jp")]
        [DataRow("ide.kyoto.jp")]
        [DataRow("ac.jp")]
        [DataRow("jp")]
        [DataRow("uk.com")]
        [DataRow("biz")]
        [DataRow("w.bg")]
        [DataRow("bi")]
        [DataRow("info.bo")]
        [DataRow("ns.ca")]
        [DataRow("ha.cn")]
        [DataRow("fm")]
        [DataRow("gmx")]
        [DataRow("hotmail")]
        [DataRow("cloudfront.net")]
        [DataRow("us-east-1.amazonaws.com")]
        [DataRow("ec2-34-206-8-177.compute-1.amazonaws.com")]
        [DataRow("s3-website.us-east-2.amazonaws.com")]
        [DataRow("test.stg.dev")]
        [ExpectedExceptionWithMessage(typeof(ParseException), "Domain is a TLD according publicsuffix")]
        public void TldCheck(string domain)
        {
            this.CheckPublicSuffix(domain, null);
        }

        [TestMethod]
        public void IdnDomainCheck()
        {
            // IDN labels.
            this.CheckPublicSuffix("食狮.com.cn", "食狮.com.cn");
            this.CheckPublicSuffix("食狮.公司.cn", "食狮.公司.cn");
            this.CheckPublicSuffix("www.食狮.公司.cn", "食狮.公司.cn");
            this.CheckPublicSuffix("shishi.公司.cn", "shishi.公司.cn");

            this.CheckPublicSuffix("食狮.中国", "食狮.中国");
            this.CheckPublicSuffix("www.食狮.中国", "食狮.中国");
            this.CheckPublicSuffix("shishi.中国", "shishi.中国");

            // Same as above, but punycoded.
            this.CheckPublicSuffix("xn--85x722f.com.cn", "xn--85x722f.com.cn");
            this.CheckPublicSuffix("xn--85x722f.xn--55qx5d.cn", "xn--85x722f.xn--55qx5d.cn");
            this.CheckPublicSuffix("www.xn--85x722f.xn--55qx5d.cn", "xn--85x722f.xn--55qx5d.cn");
            this.CheckPublicSuffix("shishi.xn--55qx5d.cn", "shishi.xn--55qx5d.cn");

            this.CheckPublicSuffix("xn--85x722f.xn--fiqs8s", "xn--85x722f.xn--fiqs8s");
            this.CheckPublicSuffix("www.xn--85x722f.xn--fiqs8s", "xn--85x722f.xn--fiqs8s");
            this.CheckPublicSuffix("shishi.xn--fiqs8s", "shishi.xn--fiqs8s");
        }

        [TestMethod]
        public void TreeSplitCheck1()
        {
            //Extra tests (Added due to avoid regression bugs)
            this.CheckPublicSuffix("web.co.ke", "web.co.ke");
            this.CheckPublicSuffix("a.b.web.co.ke", "web.co.ke");
            this.CheckPublicSuffix("web.blogspot.co.ke", "web.blogspot.co.ke");
            this.CheckPublicSuffix("a.b.web.blogspot.co.ke", "web.blogspot.co.ke");
        }
    }
}
