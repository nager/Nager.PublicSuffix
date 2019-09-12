using System.Threading;
using Xunit;

namespace Nager.PublicSuffix.UnitTest {

    public class AsyncAwaitTest {
        [Fact]
        public void CheckConstructorDeadlock () {
            var thread = new Thread (FileTldRuleProvider);
            thread.Start ();
            if (!thread.Join (30000)) {
                Assert.True (false, "Deadlock detected FileTldRuleProvider");
            }

            thread = new Thread (WebTldRuleProvider);
            thread.Start ();
            if (!thread.Join (30000)) {
                Assert.True (false, "Deadlock detected WebTldRuleProvider");
            }
        }

        private void FileTldRuleProvider () {
            //SynchronizationContext.SetSynchronizationContext (new DispatcherSynchronizationContext ());

            var domainParser = new DomainParser (new FileTldRuleProvider ("effective_tld_names.dat"));
            Assert.NotNull (domainParser);
        }

        private void WebTldRuleProvider () {
            //SynchronizationContext.SetSynchronizationContext (new DispatcherSynchronizationContext ());

            var domainParser = new DomainParser (new WebTldRuleProvider ());
            Assert.NotNull (domainParser);
        }
    }
}