using Havit.Threading;

namespace Havit.Tests.Threading;

[TestClass]
public class CriticalSectionTests
{
	[TestMethod]
	public void CriticalSection_ExecuteAction_ShouldNotRunInParallel()
	{
		// smoke test

		// Arrange
		int shared = 0;
		CriticalSection<int> criticalSection = new CriticalSection<int>();

		// Act
		Parallel.For(0, 100000, i =>
		{
			criticalSection.ExecuteAction(1, () =>
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
		// Arrange
		CriticalSection<int> criticalSection = new CriticalSection<int>();

		// Precondition
		Assert.IsEmpty(criticalSection.CriticalSectionLocks.Keys);

		// Act
		Parallel.For(0, 1000, i =>
		{
			criticalSection.ExecuteAction(1, () =>
			{
				Assert.HasCount(1, criticalSection.CriticalSectionLocks.Keys); // Assert
			});
		});
	}

	[TestMethod]
	public void CriticalSection_GetCriticalSectionLock_ValueEquality()
	{
		// Arrange
		CriticalSection<string> criticalSection = new CriticalSection<string>();

		string lockValue1 = "ABC";
		string lockValue2 = "abc".ToUpper();

		// Preconditions
		Assert.IsEmpty(criticalSection.CriticalSectionLocks.Keys);
		Assert.AreNotSame(lockValue1, lockValue2);
		Assert.AreEqual(lockValue1, lockValue2);

		// Act
		var lock1 = criticalSection.GetCriticalSectionLock(lockValue1);
		var lock2 = criticalSection.GetCriticalSectionLock(lockValue2);

		// Assert
		Assert.AreSame(lock1, lock2);

		// Cleanup
		criticalSection.ReleaseCriticalSectionLock(lockValue1, lock1);
		criticalSection.ReleaseCriticalSectionLock(lockValue2, lock2);
		Assert.IsEmpty(criticalSection.CriticalSectionLocks.Keys);
	}

	[TestMethod]
	public void CriticalSection_ExecuteAction_CleansUnusedLocks()
	{
		// Arrange
		CriticalSection<int> criticalSection = new CriticalSection<int>();

		// Precondition
		Assert.IsEmpty(criticalSection.CriticalSectionLocks.Keys, "Precondition failed.");

		// Act
		criticalSection.ExecuteAction(1, () => { });

		// Assert
		Assert.IsEmpty(criticalSection.CriticalSectionLocks.Keys); // dojde k vyčištění?
	}

	[TestMethod]
	public async Task CriticalSection_ExecuteAction_Async_CleansUnusedLocks()
	{
		// Arrange
		CriticalSection<int> criticalSection = new CriticalSection<int>();

		// Precondition
		Assert.IsEmpty(criticalSection.CriticalSectionLocks.Keys, "Precondition failed.");

		// Act
		await criticalSection.ExecuteActionAsync(1, async () => { await Task.CompletedTask; });

		// Assert
		Assert.IsEmpty(criticalSection.CriticalSectionLocks.Keys); // dojde k vyčištění?
	}

}
