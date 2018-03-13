using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Data.Entity.Tests.Validators.Infrastructure;
using Havit.Data.Entity.Tests.Validators.Infrastructure.Model;
using Havit.Data.EFCore.ModelValidation;

namespace Havit.Data.Entity.Tests.Validators
{
	[TestClass]
	public class ModelValidatorTests
	{			
		[TestMethod]
		public void ModelValidator_CheckPrimaryKeyIsNotComposite_ReportsMorePrimaryKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKeyIsNotComposite(modelValidatingDbContext.Model.FindEntityType(typeof(MoreInvalidKeysClass).FullName)).ToArray();
			
			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("only one is expected")));
		}

		[TestMethod]
		public void ModelValidator_CheckPrimaryKeyIsNotComposite_DoesNotReportOnePrimaryKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsOneCorrectKeyClass = modelValidator.CheckPrimaryKeyIsNotComposite(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass).FullName)).ToArray();

			// Assert			
			Assert.IsFalse(errorsOneCorrectKeyClass.Any());
		}

		[TestMethod]
		public void ModelValidator_CheckPrimaryKeyType_ReportsNonIntKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKeyType(modelValidatingDbContext.Model.FindEntityType(typeof(StringIdClass).FullName)).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("type int (System.Int32) is expected")));
		}

		[TestMethod]
		public void ModelValidator_CheckIdNamingConvention_ReportsCapitalId()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckIdNamingConvention(modelValidatingDbContext.Model.FindEntityType(typeof(CapitalIDClass).FullName)).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("which ends with")));
		}

		[TestMethod]
		public void CheckIdNamingConvention_DoesNotReportPascalCaseId()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckIdNamingConvention(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass).FullName)).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void ModelValidator_CheckStringsHaveMaxLengths_ReportsMissingMaxLengthAttribute()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(NoMaxLengthAttributeClass).FullName)).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("MaxLengthAttribute on property is expected")));
		}

		[TestMethod]
		public void ModelValidator_CheckStringsHaveMaxLengths_DoesNotReportMissingAttributeWithPositiveValue()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(MaxLengthAttributeWithPositiveValueClass).FullName)).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void ModelValidator_CheckSupportedNestedTypes_ReportsNestedClass()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedClassClass).FullName)).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("unsupported nested type")));
		}

		[TestMethod]
		public void ModelValidator_CheckSupportedNestedTypes_ReportsNonEntryEnum()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedEnumOtherClass).FullName)).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("unsupported nested type")));
		}

		[TestMethod]
		public void ModelValidator_CheckSupportedNestedTypes_DoesNotReportEntryEnum()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedEnumEntryClass).FullName)).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void ModelValidator_CheckNavigationPropertiesHaveForeignKeys_ReportsNavigationPropertyWithoutForeignKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckNavigationPropertiesHaveForeignKeys(modelValidatingDbContext.Model.FindEntityType(typeof(NavigationPropertyWithoutForeignKeyClass).FullName)).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("no foreign key")));
		}

		[TestMethod]
		public void ModelValidator_CheckNavigationPropertiesHaveForeignKeys_DoesNotReportNavigationPropertyWithForeignKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckNavigationPropertiesHaveForeignKeys(modelValidatingDbContext.Model.FindEntityType(typeof(NavigationPropertyWithForeignKeyClass).FullName)).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_DoesNotReportNavigationPropertyWithForeignKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsEntryWithGeneratedPrimaryKeyAndNoSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithGeneratedPrimaryKeyAndNoSymbol).FullName)).ToArray();
			string[] errorsEntryWithGeneratedPrimaryKeyAndWithSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithGeneratedPrimaryKeyAndWithSymbol).FullName)).ToArray();
			string[] errorsEntryWithPrimaryKeyAndNoSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithPrimaryKeyAndNoSymbol).FullName)).ToArray();
			string[] errorsEntryWithPrimaryKeyAndWithSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithPrimaryKeyAndWithSymbol).FullName)).ToArray();

			// Assert			
			Assert.IsTrue(errorsEntryWithGeneratedPrimaryKeyAndNoSymbol.Any()); // obsahuje chybu (není dle čeho párovat)
			Assert.IsFalse(errorsEntryWithGeneratedPrimaryKeyAndWithSymbol.Any()); // neobsahuje chybu
			Assert.IsFalse(errorsEntryWithPrimaryKeyAndNoSymbol.Any()); // neobsahuje chybu
			Assert.IsTrue(errorsEntryWithPrimaryKeyAndWithSymbol.Any()); // obsahuje chybu (duplicitní možnost párování)

		}
	}
}

