using Havit.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Tests.Threading
{
	[TestClass]
	public class CriticalSectionTests
	{
		[TestMethod]
		public void CriticalSection_ExecuteAction_ShouldNotRunInParallel()
		{
			// smoke test

			// Arrange
			int shared = 0;

			// Act
			Parallel.For(0, 100000, i =>
			{
				CriticalSection.ExecuteAction(1, () =>
				{
					Assert.AreEqual(0, shared, "Došlo k paralelnímu vstupu do kritické sekce."); // Assert

					shared += 1;
					// something long running?
					shared -= 1;
				});
			});
		}

		[TestMethod]
		public void CriticalSection_ExecuteAction_UsesOnlyOneLockForOneValue()
		{
			// Precondition
			Assert.AreEqual(0, CriticalSection.CriticalSectionLocks.Keys.Count);

			// Act
			Parallel.For(0, 1000, i =>
			{
				CriticalSection.ExecuteAction(1, () =>
				{
					Assert.AreEqual(1, CriticalSection.CriticalSectionLocks.Keys.Count); // Assert
				});
			});
		}

		[TestMethod]
		public void CriticalSection_GetCriticalSectionLock_ValueEquality()
		{
			// arrange
			string lockValue1 = "ABC";
			string lockValue2 = "abc".ToUpper();
			
			// Preconditions
			Assert.AreEqual(0, CriticalSection.CriticalSectionLocks.Keys.Count);
			Assert.AreNotSame(lockValue1, lockValue2);
			Assert.AreEqual(lockValue1, lockValue2);

			// Act
			var lock1 = CriticalSection.GetCriticalSectionLock(lockValue1);
			var lock2 = CriticalSection.GetCriticalSectionLock(lockValue2);

			// assert
			Assert.AreSame(lock1, lock2);

			// cleanup
			CriticalSection.ReleaseCriticalSectionLock(lockValue1, lock1);
			CriticalSection.ReleaseCriticalSectionLock(lockValue2, lock2);
			Assert.AreEqual(0, CriticalSection.CriticalSectionLocks.Keys.Count);
		}

		[TestMethod]
		public void CriticalSection_ExecuteAction_CleansUnusedLocks()
		{
			// Precondition
			Assert.AreEqual(0, CriticalSection.CriticalSectionLocks.Keys.Count, "Precondition failed.");

			// Act
			CriticalSection.ExecuteAction(1, () => { });

			// Assert
			Assert.AreEqual(0, CriticalSection.CriticalSectionLocks.Keys.Count); // dojde k vyčištění?
		}

		[TestMethod]
		public async Task CriticalSection_ExecuteAction_Async_CleansUnusedLocks()
		{
			// Precondition
			Assert.AreEqual(0, CriticalSection.CriticalSectionLocks.Keys.Count, "Precondition failed.");

			// Act
			await CriticalSection.ExecuteActionAsync(1, async () => { await Task.CompletedTask; });

			// Assert
			Assert.AreEqual(0, CriticalSection.CriticalSectionLocks.Keys.Count); // dojde k vyčištění?
		}

	}
}
