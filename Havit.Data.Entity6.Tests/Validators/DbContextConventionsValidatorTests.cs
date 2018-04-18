using Havit.Data.Entity.Tests.Validators.Infrastructure;
using Havit.Data.Entity.Tests.Validators.Infrastructure.Model;
using Havit.Data.Entity.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Havit.Data.Entity.Mapping.Internal;

namespace Havit.Data.Entity.Tests.Validators
{
	[TestClass]
	public class DbContextConventionsValidatorTests
	{
		[TestMethod]
		public void DbContextConventionsValidator_CheckPrimaryKey_ReportsMorePrimaryKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKey(modelValidatingDbContext.Db(typeof(MoreInvalidKeysClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("only one is expected")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckPrimaryKey_ReportsNonIntKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKey(modelValidatingDbContext.Db(typeof(StringIdClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("type int (System.Int32) is expected")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckPrimaryKey_DoesNotReportOnePrimaryKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsOneCorrectKeyClass = modelValidator.CheckPrimaryKey(modelValidatingDbContext.Db(typeof(OneCorrectKeyClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsOneCorrectKeyClass.Any());
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckIdNamingConvention_ReportsCapitalId()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckIdNamingConvention(modelValidatingDbContext.Db(typeof(CapitalIDClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("which ends with")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckIdNamingConvention_DoesNotReportPascalCaseId()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckIdNamingConvention(modelValidatingDbContext.Db(typeof(OneCorrectKeyClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckStringMaxLengthConvention_ReportsMissingMaxLengthAttribute()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringMaxLengthConvention(modelValidatingDbContext.Db(typeof(NoMaxLengthAttributeClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("MaxLengthAttribute on property is expected")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckStringMaxLengthConvention_DoesNotReportMissingAttributeWithPositiveValue()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringMaxLengthConvention(modelValidatingDbContext.Db(typeof(MaxLengthAttributeWithPositiveValueClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckNestedMembers_ReportsNestedClass()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckNestedMembers(modelValidatingDbContext.Db(typeof(WithNestedClassClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("unsupported nested type")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckNestedMembers_ReportsNonEntryEnum()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckNestedMembers(modelValidatingDbContext.Db(typeof(WithNestedEnumOtherClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("unsupported nested type")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckNestedMembers_DoesNotReportEntryEnum()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckNestedMembers(modelValidatingDbContext.Db(typeof(WithNestedEnumEntryClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckForeignKeyForNavigationProperties_ReportsNavigationPropertyWithoutForeignKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckForeignKeyForNavigationProperties(modelValidatingDbContext.Db(typeof(NavigationPropertyWithoutForeignKeyClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("no foreign key")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckForeignKeyForNavigationProperties_DoesNotReportNavigationPropertyWithForeignKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckForeignKeyForNavigationProperties(modelValidatingDbContext.Db(typeof(NavigationPropertyWithForeignKeyClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckSymbolVsPrimaryKeyForEntries_DoesNotReportNavigationPropertyWithForeignKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsEntryWithGeneratedPrimaryKeyAndNoSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Db(typeof(EntryWithGeneratedPrimaryKeyAndNoSymbol))).ToArray();
			string[] errorsEntryWithGeneratedPrimaryKeyAndWithSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Db(typeof(EntryWithGeneratedPrimaryKeyAndWithSymbol))).ToArray();
			string[] errorsEntryWithPrimaryKeyAndNoSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Db(typeof(EntryWithPrimaryKeyAndNoSymbol))).ToArray();
			string[] errorsEntryWithPrimaryKeyAndWithSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Db(typeof(EntryWithPrimaryKeyAndWithSymbol))).ToArray();

			// Assert			
			Assert.IsTrue(errorsEntryWithGeneratedPrimaryKeyAndNoSymbol.Any()); // obsahuje chybu (není dle čeho párovat)
			Assert.IsFalse(errorsEntryWithGeneratedPrimaryKeyAndWithSymbol.Any()); // neobsahuje chybu
			Assert.IsFalse(errorsEntryWithPrimaryKeyAndNoSymbol.Any()); // neobsahuje chybu
			Assert.IsTrue(errorsEntryWithPrimaryKeyAndWithSymbol.Any()); // obsahuje chybu (duplicitní možnost párování)

		}
	}
}

