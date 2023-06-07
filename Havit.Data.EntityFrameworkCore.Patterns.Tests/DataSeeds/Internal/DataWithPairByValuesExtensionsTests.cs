using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Internal.QueryHelpersTests;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataSeeds.Internal;

[TestClass]
public class DataWithPairByValuesExtensionsTests
{
	[TestMethod]
	public void DataWithPairByValuesExtensions_ThrowIfContainsDuplicates_ExceptionFormat()
	{
		// Arrange
		var dataWithPairByValues = new List<DataWithPairByValues<TestClass>>
		{
			new DataWithPairByValues<TestClass> { OriginalItem = new TestClass(), PairByValues = new PairByValues(new object[] { 123, "CodeX" }) },
			new DataWithPairByValues<TestClass> { OriginalItem = new TestClass(), PairByValues = new PairByValues(new object[] { 123, "CodeX" }) },
			new DataWithPairByValues<TestClass> { OriginalItem = new TestClass(), PairByValues = new PairByValues(new object[] { 234, null }) },
			new DataWithPairByValues<TestClass> { OriginalItem = new TestClass(), PairByValues = new PairByValues(new object[] { 234, null }) }
		};

		List<Expression<Func<TestClass, object>>> expressions = new List<Expression<Func<TestClass, object>>>
		{
			(TestClass tc) => tc.LanguageId,
			(TestClass tc) => tc.Code,
		};

		try
		{
			// Act
			dataWithPairByValues.ThrowIfContainsDuplicates("Duplicates:", expressions.ToPairByExpressionsWithCompilations());

			// Assert
			Assert.Fail("Očekávaná výjimka nebyla vyhozena.");
		}
		catch (InvalidOperationException exception)
		{
			Assert.AreEqual("Duplicates: (LanguageId: 123, Code: CodeX), (LanguageId: 234, Code: null).", exception.Message);
		}
	}

	private class TestClass
	{
		public int LanguageId { get; set; }
		public string Code { get; set; }
	}
}
