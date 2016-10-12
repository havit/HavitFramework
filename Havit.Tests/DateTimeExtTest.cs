using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests
{
	[TestClass]
	public class DateTimeExtTest
	{
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DateTimeExt_Min_ThrowExceptionForEmptyParameters()
		{
			// Act
			DateTimeExt.Min();
			
			// Assert by method attribute
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
		[ExpectedException(typeof(InvalidOperationException))]
		public void DateTimeExt_Max_ThrowExceptionForEmptyParameters()
		{
			// Act
			DateTimeExt.Max();

			// Assert by method attribute
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
}
