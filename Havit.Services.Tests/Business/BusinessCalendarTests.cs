using Havit.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Havit.Services.Tests.Business
{
    /// <summary>
    /// Testy třídy BusinessCalendar.
    /// </summary>
    [TestClass]
	public class BusinessCalendarTests
	{
		[TestMethod]
		public void BusinessCalendar_CountBusinessDays_NoHolidaysIncludeEndDate()
		{
			// Arrange
			BusinessCalendar target = new BusinessCalendar();

			DateTime startDate = new DateTime(2007, 10, 08);
			DateTime endDate = new DateTime(2007, 10, 09);
			CountBusinessDaysOptions options = CountBusinessDaysOptions.IncludeEndDate;

			int expected = 2;
			int actual;

			// Act
			actual = target.CountBusinessDays(startDate, endDate, options);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void BusinessCalendar_CountBusinessDays_SingleDayNoHolidaysIncludeEndDate()
		{
			// Arrange
			BusinessCalendar target = new BusinessCalendar();

			DateTime startDate = new DateTime(2007, 10, 08);
			DateTime endDate = new DateTime(2007, 10, 08);
			CountBusinessDaysOptions options = CountBusinessDaysOptions.IncludeEndDate;

			int expected = 1;
			int actual;

			// Act
			actual = target.CountBusinessDays(startDate, endDate, options);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void BusinessCalendar_CountBusinessDays_SingleDayNoHolidaysExcludeEndDate()
		{
			// Arrange
			BusinessCalendar target = new BusinessCalendar();

			DateTime startDate = new DateTime(2007, 10, 08);
			DateTime endDate = new DateTime(2007, 10, 08);
			CountBusinessDaysOptions options = CountBusinessDaysOptions.ExcludeEndDate;

			int expected = 0;
			int actual;

			// Act
			actual = target.CountBusinessDays(startDate, endDate, options);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void BusinessCalendar_CountBusinessDays_TwoDatesNoHolidaysExcludeEndDate()
		{
			// Arrange
			BusinessCalendar target = new BusinessCalendar();

			DateTime startDate = new DateTime(2007, 10, 08);
			DateTime endDate = new DateTime(2007, 10, 09);
			CountBusinessDaysOptions options = CountBusinessDaysOptions.ExcludeEndDate;

			int expected = 1;
			int actual;

			// Act
			actual = target.CountBusinessDays(startDate, endDate, options);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void BusinessCalendar_CountBusinessDays_Weekend()
		{
			// Arrange
			BusinessCalendar target = new BusinessCalendar();

			DateTime startDate = new DateTime(2007, 10, 6);
			DateTime endDate = new DateTime(2007, 10, 7);
			CountBusinessDaysOptions options = CountBusinessDaysOptions.IncludeEndDate;

			int expected = 0;
			int actual;

			// Act
			actual = target.CountBusinessDays(startDate, endDate, options);
			
			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void BusinessCalendar_CountBusinessDays_ReverseDates()
		{
			// Arrange
			BusinessCalendar target = new BusinessCalendar();

			DateTime startDate = new DateTime(2007, 10, 10);
			DateTime endDate = new DateTime(2007, 10, 8);
			CountBusinessDaysOptions options = CountBusinessDaysOptions.IncludeEndDate;

			int expected = -3;
			int actual;

			// Act
			actual = target.CountBusinessDays(startDate, endDate, options);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void BusinessCalendar_IsWeekend_IsTrueOnlyForWeekends()
		{
			BusinessCalendar target = new BusinessCalendar();

			Assert.IsFalse(target.IsWeekend(new DateTime(2015, 3, 2)));
			Assert.IsFalse(target.IsWeekend(new DateTime(2015, 3, 3)));
			Assert.IsFalse(target.IsWeekend(new DateTime(2015, 3, 4)));
			Assert.IsFalse(target.IsWeekend(new DateTime(2015, 3, 5)));
			Assert.IsFalse(target.IsWeekend(new DateTime(2015, 3, 6)));
			Assert.IsTrue(target.IsWeekend(new DateTime(2015, 3, 7)));
			Assert.IsTrue(target.IsWeekend(new DateTime(2015, 3, 8)));
		}
	}
}
