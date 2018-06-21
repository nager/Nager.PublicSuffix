using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class PublicSuffixTestsWithIdnMappingNormalization : PublicSuffixTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"), new IdnMappingNormalizer());
            this._domainParser = domainParser;
        }

        [TestMethod]
        public void UnderscoreCheck()
        {
            this.CheckPublicSuffix("_abc.def.ghi.jkl.com", "jkl.com");
            this.CheckPublicSuffix("abc._def.ghi.jkl.com", "jkl.com");
            this.CheckPublicSuffix("def._ghi.jkl.com", "jkl.com");

            // These tests demonstrate the wanted behaviour avoiding the 'bug' in Uri construction
            this.CheckPublicSuffix("abc.def._ghi.jkl.com", "jkl.com");
            this.CheckPublicSuffix("abc.def.ghi._jkl.com", "_jkl.com");
        }
    }
}
