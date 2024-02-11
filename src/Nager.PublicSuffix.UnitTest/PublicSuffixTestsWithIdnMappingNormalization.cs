﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.DomainNormalizers;
using Nager.PublicSuffix.RuleProviders;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class PublicSuffixTestsWithIdnMappingNormalization : PublicSuffixTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            var domainParser = new DomainParser(new FileTldRuleProvider("public_suffix_list.dat"), new IdnMappingDomainNormalizer());
            this._domainParser = domainParser;
        }
    }
}
