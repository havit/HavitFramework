using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataSeeds.Internal
{
	[TestClass]
	public class PairByValuesTests
	{
		[TestMethod]
		public void PairByValues_Equals_ReturnsTrueWhenEquals()
		{
			// Arrange
			var item1 = new PairByValues(new object[] { 1, 5.0, "test", true } );
			var item2 = new PairByValues(new object[] { 1, 5.0, "test", true } );

			// Act + Assert
			Assert.IsTrue(item1.Equals(item2));
		}

		[TestMethod]
		public void PairByValues_Equals_ReturnsFalseWhenNotEqual()
		{
			// Arrange
			var item1 = new PairByValues(new object[] { 1, 5.0, "test", true });
			var item2 = new PairByValues(new object[] { 1, 5.0, "test", false });

			// Act + Assert
			Assert.IsFalse(item1.Equals(item2));
		}

		[TestMethod]
		public void PairByValues_Equals_ReturnsFalseWhenNotSameDataCount()
		{
			// Arrange
			var item1 = new PairByValues(new object[] { 1, 5.0, "test", true });
			var item2 = new PairByValues(new object[] { 1, 5.0, "test" });

			// Act + Assert
			Assert.IsFalse(item1.Equals(item2));
		}

		[TestMethod]
		public void PairByValues_ToString_UsesBracketForMultipleValues()
		{
			// Arrange
			var item = new PairByValues(new object[] { 1, "test", true });

			// Act + Assert
			Assert.AreEqual("(1, test, True)", item.ToString());
		}

		[TestMethod]
		public void PairByValues_ToString_DoesNotUseBracketForSingleValue()
		{
			// Arrange
			var item = new PairByValues(new object[] { 1 });

			// Act + Assert
			Assert.AreEqual("1", item.ToString());
		}

        [TestMethod]
        public void PairByValues_ToString_SupportsNullInData()
        {
            // Arrange + Act
            string result = new PairByValues(new object[] { null }).ToString();
            
            // Assert
            Assert.AreEqual("null", result);
        }

        /// <summary>
        /// Bug 43592: Seedování dat se složeným klíčem, pokud je jedna z hodnot null
        /// </summary>
        [TestMethod]
        public void PairByValues_GetHashCode_SupportsNullInData()
        {
            // Arrange + Act
            int hashCode = new PairByValues(new object[] { null }).GetHashCode();

            // Assert
            // no exception was thown
        }

    }
}
