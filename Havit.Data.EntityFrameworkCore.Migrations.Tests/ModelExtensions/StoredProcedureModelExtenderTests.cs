using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.ModelExtensions
{
	[TestClass]
	public class StoredProcedureModelExtenderTests
	{
		[DataTestMethod]
		[DataRow("CREATE PROCEDURE [dbo].[InvoiceItem_Calculate]", "InvoiceItem_Calculate")]
		[DataRow("CREATE PROCEDURE dbo.InvoiceItem_Calculate", "InvoiceItem_Calculate")]
		[DataRow("CREATE PROCEDURE [dbo].InvoiceItem_Calculate", "InvoiceItem_Calculate")]
		[DataRow("CREATE PROCEDURE dbo.[InvoiceItem_Calculate]", "InvoiceItem_Calculate")]
		[DataRow("CREATE PROCEDURE [InvoiceItem_Calculate]", "InvoiceItem_Calculate")]
		[DataRow("CREATE PROCEDURE InvoiceItem_Calculate", "InvoiceItem_Calculate")]
		public void StoredProcedureModelExtender_ParseProcedureName_Test(string createProcedureLine, string expectedProcedureName)
		{
			var modelExtender = new StoredProcedureModelExtender();
			string procedureName = modelExtender.ParseProcedureName(createProcedureLine);

			Assert.AreEqual(expectedProcedureName, procedureName);
		}
	}
}