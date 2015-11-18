using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using Havit.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbContext = Havit.Data.Entity.DbContext;

namespace Havit.Data.Entity.Test
{
	[TestClass]
	public class DbEntityValidationExceptionExtensionsTest
	{
		#region DbEntityValidationExceptionExtensionsTestDbContext (internal class)
		internal class DbEntityValidationExceptionExtensionsTestDbContext : DbContext
		{			
			public DbSet<DataClass> DataClasses { get; set; }

			public DbEntityValidationExceptionExtensionsTestDbContext() : base("Havit.Data.Entity6.Test")
			{	
				Database.SetInitializer(new DropCreateDatabaseAlways<DbEntityValidationExceptionExtensionsTestDbContext>());
			}

			/// <summary>
			/// Nested data class.
			/// </summary>
			internal class DataClass
			{
				public int Id { get; set; }
			}
		}
		#endregion

		[TestMethod]
		public void DbEntityValidationException_FormatErrorMessage_FormatsValidationResults()
		{
			// arrange
			var dbContext = new DbEntityValidationExceptionExtensionsTestDbContext();
			var entry = dbContext.Entry(new DbEntityValidationExceptionExtensionsTestDbContext.DataClass());

			var error1 = new DbValidationError("Property1", "Property1 must have a value.");
			var error2 = new DbValidationError("Property2", "Property2 must have a value.");
			DbEntityValidationResult validationResult = new DbEntityValidationResult(entry, new[] { error1, error2 });
			DbEntityValidationException exception = new DbEntityValidationException("", new[] { validationResult });

			// act
			string formattedMessage = exception.FormatErrorMessage();

			// assert
			Assert.AreEqual("Property1 must have a value. Property2 must have a value.", formattedMessage);
		}

		[TestMethod]
		public void DbEntityValidationException_FormatErrorMessage_ReturnsMessageWhenNoValidationResult()
		{
			// arrange
			DbEntityValidationException exception = new DbEntityValidationException("Message.");

			// act
			string formattedMessage = exception.FormatErrorMessage();

			// assert
			Assert.AreEqual("Message.", formattedMessage);
		}

		[TestMethod]
		public void DbEntityValidationException_FormatErrorMessage_IgnoresMessageWhenValidationResultExists()
		{
			// arrange
			var dbContext = new DbEntityValidationExceptionExtensionsTestDbContext();
			var entry = dbContext.Entry(new DbEntityValidationExceptionExtensionsTestDbContext.DataClass());

			var error1 = new DbValidationError("Property1", "Property1 must have a value.");
			var error2 = new DbValidationError("Property2", "Property2 must have a value.");
			DbEntityValidationResult validationResult = new DbEntityValidationResult(entry, new[] { error1, error2 });
			DbEntityValidationException exception = new DbEntityValidationException("Message.", new[] { validationResult });

			// act
			string formattedMessage = exception.FormatErrorMessage();

			// assert
			Assert.IsFalse(formattedMessage.Contains("Message."));
		}

	}
}
