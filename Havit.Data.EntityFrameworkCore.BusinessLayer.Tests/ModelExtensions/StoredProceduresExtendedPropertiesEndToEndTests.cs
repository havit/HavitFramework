using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties.Attributes;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ModelExtensions
{
	public class StoredProceduresExtendedPropertiesEndToEndTests
    {
	    [TestClass]
	    public class StoredProcedureWithMsDescriptionExtendedProperty
		{
			[Attach(nameof(Invoice))]
			public class InvoiceStoredProcedures : StoredProcedureModelExtender
			{
				/// <summary>
				/// Calculates total amount.
				/// </summary>
				/// <returns>Total amount</returns>
				[MethodName(nameof(TotalAmount))]
				[Result(StoredProcedureResultType.DataTable)]
				[MethodAccessModifier("public")]
				public StoredProcedureModelExtension TotalAmount()
				{
					return new StoredProcedureModelExtension { CreateSql = "", ProcedureName = nameof(TotalAmount) };
				}
			}

			[Table("Dummy")]
			private class Invoice
			{
				public int Id { get; set; }

				public DateTime Created { get; set; }
			}

		    [TestMethod]
		    public void StoredProcedureModelExtensions_EndToEnd_StoredProcedureWithMsDescriptionExtendedProperty()
		    {
			    var source = new EndToEndTestModelExtensionsDbContext<Invoice>(typeof(InvoiceStoredProcedures));
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
		    public class InvoiceStoredProcedures : StoredProcedureModelExtender
		    {
			    /// <summary>
			    /// Calculates total amount.
			    /// </summary>
			    /// <returns>Total amount</returns>
			    [MethodName(nameof(TotalAmount))]
			    [Result(StoredProcedureResultType.DataTable)]
			    [MethodAccessModifier("public")]
			    public StoredProcedureModelExtension TotalAmount()
			    {
				    return new StoredProcedureModelExtension { CreateSql = $"CREATE PROCEDURE [{nameof(TotalAmount)}]", ProcedureName = nameof(TotalAmount) };
			    }
		    }

		    [Table("Dummy")]
		    private class Invoice
		    {
			    public int Id { get; set; }

			    public DateTime Created { get; set; }
		    }

		    [TestMethod]
		    public void StoredProcedureModelExtensions_EndToEnd_StoredProcedureWithXmlCommentMsDescriptionAdded()
		    {
			    var source = new EndToEndTestModelExtensionsDbContext<Invoice>();
			    var target = new EndToEndTestModelExtensionsDbContext<Invoice>(typeof(InvoiceStoredProcedures));

				var commands = source.Migrate(target);

				Assert.AreNotEqual(0, commands.Count);

			    var addExtendedPropertyCommands = commands.Where(c => c.CommandText.StartsWith("EXEC sys.sp_addextendedproperty @name=N'MS_Description'"));
			    var command = addExtendedPropertyCommands.FirstOrDefault(c => c.CommandText.EndsWith($"@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'{nameof(InvoiceStoredProcedures.TotalAmount)}'"));

				Assert.IsNotNull(command, $"MS_Description extended property for stored procedure '{nameof(InvoiceStoredProcedures.TotalAmount)}' is missing.");
				Assert.AreEqual("Calculates total amount.", Regex.Match(command.CommandText, "@value=N'(.*?)'", RegexOptions.Singleline).Groups[1].Value.Trim('\r', '\n', ' '));
		    }
	    }

        /// <summary>
        /// Checks, whether dropping procedure generates IF EXISTS statements for dropping extended properties (workaround for Bug 45536)
        /// </summary>
        [TestClass]
        public class DeletingStoredProcedureIfExistsForExtendedProperties
        {
            public class InvoiceStoredProcedures : StoredProcedureModelExtender
            {
                /// <summary>
                /// Calculates total amount.
                /// </summary>
                /// <returns>Total amount</returns>
                [MethodName(nameof(TotalAmount))]
                [Result(StoredProcedureResultType.DataTable)]
                [MethodAccessModifier("public")]
                public StoredProcedureModelExtension TotalAmount()
                {
                    return new StoredProcedureModelExtension { CreateSql = $"CREATE PROCEDURE [{nameof(TotalAmount)}]", ProcedureName = nameof(TotalAmount) };
                }
            }

            [Table("Dummy")]
            private class Entity
            {
                public int Id { get; set; }
            }

            [TestMethod]
            public void StoredProcedureModelExtensions_DroppingStoredProcedureWithExtendedProperty_DroppingExtendedPropertyHasIfObjectIdCheck()
            {
                var source = new EndToEndTestModelExtensionsDbContext<Entity>(typeof(InvoiceStoredProcedures));
                var target = new EndToEndTestModelExtensionsDbContext<Entity>();

                var commands = source.Migrate(target);

                Assert.AreNotEqual(0, commands.Count);

                var dropExtendedPropertyCommand = commands.First(c => c.CommandText.Contains("EXEC sys.sp_dropextendedproperty @name=N'MS_Description'"));

                Assert.AreEqual(@"IF OBJECT_ID(N'[dbo].[TotalAmount]') IS NOT NULL
BEGIN
    EXEC sys.sp_dropextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'PROCEDURE', @level1name=N'TotalAmount'
END
", dropExtendedPropertyCommand.CommandText);
            }
        }

		/// <summary>
		/// Checks, whether order of migration commands is correct, i.e. first create stored procedure, then add extended properties.
		/// </summary>
		[TestClass]
		public class StoredProcedureWithExtendedPropertiesOrderOfMigrationCommandsIsCorrect
		{
			[Attach(nameof(Invoice))]
			public class InvoiceStoredProcedures : StoredProcedureModelExtender
			{
				[MethodName(nameof(TotalAmount))]
				[Result(StoredProcedureResultType.DataTable)]
				[MethodAccessModifier("public")]
				public StoredProcedureModelExtension TotalAmount()
				{
					return new StoredProcedureModelExtension { CreateSql = $"CREATE PROCEDURE [{nameof(TotalAmount)}]", ProcedureName = nameof(TotalAmount) };
				}
			}

			[Table("Dummy")]
			private class Invoice
			{
				public int Id { get; set; }

				public DateTime Created { get; set; }
			}

			[TestMethod]
			public void StoredProcedureModelExtensions_EndToEnd_StoredProcedureWithExtendedPropertiesOrderOfMigrationCommandsIsCorrect()
			{
				var source = new EndToEndTestModelExtensionsDbContext<Invoice>();
				var target = new EndToEndTestModelExtensionsDbContext<Invoice>(typeof(InvoiceStoredProcedures));

				var commands = source.Migrate(target).ToList();

				Assert.AreNotEqual(0, commands.Count);

				// currently checking only whether CREATE PROCEDURE command is before EXEC sys.sp_addextendedproperty commands
				// (should make it less prone it breakage, when more extensions (that modify generated migration commands) are added to DbContext
				int createProcedureCommandIndex = commands.FindIndex(c => c.CommandText.StartsWith("CREATE PROCEDURE"));
                int[] addExtendedPropertyCommandIndexes = commands
                    .Select((command, index) => new { command, index })
                    .Where(a => a.command.CommandText.StartsWith("EXEC sys.sp_addextendedproperty"))
                    .Select(a => a.index)
                    .ToArray();

				Assert.AreNotEqual(-1, createProcedureCommandIndex);

				Assert.IsTrue(addExtendedPropertyCommandIndexes.All(index => createProcedureCommandIndex < index));
			}
		}
	}
}