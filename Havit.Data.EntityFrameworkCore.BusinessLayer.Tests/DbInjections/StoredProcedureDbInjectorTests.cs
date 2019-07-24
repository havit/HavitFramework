using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.StoredProcedures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.DbInjections
{
    [TestClass]
    public class StoredProcedureDbInjectorTests
    {
        [DataTestMethod]
        [DataRow("CREATE PROCEDURE [dbo].[InvoiceItem_Calculate]", "InvoiceItem_Calculate")]
        [DataRow("CREATE PROCEDURE dbo.InvoiceItem_Calculate", "InvoiceItem_Calculate")]
        [DataRow("CREATE PROCEDURE [dbo].InvoiceItem_Calculate", "InvoiceItem_Calculate")]
        [DataRow("CREATE PROCEDURE dbo.[InvoiceItem_Calculate]", "InvoiceItem_Calculate")]
        [DataRow("CREATE PROCEDURE [InvoiceItem_Calculate]", "InvoiceItem_Calculate")]
        [DataRow("CREATE PROCEDURE InvoiceItem_Calculate", "InvoiceItem_Calculate")]
        public void StoredProcedureDbInjector_ParseProcedureName_Test(string createProcedureLine, string expectedProcedureName)
        {
            var dbInjector = new StoredProcedureDbInjector();
            string procedureName = dbInjector.ParseProcedureName(createProcedureLine);

            Assert.AreEqual(expectedProcedureName, procedureName);
        }
    }
}