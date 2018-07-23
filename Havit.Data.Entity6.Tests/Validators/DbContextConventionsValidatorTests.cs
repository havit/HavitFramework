using Havit.Data.Entity.Tests.Validators.Infrastructure;
using Havit.Data.Entity.Tests.Validators.Infrastructure.Model;
using Havit.Data.Entity.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Havit.Data.Entity.Mapping.Internal;
using System.Collections.Generic;

namespace Havit.Data.Entity.Tests.Validators
{
	[TestClass]
	public class DbContextConventionsValidatorTests
	{
		[TestMethod]
		public void DbContextConventionsValidator_CheckPrimaryKeyIsNotComposite_ReportsMorePrimaryKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKeyIsNotComposite(modelValidatingDbContext.GetRegisteredEntities(typeof(MoreInvalidKeysClass))).ToArray();

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
            string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKeyType(modelValidatingDbContext.GetRegisteredEntities(typeof(StringIdClass))).ToArray();

            // Assert			
            Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("type int (System.Int32) is expected")));
        }

        [TestMethod]
        public void DbContextConventionsValidator_CheckPrimaryKeyName_ReportsInvalidNamedKeys()
        {
            // Arrange
            ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
            ModelValidator modelValidator = new ModelValidator();

            // Act
            string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKeyName(modelValidatingDbContext.GetRegisteredEntities(typeof(InvalidNameOfPrimaryKey))).ToArray();

            // Assert			
            Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("but 'Id' is expected")));
        }

        [TestMethod]
        public void DbContextConventionsValidator_CheckPrimaryKey_DoesNotReportOnePrimaryKeys()
        {
            // Arrange
            ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
            ModelValidator modelValidator = new ModelValidator();

            // Act
            List<string> errorsOneCorrectKeyClass = modelValidator.CheckPrimaryKeyName(modelValidatingDbContext.GetRegisteredEntities(typeof(OneCorrectKeyClass))).ToList();
            errorsOneCorrectKeyClass.AddRange(modelValidator.CheckPrimaryKeyType(modelValidatingDbContext.GetRegisteredEntities(typeof(OneCorrectKeyClass))));
            errorsOneCorrectKeyClass.AddRange(modelValidator.CheckPrimaryKeyIsNotComposite(modelValidatingDbContext.GetRegisteredEntities(typeof(OneCorrectKeyClass))));

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckIdPascalCaseNamingConvention(modelValidatingDbContext.GetRegisteredEntities(typeof(CapitalIDClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckIdPascalCaseNamingConvention(modelValidatingDbContext.GetRegisteredEntities(typeof(OneCorrectKeyClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.GetRegisteredEntities(typeof(NoMaxLengthAttributeClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.GetRegisteredEntities(typeof(MaxLengthAttributeWithPositiveValueClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.GetRegisteredEntities(typeof(WithNestedClassClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.GetRegisteredEntities(typeof(WithNestedEnumOtherClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.GetRegisteredEntities(typeof(WithNestedEnumEntryClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckForeignKeyForNavigationProperties(modelValidatingDbContext.GetRegisteredEntities(typeof(NavigationPropertyWithoutForeignKeyClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckForeignKeyForNavigationProperties(modelValidatingDbContext.GetRegisteredEntities(typeof(NavigationPropertyWithForeignKeyClass))).ToArray();

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
			string[] errorsEntryWithGeneratedPrimaryKeyAndNoSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.GetRegisteredEntities(typeof(EntryWithGeneratedPrimaryKeyAndNoSymbol))).ToArray();
			string[] errorsEntryWithGeneratedPrimaryKeyAndWithSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.GetRegisteredEntities(typeof(EntryWithGeneratedPrimaryKeyAndWithSymbol))).ToArray();
			string[] errorsEntryWithPrimaryKeyAndNoSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.GetRegisteredEntities(typeof(EntryWithPrimaryKeyAndNoSymbol))).ToArray();
			string[] errorsEntryWithPrimaryKeyAndWithSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.GetRegisteredEntities(typeof(EntryWithPrimaryKeyAndWithSymbol))).ToArray();

			// Assert			
			Assert.IsTrue(errorsEntryWithGeneratedPrimaryKeyAndNoSymbol.Any()); // obsahuje chybu (není dle čeho párovat)
			Assert.IsFalse(errorsEntryWithGeneratedPrimaryKeyAndWithSymbol.Any()); // neobsahuje chybu
			Assert.IsFalse(errorsEntryWithPrimaryKeyAndNoSymbol.Any()); // neobsahuje chybu
			Assert.IsTrue(errorsEntryWithPrimaryKeyAndWithSymbol.Any()); // obsahuje chybu (duplicitní možnost párování)

        }
    }
}

