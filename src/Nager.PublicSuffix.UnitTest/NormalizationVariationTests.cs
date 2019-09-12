using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nager.PublicSuffix.Exceptions;
using Xunit;

namespace Nager.PublicSuffix.UnitTest {
    // Matching TLDs against entries in the PublicSuffix list requires punycode be translated into UTF8
    // This can be accomplised either by using the Uri class or the IdnMapping class.
    // Both methods are constrasted here.
    // These tests demonstrate how the Uri class is performing additional validation/transformation on the domain name
    // Consumers of the DomainParser who are realy interested in the use of the PublicSuffix list to
    // examine behaviour around TLDs and their presence/absence/rules may not wish the outcomes to be
    // influenced by other rules imposed by the Uri class
    public class NormalizationVariationTests {
        private DomainParser _parserUsingUriNormalization;
        private DomainParser _parserUsingIdnNormalization;

        public NormalizationVariationTests () {
            var rules = new List<TldRule> ();
            rules.Add (new TldRule ("com"));
            this._parserUsingUriNormalization = new DomainParser (rules, new UriNormalizer ());
            this._parserUsingIdnNormalization = new DomainParser (rules, new IdnMappingNormalizer ());
        }

        [Theory (Skip = "https://community.letsencrypt.org/t/underscore-in-subdomain-fails/31431")]
        [InlineData ("_abc.def.ghi.jkl.com")]
        [InlineData ("abc._def.ghi.jkl.com")]
        [InlineData ("def._ghi.jkl.com")]
        [Trait ("UriNormalization", "IdnNormalization")]
        public void Given_UnderscoreBasedVariations_When_SomeUnderscoreAddedToUrlAndItIsPassedToTheUriAndIdnUrlNormalizationParser_Then_ValidateAndReturnDomainName (string url) {
            // this test reveals the motivation for creating a different method of normalization because of the handling of underscore characters
            // by the Uri class.
            // Some online services require the creation of DNS CNAME records for "DomainKeys Identified Mail" (DKIM) which
            // may have values formatted like
            // mail-this-that-co-uk.dkim1._domainkey.firebasemail.com
            // selector1-this.co.uk._domainkey.that.onmicrosoft.com 
            // Attempting to use the DomainParser to validate that these relate to real TLDs does not work when using the Uri class
            Assert.Equal ("jkl.com", this._parserUsingUriNormalization.Get (url).RegistrableDomain);
            Assert.Equal ("jkl.com", this._parserUsingIdnNormalization.Get (url).RegistrableDomain);
        }

        [Theory (Skip = "https://community.letsencrypt.org/t/underscore-in-subdomain-fails/31431")]
        [InlineData ("abc.def._ghi.jkl.com")]
        [Trait ("UriNormalization", "IdnNormalization")]
        public void Given_UnderscoreBasedVariations_When_SomeUnderscoeAddedToUrlAndItIsPassedToTheUriAndIdnUrlNormalizationParser_Then_ThrowAnInvalidUrlException (string url) {
            // These domains are treated as invalid and produce null via the Uri normalization method.
            // def._ghi.jkl.com is valid but
            // abc.def._ghi.jkl.com is invalid.
            // It can't be correct that adding abc. in front of a valid domain makes it invalid
            Assert.Throws<InvalidUrlException> (() => this._parserUsingUriNormalization.Get (url).RegistrableDomain);
            Assert.Throws<InvalidUrlException> (() => this._parserUsingIdnNormalization.Get (url).RegistrableDomain);
        }

        [Fact]
        public void Given_ASimpleWordWithoutADomainExtension_When_ASimpleWordIsPassedToTheIdnUrlNormalizationParser_Then_ThrowAnInvalidUrlException () {
            Assert.Null (this._parserUsingUriNormalization.Get ("singleword"));
            Assert.Null (this._parserUsingIdnNormalization.Get ("singleword"));
        }

        [Fact] // (Skip = "Parsing decimal form is forbidden")]
        /*
        - RFC 3986 -  URI Generic Syntax - January 2005
        3.2.2.  Host
        The host subcomponent of authority is identified by an IP literal
        encapsulated within square brackets, an IPv4 address in dotted-
        decimal form, or a registered name.

        host        = IP-literal / IPv4address / reg-name
         */
        public void Given_DecimalFormattedUrl_When_NumberUriIsPassedToTheIdnUrlNormalizationParser_Then_Return44dot57 () {
            // Uri object transforms 12344 to 48.57 because 12345 = (48 * 256) + (57 * 1)
            // This creates two parts separated by a dot which allows the rule-matching to handle it like a domain name
            // Whereas the IdnMapper does not do this so the rule-matching fails because the 'domain name' is just a single word with no dot.s
            var registrableDomain = this._parserUsingIdnNormalization.Get ("12345").RegistrableDomain;
            Assert.Equal ("48.57", registrableDomain);
        }

        [Theory]

        [InlineData ("-example.com")]
        [InlineData ("example.-com")]
        [InlineData ("example.com-")]
        [InlineData ("example.-com-")]

        [InlineData ("sub.-example.com")]
        [InlineData ("sub.example.-com")]
        [InlineData ("sub.example.com-")]
        [InlineData ("sub.example.-com-")]

        [InlineData ("double.sub.-example.com")]
        [InlineData ("double.sub.example.-com")]
        [InlineData ("double.sub.example.com-")]
        [InlineData ("double.sub.example.-com-")]

        [Trait ("UriNormalization", "IdnNormalization")]
        public void Given_DashBasedVariations_When_TheDomainNameStartsOrEndsWithADashOrAnHyphen_Then_ThrowAnInvalidUrlException (string url) {
            //Domain names and extensions cannot begin or end with dash/hyphen
            Assert.Null (this._parserUsingUriNormalization.Get (url));
            Assert.Null (this._parserUsingIdnNormalization.Get (url));
        }
    }
}