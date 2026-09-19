#if NET6_0_OR_GREATER
namespace Havit.Tests;

[TestClass]
public class DateOnlyRangeTests
{
	[TestMethod]
	[DataRow(2024, 2, 29, 2024, 3, 1)]
	[DataRow(2024, 12, 31, 2025, 1, 1)]
	[DataRow(2025, 1, 1, 2024, 12, 31)]
	public void DateOnlyRange_PreservesEndpoints(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
	{
		DateOnly start = new DateOnly(startYear, startMonth, startDay);
		DateOnly end = new DateOnly(endYear, endMonth, endDay);

		DateOnlyRange range = new DateOnlyRange(start, end);

		Assert.AreEqual(start, range.StartDate);
		Assert.AreEqual(end, range.EndDate);
	}

	[TestMethod]
	public void DateOnlyRange_OpenStart()
	{
		DateOnly end = new DateOnly(2024, 2, 29);
		DateOnlyRange range = new DateOnlyRange(null, end);

		Assert.IsNull(range.StartDate);
		Assert.AreEqual(end, range.EndDate);
	}

	[TestMethod]
	public void DateOnlyRange_OpenEnd()
	{
		DateOnly start = new DateOnly(2024, 12, 31);
		DateOnlyRange range = new DateOnlyRange(start, null);

		Assert.AreEqual(start, range.StartDate);
		Assert.IsNull(range.EndDate);
	}

	[TestMethod]
	public void DateOnlyRange_DefaultIsEmpty()
	{
		DateOnlyRange range = default;

		Assert.IsNull(range.StartDate);
		Assert.IsNull(range.EndDate);
		Assert.AreEqual(new DateOnlyRange(null, null), range);
		Assert.AreEqual(new DateOnlyRange(), range);
	}

	[TestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void DateOnlyRange_ValueEquality(bool hasStart, bool hasEnd)
	{
		DateOnly? start = hasStart ? new DateOnly(2024, 2, 29) : null;
		DateOnly? end = hasEnd ? new DateOnly(2025, 1, 1) : null;
		DateOnlyRange range = new DateOnlyRange(start, end);
		DateOnlyRange equalRange = new DateOnlyRange(start, end);
		DateOnlyRange differentStart = new DateOnlyRange(new DateOnly(2024, 3, 1), end);
		DateOnlyRange differentEnd = new DateOnlyRange(start, new DateOnly(2025, 1, 2));

		Assert.AreEqual(range, equalRange);
		Assert.IsTrue(range == equalRange);
		Assert.AreEqual(range.GetHashCode(), equalRange.GetHashCode());
		Assert.AreNotEqual(range, differentStart);
		Assert.IsTrue(range != differentEnd);
	}

	[TestMethod]
	public void DateOnlyRange_WithChangesEndpointsWithoutChangingOriginal()
	{
		DateOnly start = new DateOnly(2024, 2, 29);
		DateOnly end = new DateOnly(2025, 1, 1);
		DateOnlyRange original = new DateOnlyRange(start, end);

		DateOnlyRange changedStart = original with { StartDate = end.AddDays(1) };
		DateOnlyRange changedEnd = original with { EndDate = null };
		DateOnlyRange filledEnd = changedEnd with { EndDate = end };

		Assert.AreEqual(new DateOnlyRange(end.AddDays(1), end), changedStart);
		Assert.AreEqual(new DateOnlyRange(start, null), changedEnd);
		Assert.AreEqual(original, filledEnd);
		Assert.AreEqual(new DateOnlyRange(start, end), original);
	}

	[TestMethod]
	public void DateOnlyRange_Deconstruction()
	{
		DateOnly end = new DateOnly(2024, 2, 29);
		DateOnlyRange range = new DateOnlyRange(null, end);

		(DateOnly? startDate, DateOnly? endDate) = range;

		Assert.IsNull(startDate);
		Assert.AreEqual(end, endDate);
	}
}
#endif
