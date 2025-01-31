using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests;

[TestClass]
public class DateTimeExtensionsTests
{
	[TestMethod]
	public void DateTimeExtensions_GetRemainingToNext()
	{
		Assert.AreEqual(TimeSpan.FromSeconds(1), new DateTime(2020, 1, 1, 10, 14, 59).GetRemainingToNext(TimeSpan.FromMinutes(15)));
		Assert.AreEqual(TimeSpan.FromMinutes(15), new DateTime(2020, 1, 1, 10, 15, 00).GetRemainingToNext(TimeSpan.FromMinutes(15)));
		Assert.AreEqual(TimeSpan.FromMinutes(14), new DateTime(2020, 1, 1, 10, 16, 00).GetRemainingToNext(TimeSpan.FromMinutes(15)));
	}
}
