using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;

namespace Havit.Tests
{
	[TestClass]
	public class MathExtTests
	{
		[TestMethod]
		public void MathExt_IsInteger_ReturnsFalseForNull()
		{
			string text = null;

			bool expected = false;
			bool actual;

			actual = Havit.MathExt.IsInteger(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MathExt_IsInteger_ReturnsTrueForPositiveInteger()
		{
			string text = "156";

			bool expected = true;
			bool actual;

			actual = Havit.MathExt.IsInteger(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MathExt_IsInteger_ReturnsTrueForNegativeInteger()
		{
			string text = "-156";

			bool expected = true;
			bool actual;

			actual = Havit.MathExt.IsInteger(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MathExt_IsIntegerTest_ReturnsFalseForEmptyString()
		{
			string text = String.Empty;

			bool expected = false;
			bool actual;

			actual = Havit.MathExt.IsInteger(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MathExt_IsIntegerTest_ReturnsFalseForInvalidValue()
		{
			string text = "10.";

			bool expected = false;
			bool actual;

			actual = Havit.MathExt.IsInteger(text);

			Assert.AreEqual(expected, actual);
		}
	}
}
