using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Tests.TimeServices
{
    [TestClass]
	public class WorkingDaysCalculatorTests
    {
		[TestMethod]
		public void WorkingDaysCalculator_CountBusinessDays_NoHolidaysIncludeEndDate()
		{
			// Arrange
			WorkingDaysCalculator target = new WorkingDaysCalculator(Mock.Of<IDateInfoProvider>());

			DateTime startDate = new DateTime(2007, 10, 08);
			DateTime endDate = new DateTime(2007, 10, 09);

			// Act
			var result = target.CountBusinessDays(startDate, endDate, includeEndDate: true);

			// Assert
			Assert.AreEqual(2, result);
		}

		[TestMethod]
		public void WorkingDaysCalculator_CountBusinessDays_SingleDayNoHolidaysIncludeEndDate()
		{
			// Arrange
			WorkingDaysCalculator target = new WorkingDaysCalculator(Mock.Of<IDateInfoProvider>());

			DateTime startDate = new DateTime(2007, 10, 08);
			DateTime endDate = new DateTime(2007, 10, 08);

			// Act
			var result = target.CountBusinessDays(startDate, endDate, includeEndDate: true);

			// Assert
			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void WorkingDaysCalculator_CountBusinessDays_SingleDayNoHolidaysExcludeEndDate()
		{
			// Arrange
			WorkingDaysCalculator target = new WorkingDaysCalculator(Mock.Of<IDateInfoProvider>());

			DateTime startDate = new DateTime(2007, 10, 08);
			DateTime endDate = new DateTime(2007, 10, 08);

			// Act
			var result = target.CountBusinessDays(startDate, endDate, includeEndDate: false);

			// Assert
			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void WorkingDaysCalculator_CountBusinessDays_TwoDatesNoHolidaysExcludeEndDate()
		{
			// Arrange
			WorkingDaysCalculator target = new WorkingDaysCalculator(Mock.Of<IDateInfoProvider>());

			DateTime startDate = new DateTime(2007, 10, 08);
			DateTime endDate = new DateTime(2007, 10, 09);

			// Act
			var result = target.CountBusinessDays(startDate, endDate, includeEndDate: false);

			// Assert
			Assert.AreEqual(1, result);
		}

		[TestMethod]
		public void WorkingDaysCalculator_CountBusinessDays_Weekend()
		{
			// Arrange
			WorkingDaysCalculator target = new WorkingDaysCalculator(Mock.Of<IDateInfoProvider>());

			DateTime startDate = new DateTime(2007, 10, 6);
			DateTime endDate = new DateTime(2007, 10, 7);

			// Act
			var result = target.CountBusinessDays(startDate, endDate, includeEndDate: true);

			// Assert
			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void WorkingDaysCalculator_CountBusinessDays_ReverseDates()
		{
			// Arrange
			WorkingDaysCalculator target = new WorkingDaysCalculator(Mock.Of<IDateInfoProvider>());

			DateTime startDate = new DateTime(2007, 10, 10);
			DateTime endDate = new DateTime(2007, 10, 8);

			// Act
			var result = target.CountBusinessDays(startDate, endDate, includeEndDate: true);

			// Assert
			Assert.AreEqual(-3, result);
		}

		[TestMethod]
		public void WorkingDaysCalculator_IsWeekend_IsTrueOnlyForWeekends()
		{
			WorkingDaysCalculator target = new WorkingDaysCalculator(Mock.Of<IDateInfoProvider>());

			Assert.IsFalse(target.IsWeekend(new DateTime(2015, 3, 2)));
			Assert.IsFalse(target.IsWeekend(new DateTime(2015, 3, 3)));
			Assert.IsFalse(target.IsWeekend(new DateTime(2015, 3, 4)));
			Assert.IsFalse(target.IsWeekend(new DateTime(2015, 3, 5)));
			Assert.IsFalse(target.IsWeekend(new DateTime(2015, 3, 6)));
			Assert.IsTrue(target.IsWeekend(new DateTime(2015, 3, 7)));
			Assert.IsTrue(target.IsWeekend(new DateTime(2015, 3, 8)));
		}

		[TestMethod]
		public void WorkingDaysCalculator_CountBusinessDays_WithHolidays()
		{
			var dateInfo = new Mock<IDateInfo>();
			dateInfo.Setup(di => di.IsHoliday).Returns(true);

			var dateInfoProvider = new Mock<IDateInfoProvider>();
			dateInfoProvider.Setup(p => p.GetDateInfo(new DateTime(2020, 05, 01))).Returns(dateInfo.Object);

			WorkingDaysCalculator target = new WorkingDaysCalculator(dateInfoProvider.Object);

			// act
			var result = target.CountBusinessDays(new DateTime(2020, 4, 27), new DateTime(2020, 5, 7), includeEndDate: true);

			// assert
			Assert.AreEqual(8, result);
		}
	}
}
