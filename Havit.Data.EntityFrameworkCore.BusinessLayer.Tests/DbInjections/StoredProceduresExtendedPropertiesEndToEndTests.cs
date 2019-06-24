using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections.StoredProcedures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.DbInjections
{
	public class StoredProceduresExtendedPropertiesEndToEndTests
    {
	    [TestClass]
	    public class StoredProcedureWithMsDescriptionExtendedProperty
		{
			[Attach(nameof(Invoice))]
			public class InvoiceStoredProcedures : StoredProcedureDbInjector
			{
				/// <summary>
				/// Calculates total amount.
				/// </summary>
				/// <returns>Total amount</returns>
				[MethodName(nameof(TotalAmount))]
				[Result(StoredProcedureResultType.DataTable)]
				[MethodAccessModifier("public")]
				public StoredProcedureDbInjection TotalAmount()
				{
					return new StoredProcedureDbInjection { CreateSql = "", ProcedureName = nameof(TotalAmount) };
				}
			}

			[Table("Dummy")]
			private class Invoice
			{
				public int Id { get; set; }

				public DateTime Created { get; set; }
			}

		    [TestMethod]
		    public void StoredProceduresDbInjections_EndToEnd_StoredProcedureWithMsDescriptionExtendedProperty()
		    {
			    var source = new EndToEndTestDbInjectionsDbContext<Invoice>(typeof(InvoiceStoredProcedures));
			    var model = source.Model;

			    IDictionary<string, string> extendedProperties = model.GetExtendedProperties();
			    Assert.AreEqual("Calculates total amount.", extendedProperties.FirstOrDefault(a => a.Key.EndsWith("MS_Description")).Value?.Trim());
		    }
		}

		/// <summary>
		/// Checks, whether XML comment actually yields SQL statement for adding extended property to stored procedure.
		/// </summary>
	    [TestClass]
	    public class StoredProcedureWithXmlCommentMsDescriptionAdded
	    {
		    [Attach(nameof(Invoice))]
		    public class InvoiceStoredProcedures : StoredProcedureDbInjector
		    {
			    /// <summary>
			    /// Calculates total amount.
			    /// </summary>
			    /// <returns>Total amount</returns>
			    [MethodName(nameof(TotalAmount))]
			    [Result(StoredProcedureResultType.DataTable)]
			    [MethodAccessModifier("public")]
			    public StoredProcedureDbInjection TotalAmount()
			    {
				    return new StoredProcedureDbInjection { CreateSql = $"CREATE PROCEDURE [{nameof(TotalAmount)}]", ProcedureName = nameof(TotalAmount) };
			    }
		    }

		    [Table("Dummy")]
		    private class Invoice
		    {
			    public int Id { get; set; }

			    public DateTime Created { get; set; }
		    }

		    [TestMethod]
		    public void StoredProceduresDbInjections_EndToEnd_StoredProcedureWithXmlCommentMsDescriptionAdded()
		    {
			    var source = new EndToEndTestDbContext<Invoice>();
			    var target = new EndToEndTestDbInjectionsDbContext<Invoice>(typeof(InvoiceStoredProcedures));

				var commands = source.Migrate(target);

				Assert.AreNotEqual(0, commands.Count);

			    var addExtendedPropertyCommands = commands.Where(c => c.CommandText.StartsWith("EXEC sys.sp_addextendedproperty @name=N'MS_Description'"));
			    var command = addExtendedPropertyCommands.FirstOrDefault(c => c.CommandText.EndsWith($"@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'{nameof(InvoiceStoredProcedures.TotalAmount)}'"));

				Assert.IsNotNull(command, $"MS_Description extended property for stored procedure '{nameof(InvoiceStoredProcedures.TotalAmount)}' is missing.");
				Assert.AreEqual("Calculates total amount.", Regex.Match(command.CommandText, "@value=N'(.*?)'", RegexOptions.Singleline).Groups[1].Value.Trim('\r', '\n', ' '));
		    }
	    }
    }
}