using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.DomainNormalizers;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class DomainNormalizerTest
    {
        [DataRow("microsoft.com")]
        [DataRow("Microsoft.com")]
        [DataRow("HTTPS://microsoft.com")]
        [DataRow("HTTPS://Microsoft.com")]
        [DataRow("HTTPS://MICROSOFT.COM")]
        [DataTestMethod]
        public void UriDomainNormalizerTest(string domain)
        {
            var domainNormalizer = new UriDomainNormalizer();
            var domainParts = domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(domain, out var partlyNormalizedDomain);

            var expectedDomainParts = new[] { "com", "microsoft" };
            var expectedPartlyNormalizedDomain = "microsoft.com";

            Assert.AreEqual(expectedPartlyNormalizedDomain, partlyNormalizedDomain);
            CollectionAssert.AreEqual(expectedDomainParts, domainParts);
        }

        [DataRow("xn--frisr-mua.com")]
        [DataRow("XN--frisr-mua.com")]
        [DataTestMethod]
        public void IdnMappingDomainNormalizerTest(string domain)
        {
            var domainNormalizer = new IdnMappingDomainNormalizer();
            var domainParts = domainNormalizer.PartlyNormalizeDomainAndExtractFullyNormalizedParts(domain, out var partlyNormalizedDomain);

            var expectedDomainParts = new[] { "com", "frisör" };
            var expectedPartlyNormalizedDomain = "xn--frisr-mua.com";

            Assert.AreEqual(expectedPartlyNormalizedDomain, partlyNormalizedDomain);
            CollectionAssert.AreEqual(expectedDomainParts, domainParts);
        }
    }
}
