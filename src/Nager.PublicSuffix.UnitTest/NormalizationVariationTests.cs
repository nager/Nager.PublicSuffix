using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.DomainNormalizers;
using Nager.PublicSuffix.Exceptions;
using System.Collections.Generic;

namespace Nager.PublicSuffix.UnitTest
{
    // Matching TLDs against entries in the PublicSuffix list requires punycode be translated into UTF8
    // This can be accomplised either by using the Uri class or the IdnMapping class.
    // Both methods are constrasted here.
    // These tests demonstrate how the Uri class is performing additional validation/transformation on the domain name
    // Consumers of the DomainParser who are realy interested in the use of the PublicSuffix list to
    // examine behaviour around TLDs and their presence/absence/rules may not wish the outcomes to be
    // influenced by other rules imposed by the Uri class
    [TestClass]
    public class NormalizationVariationTests
    {
        private IDomainParser _parserUsingUriNormalization;
        private IDomainParser _parserUsingIdnNormalization;

        [TestInitialize()]
        public void Initialize()
        {
            var rules = new List<TldRule>
            {
                new TldRule("com")
            };

            this._parserUsingUriNormalization = new DomainParser(rules, new UriNormalizer());
            this._parserUsingIdnNormalization = new DomainParser(rules, new IdnMappingNormalizer());
        }

        [TestMethod]
        public void UnderscoreBasedVariationsValid()
        {
            // this test reveals the motivation for creating a different method of normalization because of the handling of underscore characters
            // by the Uri class.
            // Some online services require the creation of DNS CNAME records for "DomainKeys Identified Mail" (DKIM) which
            // may have values formatted like
            // mail-this-that-co-uk.dkim1._domainkey.firebasemail.com
            // selector1-this.co.uk._domainkey.that.onmicrosoft.com 
            // Attempting to use the DomainParser to validate that these relate to real TLDs does not work when using the Uri class

            // these domains produce the same result for both normalization methods.
            this.PerformParsingCheck("_abc.def.ghi.jkl.com", "jkl.com", "jkl.com");
            this.PerformParsingCheck("abc._def.ghi.jkl.com", "jkl.com", "jkl.com");
            this.PerformParsingCheck("def._ghi.jkl.com", "jkl.com", "jkl.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void UnderscoreBasedVariationsInvalid()
        {
            // These domains are treated as invalid and produce null via the Uri normalization method.
            // def._ghi.jkl.com is valid but
            // abc.def._ghi.jkl.com is invalid.
            // It can't be correct that adding abc. in front of a valid domain makes it invalid
            this.PerformParsingCheck("abc.def._ghi.jkl.com", null, "jkl.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void SingleWordTest()
        {
            this.PerformParsingCheck("singleword", null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void SimpleNumberTest()
        {
            // Uri object transforms 12344 to 48.57 because 12345 = (48 * 256) + (57 * 1)
            // This creates two parts separated by a dot which allows the rule-matching to handle it like a domain name
            // Whereas the IdnMapper does not do this so the rule-matching fails because the 'domain name' is just a single word with no dot.
            this.PerformParsingCheck("12345", "48.57", null);
        }

        [TestMethod]
        public void DashBasedVariationsValid()
        {
            // Adding sub domains to these examples demonstrates how the Uri validation behaves in an unexpected way.
            // For example "sub.-example.com" is valid but "double.sub.-example.com" is not.

            this.PerformParsingCheck("-example.com", "-example.com", "-example.com");
            this.PerformParsingCheck("example.-com", "example.-com", "example.-com");
            this.PerformParsingCheck("example.com-", "example.com-", "example.com-");
            this.PerformParsingCheck("example.-com-", "example.-com-", "example.-com-");

            this.PerformParsingCheck("sub.-example.com", "-example.com", "-example.com");
            this.PerformParsingCheck("sub.example.com-", "example.com-", "example.com-");

            this.PerformParsingCheck("double.sub.example.com-", "example.com-", "example.com-");
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void DashBasedVariationsInvalid()
        {
            // Adding sub domains to these examples demonstrates how the Uri validation behaves in an unexpected way.
            // For example "sub.-example.com" is valid but "double.sub.-example.com" is not.

            this.PerformParsingCheck("sub.example.-com", null, "example.-com");
            this.PerformParsingCheck("sub.example.-com-", null, "example.-com-");
            this.PerformParsingCheck("double.sub.-example.com", null, "-example.com");
            this.PerformParsingCheck("double.sub.example.-com", null, "example.-com");
            this.PerformParsingCheck("double.sub.example.-com-", null, "example.-com-");
        }

        private void PerformParsingCheck(string domain, string expectedRegistrableDomain_UriVersion, string expectedRegistrableDomain_IdnVersion)
        {
            this.PerformParsingCheckUsingParser(domain, expectedRegistrableDomain_UriVersion, this._parserUsingUriNormalization, "uri normalizing parser");
            this.PerformParsingCheckUsingParser(domain, expectedRegistrableDomain_IdnVersion, this._parserUsingIdnNormalization, "idn normalizing parser");
        }

        private void PerformParsingCheckUsingParser(string domain, string expectedRegistrableDomain, IDomainParser domainParser, string parserDescription)
        {
            var domainData = domainParser.Parse(domain);
            if (domainData == null)
            {
                Assert.IsNull(expectedRegistrableDomain, $"{parserDescription} produced null instead of {expectedRegistrableDomain} from {domain}");
            }
            else
            {
                Assert.AreEqual(expectedRegistrableDomain, domainData.RegistrableDomain, $"{parserDescription} produced {domainData.RegistrableDomain} instead of {expectedRegistrableDomain ?? "null" } from {domain}");
            }
        }
    }
}
