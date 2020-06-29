using Havit.Data.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Data.Tests.Threading
{
	[TestClass]
	public class DbLockedCriticalSectionTests
	{
		private DbLockedCriticalSectionOptions GetDbLockedCriticalSectionOptions() => new DbLockedCriticalSectionOptions
		{
			ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString
		};

		[TestMethod]
		public void DbLockedCriticalSection_ExecuteAction_CanGetLock()
		{
			// Arrange
			DbLockedCriticalSectionOptions options = GetDbLockedCriticalSectionOptions();
			DbLockedCriticalSection criticalSection = new DbLockedCriticalSection(options);

			// Act
			criticalSection.ExecuteAction("FakeResource", () => {
				// Assert
				Assert.AreEqual(DbLockedCriticalSection.SpGetAppLockResultCode.Locked, criticalSection.GetAppLockResultCode);
			});
		}

		[TestMethod]
		public void DbLockedCriticalSection_ExecuteAction_CanReleaseLock()
		{
			// Arrange
			DbLockedCriticalSectionOptions options = GetDbLockedCriticalSectionOptions();
			DbLockedCriticalSection criticalSection = new DbLockedCriticalSection(options);

			// Act
			criticalSection.ExecuteAction("FakeResource", () => { });

			// Assert
			Assert.AreEqual(DbLockedCriticalSection.SpReleaseAppLockResultCode.Released, criticalSection.ReleaseAppLockResultCode);
		}

		[TestMethod]
		public void DbLockedCriticalSection_ExecuteAction_CanWaitOnLockedResourceAndLockAfter()
		{
			// Arrange
			DbLockedCriticalSectionOptions options = GetDbLockedCriticalSectionOptions();
			DbLockedCriticalSection criticalSection = new DbLockedCriticalSection(options);
			DbLockedCriticalSection criticalSection2 = new DbLockedCriticalSection(options);

			// Act - simulation of parallel actions
			Action parallelAction1 = () => criticalSection.ExecuteAction("FakeResource", () =>
			{
				// simulation of doing something
				Thread.Sleep(1000);
			});

			Action parallelAction2 = () =>
			{
				// Wait for 500ms to ensure parallelAction1 is locked first.
				Thread.Sleep(500);
				criticalSection2.ExecuteAction("FakeResource", () => { });
			};

			Parallel.Invoke(parallelAction1, parallelAction2);

			// Assert
			Assert.AreEqual(DbLockedCriticalSection.SpGetAppLockResultCode.LockedAfterWaiting, criticalSection2.GetAppLockResultCode);
		}

		[TestMethod]
		[ExpectedException(typeof(DbLockedCriticalSectionException))]
		public void DbLockedCriticalSection_ExecuteAction_CanTimeout()
		{
			// Arrange
			DbLockedCriticalSectionOptions options1 = GetDbLockedCriticalSectionOptions();
			DbLockedCriticalSection criticalSection1 = new DbLockedCriticalSection(options1);
			DbLockedCriticalSectionOptions options2 = GetDbLockedCriticalSectionOptions();
			options2.LockTimeoutMs = 1;
			DbLockedCriticalSection criticalSection2 = new DbLockedCriticalSection(options2);

			// Act - simulation of parallel actions
			Action parallelAction1 = () => criticalSection1.ExecuteAction("FakeResource", () =>
			{
				// Simulation of doing something longer than lock timeout for next action.
				Thread.Sleep(1500);
			});

			Action parallelAction2 = () =>
			{
				// Wait for 500ms to ensure parallelAction1 is locked first.
				Thread.Sleep(500);
				criticalSection2.ExecuteAction("FakeResource", () => { });
			};

			try
			{
				Parallel.Invoke(parallelAction1, parallelAction2);
			}
			catch (AggregateException ex)
			{
				throw ex.InnerException; // Parallel.Invoke() aggregates exception -> need to rethrow inner exception for simulation purpose
			}

			// Assert
			// throws exception
		}

		[TestMethod]
		public void DbLockedCriticalSection_ExecuteAction_CanExecuteCascadeAction()
		{
			// Arrange
			DbLockedCriticalSectionOptions options = GetDbLockedCriticalSectionOptions();
			DbLockedCriticalSection criticalSection = new DbLockedCriticalSection(options);

			// Act - cascade
			criticalSection.ExecuteAction("FakeResourceA", () =>
			{
				// Assert
				Assert.AreEqual(DbLockedCriticalSection.SpGetAppLockResultCode.Locked, criticalSection.GetAppLockResultCode);
				criticalSection.ExecuteAction("FakeResourceB", () => {
					Assert.AreEqual(DbLockedCriticalSection.SpGetAppLockResultCode.Locked, criticalSection.GetAppLockResultCode);
				});

				Assert.AreEqual(DbLockedCriticalSection.SpReleaseAppLockResultCode.Released, criticalSection.ReleaseAppLockResultCode);
			});

			Assert.AreEqual(DbLockedCriticalSection.SpReleaseAppLockResultCode.Released, criticalSection.ReleaseAppLockResultCode);
		}

		[TestMethod]
		public async Task DbLockedCriticalSection_ExecuteActionAsync_CanGetLock()
		{
			// Arrange
			DbLockedCriticalSectionOptions options = GetDbLockedCriticalSectionOptions();
			DbLockedCriticalSection criticalSection = new DbLockedCriticalSection(options);

			// Act
			await criticalSection.ExecuteActionAsync("FakeResource", () =>
			{
				// Assert
				Assert.AreEqual(DbLockedCriticalSection.SpGetAppLockResultCode.Locked, criticalSection.GetAppLockResultCode);
				return Task.CompletedTask;
			});
		}

		[TestMethod]
		public async Task DbLockedCriticalSection_ExecuteActionAsync_CanReleaseLock()
		{
			// Arrange
			DbLockedCriticalSectionOptions options = GetDbLockedCriticalSectionOptions();
			DbLockedCriticalSection criticalSection = new DbLockedCriticalSection(options);

			// Act
			await criticalSection.ExecuteActionAsync("FakeResource", () => { return Task.CompletedTask; });

			// Assert
			Assert.AreEqual(DbLockedCriticalSection.SpReleaseAppLockResultCode.Released, criticalSection.ReleaseAppLockResultCode);
		}

        [TestMethod]
        public async Task DbLockedCriticalSection_ExecuteActionAsync_CanWaitOnLockedResourceAndLockAfter()
        {
            // Arrange
            DbLockedCriticalSectionOptions options = GetDbLockedCriticalSectionOptions();
            DbLockedCriticalSection criticalSection1 = new DbLockedCriticalSection(options);
            DbLockedCriticalSection criticalSection2 = new DbLockedCriticalSection(options);

            // Act - simulation of parallel actions
            _ = criticalSection1.ExecuteActionAsync("FakeResource", async () =>
            {
                // simulation of doing something
                await Task.Delay(1000);
            });
            await Task.Delay(500);
            await criticalSection2.ExecuteActionAsync("FakeResource", () => Task.CompletedTask);

            // Assert
            Assert.AreEqual(DbLockedCriticalSection.SpGetAppLockResultCode.LockedAfterWaiting, criticalSection2.GetAppLockResultCode);
        }

        [TestMethod]
		[ExpectedException(typeof(DbLockedCriticalSectionException))]
		public async Task DbLockedCriticalSection_ExecuteActionAsync_CanTimeout()
		{
			// Arrange
			DbLockedCriticalSectionOptions options1 = GetDbLockedCriticalSectionOptions();
			DbLockedCriticalSection criticalSection = new DbLockedCriticalSection(options1);
			DbLockedCriticalSectionOptions options2 = GetDbLockedCriticalSectionOptions();
			options2.LockTimeoutMs = 1;
			DbLockedCriticalSection criticalSection2 = new DbLockedCriticalSection(options2);

			// Act
			await criticalSection.ExecuteActionAsync("FakeResource", async () =>
			{
				await criticalSection2.ExecuteActionAsync("FakeResource", () => Task.CompletedTask);
			});

			// Assert
			// throws exception
		}

		[TestMethod]
		public async Task DbLockedCriticalSection_ExecuteActionAsync_CanExecuteCascadeAction()
		{
			// Arrange
			DbLockedCriticalSectionOptions options = GetDbLockedCriticalSectionOptions();
			DbLockedCriticalSection criticalSection = new DbLockedCriticalSection(options);

			// Act - cascade
			await criticalSection.ExecuteActionAsync("FakeResourceA", async () =>
			{
				// Assert
				Assert.AreEqual(DbLockedCriticalSection.SpGetAppLockResultCode.Locked, criticalSection.GetAppLockResultCode);
				await criticalSection.ExecuteActionAsync("FakeResourceB", () =>
				{
					Assert.AreEqual(DbLockedCriticalSection.SpGetAppLockResultCode.Locked, criticalSection.GetAppLockResultCode);
					return Task.CompletedTask;
				});

				Assert.AreEqual(DbLockedCriticalSection.SpReleaseAppLockResultCode.Released, criticalSection.ReleaseAppLockResultCode);
			});

			Assert.AreEqual(DbLockedCriticalSection.SpReleaseAppLockResultCode.Released, criticalSection.ReleaseAppLockResultCode);
		}
	}
}
