using Havit.Data.EntityFrameworkCore.Patterns.QueryServices;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Query;

[TestClass]
public class QueryBaseTests
{
	[TestMethod]
	[DataRow(0, null, 0, false)] // není stránkování, máme všechna data, nechceme volat bezparametrický Count.
	[DataRow(0, null, 10, false)] // není stránkování, máme všechna data, nechceme volat bezparametrický Count.
	[DataRow(10, null, 9, false)] // není stránkování (byť nezačínáme od začátku), máme všechna data (od daného startIndexu), nechceme volat bezparametrický Count.
	[DataRow(10, null, 10, false)] // není stránkování (byť nezačínáme od začátku), máme všechna data (od daného startIndexu), nechceme volat bezparametrický Count.
	[DataRow(10, null, 100, false)] // není stránkování (byť nezačínáme od začátku), máme všechna data (od daného startIndexu), nechceme volat bezparametrický Count.
	[DataRow(0, 10, 0, false)] // stránkování + jsme na začátku, nejsou žádná data, nepotřebujeme volat bezparametrický Count.
	[DataRow(0, 10, 9, false)] // stránkování + jsme na začátku, dat je méně, než je velikost stránky, nepotřebujeme volat bezparametrický Count.
	[DataRow(0, 10, 10, true)] // stránkování + jsme na začátku, dat je stejně, jako je velikost stránky, MUSÍME volat bezparametrický Count (nevíme, jaká data následují).
	[DataRow(0, 10, 100, true)] // stránkování + jsme na začátku, dat je více, něž je velikost stránky, jde o chybu, budeme volat bezparametrický Count (pro chybu nedokážeme o datech nic říct).
	[DataRow(10, 10, 0, true)] // stránkování + nejsme na začátku, nejsou žádná data pro danou stránku, MUSÍME volat bezparametrický Count (dat je méně, než na jaké stránce jsme).
	[DataRow(10, 10, 9, false)] // stránkování + nejsme na začátku, dat je méně, než je velikost stránky, nepotřebujeme volat bezparametrický Count.
	[DataRow(10, 10, 10, true)] // stránkování + nejsme na začátku, dat je stejně, jako je velikost stránky, MUSÍME volat bezparametrický Count nevíme, jaká data následují).
	[DataRow(10, 10, 100, true)] // stránkování + nejsme na začátku, dat je více, něž je velikost stránky, jde o chybu, budeme volat bezparametrický Count (pro chybu nedokážeme o datech nic říct).
	public void QueryBase_IsCallCountRequired(int startIndex, int? count, int dataCount, bool expectedIsCallCountRequired)
	{
		// Arrange
		// NOOP

		// Act
		bool isCallCountRequired = QueryBase<object>.IsCallCountRequired(startIndex, count, dataCount);

		// Assert
		Assert.AreEqual(expectedIsCallCountRequired, isCallCountRequired);
	}

	[TestMethod]
	public void QueryBase_GetDataFragment_ReturnsCorrentTotalCount()
	{
		// Arrange
		const int totalCount = 100;

		Mock<QueryBase<int>> m = new Mock<QueryBase<int>>(MockBehavior.Strict);
		m.Setup(m => m.Query()).Returns(() => Enumerable.Range(0, totalCount).AsQueryable());
		var queryBaseInstance = m.Object;

		// Act
		var dataFragment = queryBaseInstance.GetDataFragment(0, 20);

		// Assert
		Assert.AreEqual(totalCount, dataFragment.TotalCount);
	}
}
