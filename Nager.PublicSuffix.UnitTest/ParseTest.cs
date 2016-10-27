using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class ParseTest
    {
        [TestMethod]
        public void ParseValidData1()
        {
            var lines = new string[] { "com", "uk", "co.uk" };

            var domainParser = new DomainParser();
            var tldRules = domainParser.ParseRules(lines);

            Assert.AreEqual("com", tldRules[0].Name);
            Assert.AreEqual("uk", tldRules[1].Name);
            Assert.AreEqual("co.uk", tldRules[2].Name);
        }

        [TestMethod]
        public void ParseValidData2()
        {
            var lines = new string[] { "com", "//this is a example comment", "uk", "co.uk" };

            var domainParser = new DomainParser();
            var tldRules = domainParser.ParseRules(lines);

            Assert.AreEqual("com", tldRules[0].Name);
            Assert.AreEqual("uk", tldRules[1].Name);
            Assert.AreEqual("co.uk", tldRules[2].Name);
        }
    }
}
