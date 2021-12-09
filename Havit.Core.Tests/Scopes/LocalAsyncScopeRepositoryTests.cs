using Havit.Tests.Scopes.Instrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Tests.Scopes
{
    [TestClass]
    public class LocalAsyncScopeRepositoryTests
    {
        [TestMethod]
        public void LocalAsyncScopeRepository_InstancePersistInCurrentThread()
        {
            // Arrange
            object instance1 = new object();

            using (new TestAsyncLocalScope(instance1))
            {
                // Act
                object instance2 = TestAsyncLocalScope.Current;

                // Assert
                Assert.AreSame(instance1, instance2);
            }
        }

        [TestMethod]
        public void LocalAsyncScopeRepository_InstanceFlowsToNewTask()
        {
            // Test je spíše dokumentací stavu a chování..            

            // Arrange 
            object instance1 = new object();
            using (new TestAsyncLocalScope(instance1))
            {
                object instance2 = null;

                // Act
                Task.Factory.StartNew(() =>
                {
                    instance2 = TestAsyncLocalScope.Current;
                }).Wait();

                // Assert
                Assert.AreSame(instance1, instance2);
            }
        }

        [TestMethod]
        public async Task LocalAsyncScopeRepository_InstanceFlowsToAsyncAwait()
        {
            // Arrange
            object instance1 = new object();
            using (new TestAsyncLocalScope(instance1))
            {
                // Act
                await Task.Yield();
                object instance2 = TestAsyncLocalScope.Current;

                // Assert
                Assert.AreSame(instance1, instance2);
            }            
        }

        [TestMethod]
        public async Task LocalAsyncScopeRepository_ScopesInDifferentTasksAreIndependent()
        {
            // Arrange
            object task1InstanceToScope = new object();
            object task1InstanceFromScope = null;

            object task2InstanceToScope = new object();
            object task2InstanceFromScope = null;

            ManualResetEventSlim task1_NotifyReadyToReadFromScope_ManualResetEvent = new ManualResetEventSlim(false);
            ManualResetEventSlim task1_NotifyFinishedReadingFromScope_ManualResetEvent = new ManualResetEventSlim(false);
            ManualResetEventSlim task2_NotifyReadyToReadFromScope_ManualResetEvent = new ManualResetEventSlim(false);
            ManualResetEventSlim task2_NotifyFinishedReadingFromScope_ManualResetEvent = new ManualResetEventSlim(false);
                            
            ManualResetEventSlim orchestrator_NotifyStartReadingFromScope_ManualResetEvent = new ManualResetEventSlim(false);
            ManualResetEventSlim orchestrator_NotifyReadyForDisposeScpe_ManualResetEvent = new ManualResetEventSlim(false);

            // Act
            Task task1 = Task.Factory.StartNew(() =>
            {
                using (new TestAsyncLocalScope(task1InstanceToScope))
                {
                    task1_NotifyReadyToReadFromScope_ManualResetEvent.Set(); // řekneme, že jsme před čtením instance ze scopu
                    orchestrator_NotifyStartReadingFromScope_ManualResetEvent.Wait(); // počkáme, až budeme všichni před čtením instance ze scope (všichni budeme mít založen scope)

                    // Act 1
                    task1InstanceFromScope = TestAsyncLocalScope.Current;

                    task1_NotifyFinishedReadingFromScope_ManualResetEvent.Set(); // řekneme, že jsme za čtením instance
                    orchestrator_NotifyReadyForDisposeScpe_ManualResetEvent.Wait(); // počkáme, až budeme všichni za čtením instance (aby scope neskončil předčasně)
                }
            });

            Task task2 = Task.Factory.StartNew(() =>
            {
                using (new TestAsyncLocalScope(task2InstanceToScope))
                {
                    task2_NotifyReadyToReadFromScope_ManualResetEvent.Set();
                    orchestrator_NotifyStartReadingFromScope_ManualResetEvent.Wait();
                    
                    // Act 2
                    task2InstanceFromScope = TestAsyncLocalScope.Current;

                    task2_NotifyFinishedReadingFromScope_ManualResetEvent.Set();
                    orchestrator_NotifyReadyForDisposeScpe_ManualResetEvent.Wait();
                }
            });

            // počkáme, až budou všichni připraveni číst z TestAsyncLocalScope.Current (až budou mít všichni scope založen)
            task1_NotifyReadyToReadFromScope_ManualResetEvent.Wait();
            task2_NotifyReadyToReadFromScope_ManualResetEvent.Wait();

            // pustíme všechny ke čtení TestAsyncLocalScope.Current
            orchestrator_NotifyStartReadingFromScope_ManualResetEvent.Set();

            // počkáme, až budou mít všichni přečteno
            task1_NotifyFinishedReadingFromScope_ManualResetEvent.Wait();
            task2_NotifyFinishedReadingFromScope_ManualResetEvent.Wait();

            // pustíme všechny dál k dispose scopu
            orchestrator_NotifyReadyForDisposeScpe_ManualResetEvent.Set();

            await Task.WhenAll(task1, task2); // nepotřebujeme čekat, spíš pro kontrolu, že máme dobře práci s MenualResetEventy

            // Assert
            Assert.AreSame(task1InstanceToScope, task1InstanceFromScope);
            Assert.AreSame(task2InstanceToScope, task2InstanceFromScope);
        }

    }
}
