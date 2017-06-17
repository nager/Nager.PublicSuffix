using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Windows.Threading;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class DomainParserTest
    {
        [TestMethod]
        public void CheckConstructorDeadlock()
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

            var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"));

            Assert.IsNotNull(domainParser);
        }
    }
}
