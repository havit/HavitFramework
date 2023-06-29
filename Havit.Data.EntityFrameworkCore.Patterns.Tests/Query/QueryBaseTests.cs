using System.Linq;
using Havit.Data.EntityFrameworkCore.Patterns.QueryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.AutronicShop.DataLayer.Tests.Query;

[TestClass]
public class QueryBaseTests
{
	[TestMethod]
	[DataRow(0, null, 0, 0, false)] // není stránkování, máme všechna data, nechceme volat bezparametrický Count.
	[DataRow(0, null, 10, 10, false)] // není stránkování, máme všechna data, nechceme volat bezparametrický Count.
	[DataRow(10, null, 9, 19, false)] // není stránkování (byť nezačínáme od začátku), máme všechna data (od daného startIndexu), nechceme volat bezparametrický Count.
	[DataRow(10, null, 10, 20, false)] // není stránkování (byť nezačínáme od začátku), máme všechna data (od daného startIndexu), nechceme volat bezparametrický Count.
	[DataRow(10, null, 100, 110, false)] // není stránkování (byť nezačínáme od začátku), máme všechna data (od daného startIndexu), nechceme volat bezparametrický Count.
	[DataRow(0, 10, 0, 0, false)] // stránkování + jsme na začátku, nejsou žádná data, nepotřebujeme volat bezparametrický Count.
	[DataRow(0, 10, 9, 9, false)] // stránkování + jsme na začátku, dat je méně, než je velikost stránky, nepotřebujeme volat bezparametrický Count.
	[DataRow(0, 10, 10, 999, true)] // stránkování + jsme na začátku, dat je stejně, jako je velikost stránky, MUSÍME volat bezparametrický Count (nevíme, jaká data následují).
	[DataRow(0, 10, 100, 999, true)] // stránkování + jsme na začátku, dat je více, něž je velikost stránky, jde o chybu, budeme volat bezparametrický Count (pro chybu nedokážeme o datech nic říct).
	[DataRow(10, 10, 0, 999, true)] // stránkování + nejsme na začátku, nejsou žádná data pro danou stránku, MUSÍME volat bezparametrický Count (dat je méně, než na jaké stránce jsme).
	[DataRow(10, 10, 9, 19, false)] // stránkování + nejsme na začátku, dat je méně, než je velikost stránky, nepotřebujeme volat bezparametrický Count.
	[DataRow(10, 10, 10, 999, true)] // stránkování + nejsme na začátku, dat je stejně, jako je velikost stránky, MUSÍME volat bezparametrický Count nevíme, jaká data následují).
	[DataRow(10, 10, 100, 999, true)] // stránkování + nejsme na začátku, dat je více, něž je velikost stránky, jde o chybu, budeme volat bezparametrický Count (pro chybu nedokážeme o datech nic říct).
	public void QueryBase_Count(int startIndex, int? count, int dataCount, int expectedResult, bool shouldCallParameterlessCount)
	{
		// Arrange
		Mock<QueryBase<int>> queryBaseMock = new Mock<QueryBase<int>>(MockBehavior.Loose)
		{
			CallBase = true
		};		

		var testQuery = queryBaseMock.Object;

		// Act
		// Testujeme Count, protože CountAsync vyžaduje IQueryable s IAsyncQueryProvider.
		var result = testQuery.Count(startIndex, count, dataCount);

		// Assert
		//Assert.AreEqual(shouldCallParameterlessCount, testQuery.QueryExecuted);
		Assert.AreEqual(expectedResult, result);
	}
}
