using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class PublicSuffixTestsWithUriNormalization : PublicSuffixTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"));
            this._domainParser = domainParser;
        }

        [TestMethod]
        public void UnderscoreCheck()
        {
            this.CheckPublicSuffix("_abc.def.ghi.jkl.com", "jkl.com");
            this.CheckPublicSuffix("abc._def.ghi.jkl.com", "jkl.com");
            this.CheckPublicSuffix("def._ghi.jkl.com", "jkl.com");

            // These tests demonstrate unwanted behaviour due to a 'bug' in Uri construction
            // def._ghi.jkl.com is valid but
            // abc.def._ghi.jkl.com is invalid.
            // It can't be correct that adding abc. in front of a valid domain makes it invalid
            this.CheckPublicSuffix("abc.def._ghi.jkl.com", null);
            this.CheckPublicSuffix("abc.def.ghi._jkl.com", null);
        }
    }
}
