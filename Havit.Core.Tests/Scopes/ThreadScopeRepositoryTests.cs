using Havit.Tests.Scopes.Instrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Havit.Tests.Scopes;

    [TestClass]
    public class ThreadScopeRepositoryTests
    {
        [TestMethod]
        public void ThreadScopeRepository_InstancePersistInCurrentThread()
        {
            // Arrange
            object instance1 = new object();

            using (new TestThreadScope(instance1))
            {
                // Act
                object instance2 = TestThreadScope.Current;

                // Assert
                Assert.AreSame(instance1, instance2);
            }
        }

        [TestMethod]
        public void ThreadScopeRepository_InstanceDoesNotFlowToNewTask()
        {
            // Arrange 
            object instance1 = new object();
            using (new TestThreadScope(instance1))
            {
                object instance2 = instance1;

                // Act
                Task.Factory.StartNew(() =>
                {
                    instance2 = TestThreadScope.Current;
                }).Wait();

                // Assert
                Assert.IsNull(instance2);
            }
        }

        [TestMethod]
        public async Task ThreadScopeRepository_InstanceDoesNotFlowToAsyncAwait()
        {
            // Test je spíše dokumentací stavu a chování, než předpisem chování.
            // Pokud by se přenášela hodnota přes async/await, bylo by to výhodou.

            // Arrange
            object instance1 = new object();
            using (new TestThreadScope(instance1, suppressDispose: true))
            {
                // Act
                await Task.Yield();
                object instance2 = TestThreadScope.Current;

                // Assert
                Assert.IsNull(instance2);
            }
            
        }

    }
