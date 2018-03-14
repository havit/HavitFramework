using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Havit.Tests.ComponentModel
{
	[TestClass]
	public class UniversalConverterTests
	{
		[TestMethod]
		public void UniversalConverter_TryConvertTo()
		{
			object result;
			bool success;

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo("A", typeof(int), out result);
			Assert.IsFalse(success);
			Assert.AreEqual(result, null);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo("5", typeof(int), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((int)result, 5);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo("5", typeof(int?), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((int?)result, 5);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo("5", typeof(decimal), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((decimal)result, 5M);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo("5", typeof(decimal?), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((decimal?)result, 5M);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(5, typeof(int), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((int)result, 5);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(5, typeof(int?), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((int?)result, 5);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(5, typeof(decimal), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((decimal)result, 5M);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(5, typeof(decimal?), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((decimal?)result, 5M);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(5M, typeof(int), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((int)result, 5);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(5M, typeof(int?), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((int?)result, 5);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(5M, typeof(decimal), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((decimal)result, 5M);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(5M, typeof(decimal?), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((decimal?)result, 5M);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(null, typeof(int?), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((int?)result, null);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo(null, typeof(decimal?), out result);
			Assert.IsTrue(success);
			Assert.AreEqual((decimal?)result, null);

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo("1/2/2013", typeof(DateTime), out result, CultureInfo.GetCultureInfo("en-US"));
			Assert.IsTrue(success);
			Assert.AreEqual((DateTime)result, new DateTime(2013, 1, 2));

			success = Havit.ComponentModel.UniversalTypeConverter.TryConvertTo("1/2/2013", typeof(DateTime), out result, CultureInfo.GetCultureInfo("en-GB"));
			Assert.IsTrue(success);
			Assert.AreEqual((DateTime)result, new DateTime(2013, 2, 1));

		}
	}
}
