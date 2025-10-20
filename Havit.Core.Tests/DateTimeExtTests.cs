namespace Havit.Tests;

[TestClass]
public class DateTimeExtTests
{
	[TestMethod]
	public void DateTimeExt_Min_ThrowExceptionForEmptyParameters()
	{
		// Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			// Act
			DateTimeExt.Min();
		});
	}

	[TestMethod]
	public void DateTimeExt_Min()
	{
		// Act + Assert
		Assert.AreEqual(DateTime.MinValue, DateTimeExt.Min(DateTime.Now, DateTime.MinValue));
		Assert.AreEqual(DateTime.MinValue, DateTimeExt.Min(DateTime.MinValue, DateTime.Now));
		Assert.AreEqual(DateTime.MinValue, DateTimeExt.Min(DateTime.MinValue, DateTime.MinValue, DateTime.Now));
	}

	[TestMethod]
	public void DateTimeExt_Max_ThrowExceptionForEmptyParameters()
	{
		// Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			// Act
			DateTimeExt.Max();
		});
	}

	[TestMethod]
	public void DateTimeExt_Max()
	{
		// Act + Assert
		Assert.AreEqual(DateTime.MaxValue, DateTimeExt.Max(DateTime.Now, DateTime.MaxValue));
		Assert.AreEqual(DateTime.MaxValue, DateTimeExt.Max(DateTime.MaxValue, DateTime.Now));
		Assert.AreEqual(DateTime.MaxValue, DateTimeExt.Max(DateTime.MaxValue, DateTime.MaxValue, DateTime.Now));
	}
}
