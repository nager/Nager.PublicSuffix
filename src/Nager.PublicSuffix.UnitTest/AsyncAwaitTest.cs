using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Windows.Threading;

namespace Nager.PublicSuffix.UnitTest
{
    [TestClass]
    public class AsyncAwaitTest
    {
        [TestMethod]
        public void CheckConstructorDeadlock()
        {
            var thread = new Thread(FileTldRuleProvider);
            thread.Start();
            if (!thread.Join(30000))
            {
                Assert.Fail("Deadlock detected FileTldRuleProvider");
            }

            thread = new Thread(WebTldRuleProvider);
            thread.Start();
            if (!thread.Join(30000))
            {
                Assert.Fail("Deadlock detected WebTldRuleProvider");
            }
        }

        private void FileTldRuleProvider()
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

            var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"));
            Assert.IsNotNull(domainParser);
        }

        private void WebTldRuleProvider()
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

            var domainParser = new DomainParser(new WebTldRuleProvider());
            Assert.IsNotNull(domainParser);
        }
    }
}
