﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.PublicSuffix.Models;
using Nager.PublicSuffix.RuleParsers;
using System.Linq;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class TldRuleParserTest
    {
        [TestMethod]
        public void ParseValidData1()
        {
            var lines = new string[] { "com", "uk", "co.uk" };

            var ruleParser = new TldRuleParser(TldRuleDivisionFilter.All);
            var tldRules = ruleParser.ParseRules(lines).ToList();

            Assert.AreEqual("com", tldRules[0].Name);
            Assert.AreEqual("uk", tldRules[1].Name);
            Assert.AreEqual("co.uk", tldRules[2].Name);
        }

        [TestMethod]
        public void ParseValidData2()
        {
            var lines = new string[] { "com", "//this is a example comment", "uk", "co.uk" };

            var ruleParser = new TldRuleParser(TldRuleDivisionFilter.All);
            var tldRules = ruleParser.ParseRules(lines).ToList();

            Assert.AreEqual("com", tldRules[0].Name);
            Assert.AreEqual("uk", tldRules[1].Name);
            Assert.AreEqual("co.uk", tldRules[2].Name);
        }

        [TestMethod]
        public void ParseValidData3()
        {
            var lines = new string[] 
            {
                "example.above",
                "// ===BEGIN ICANN DOMAINS===",
                "uk", "co.uk",
                "// ===END ICANN DOMAINS===",
                "example.between",
                "// ===BEGIN PRIVATE DOMAINS===",
                "blogspot.com","no-ip.co.uk",
                "// ===END PRIVATE DOMAINS===",
                "example.after"
            };

            var ruleParser = new TldRuleParser(TldRuleDivisionFilter.All);
            var tldRules = ruleParser.ParseRules(lines).ToList();

            Assert.AreEqual("example.above", tldRules[0].Name);
            Assert.AreEqual(TldRuleDivision.Unknown, tldRules[0].Division);

            Assert.AreEqual("uk", tldRules[1].Name);
            Assert.AreEqual(TldRuleDivision.ICANN, tldRules[1].Division);
            Assert.AreEqual("co.uk", tldRules[2].Name);
            Assert.AreEqual(TldRuleDivision.ICANN, tldRules[2].Division);

            Assert.AreEqual("example.between", tldRules[3].Name);
            Assert.AreEqual(TldRuleDivision.Unknown, tldRules[3].Division);

            Assert.AreEqual("blogspot.com", tldRules[4].Name);
            Assert.AreEqual(TldRuleDivision.Private, tldRules[4].Division);
            Assert.AreEqual("no-ip.co.uk", tldRules[5].Name);
            Assert.AreEqual(TldRuleDivision.Private, tldRules[5].Division);

            Assert.AreEqual("example.after", tldRules[6].Name);
            Assert.AreEqual(TldRuleDivision.Unknown, tldRules[6].Division);
        }
    }
}
