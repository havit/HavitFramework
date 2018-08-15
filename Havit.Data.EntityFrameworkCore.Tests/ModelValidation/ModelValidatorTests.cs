﻿using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.ModelValidation;
using Havit.Data.Entity.Tests.ModelValidation.Infrastructure;
using Havit.Data.Entity.Tests.ModelValidation.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Tests.ModelValidation
{
	[TestClass]
	public class ModelValidatorTests
	{			
		[TestMethod]
		public void ModelValidator_IsSystemEntity()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act + Assert
			Assert.IsFalse(modelValidator.IsSystemEntity(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass))));
		}

		[TestMethod]
		public void ModelValidator_CheckWhenEnabled()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act + Assert  
			Assert.IsFalse(modelValidator.CheckWhenEnabled(false, () => throw new InvalidOperationException()).Any()); // jednak se nevolá action a jednak nic nevrátí
			Assert.IsTrue(modelValidator.CheckWhenEnabled(true, () => new List<string> { "ok" }).Contains("ok")); // jednak se volá action a je jeho hodnota ve výsledku
		}

		[TestMethod]
		public void ModelValidator_IsManyToManyEntity()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act + Assert
			Assert.IsTrue(modelValidator.IsManyToManyEntity(modelValidatingDbContext.Model.FindEntityType(typeof(UserRoleMembership))), typeof(UserRoleMembership).Name);
			Assert.IsFalse(modelValidator.IsManyToManyEntity(modelValidatingDbContext.Model.FindEntityType(typeof(User))), typeof(User).Name);
			Assert.IsFalse(modelValidator.IsManyToManyEntity(modelValidatingDbContext.Model.FindEntityType(typeof(MoreInvalidKeysClass))), typeof(MoreInvalidKeysClass).Name);
		}

		[TestMethod]
		public void ModelValidator_CheckPrimaryKeyName_ReportsNonIdKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKeyName(modelValidatingDbContext.Model.FindEntityType(typeof(NonIdKeyClass))).ToArray();
			
			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("has a primary key named")));
		}

		[TestMethod]
		public void ModelValidator_CheckPrimaryKeyName_DoesNotReportIdKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKeyName(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass))).ToArray();
			
			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void ModelValidator_CheckPrimaryKeyIsNotComposite_ReportsMorePrimaryKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKeyIsNotComposite(modelValidatingDbContext.Model.FindEntityType(typeof(MoreInvalidKeysClass))).ToArray();
			
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
			string[] errorsOneCorrectKeyClass = modelValidator.CheckPrimaryKeyIsNotComposite(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKeyType(modelValidatingDbContext.Model.FindEntityType(typeof(StringIdClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("type int (System.Int32) is expected")));
		}

		[TestMethod]
		public void ModelValidator_CheckIdPascalCaseNamingConvention_ReportsCapitalId()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckIdPascalCaseNamingConvention(modelValidatingDbContext.Model.FindEntityType(typeof(CapitalIDClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("which ends with")));
		}

		[TestMethod]
		public void ModelValidator_CheckIdPascalCaseNamingConvention_DoesNotReportPascalCaseId()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckIdPascalCaseNamingConvention(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void ModelValidator_CheckStringsHaveMaxLengths_ReportsNegativeMaxLengthAttribute()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(NegativeMaxLengthAttributeClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("negative value")));
		}

		[TestMethod]
		public void ModelValidator_CheckStringsHaveMaxLengths_ReportsZeroMaxLengthAttribute()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(ZeroMaxLengthAttributeClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("zero value")));
		}
		[TestMethod]
		public void ModelValidator_CheckStringsHaveMaxLengths_ReportsMissingMaxLengthAttribute()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(NoMaxLengthAttributeClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("MaxLengthAttribute on property is expected")));
		}

		[TestMethod]
		public void ModelValidator_CheckStringsHaveMaxLengths_DoesNotReportMaxLengthAttributeWithPositiveValue()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(MaxLengthAttributeWithPositiveValueClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedClassClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedEnumOtherClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedEnumEntryClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckNavigationPropertiesHaveForeignKeys(modelValidatingDbContext.Model.FindEntityType(typeof(NavigationPropertyWithoutForeignKeyClass))).ToArray();

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
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckNavigationPropertiesHaveForeignKeys(modelValidatingDbContext.Model.FindEntityType(typeof(NavigationPropertyWithForeignKeyClass))).ToArray();

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
			string[] errorsEntryWithGeneratedPrimaryKeyAndNoSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithGeneratedPrimaryKeyAndNoSymbol))).ToArray();
			string[] errorsEntryWithGeneratedPrimaryKeyAndWithSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithGeneratedPrimaryKeyAndWithSymbol))).ToArray();
			string[] errorsEntryWithPrimaryKeyAndNoSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithPrimaryKeyAndNoSymbol))).ToArray();
			string[] errorsEntryWithPrimaryKeyAndWithSymbol = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithPrimaryKeyAndWithSymbol))).ToArray();

			// Assert			
			Assert.IsTrue(errorsEntryWithGeneratedPrimaryKeyAndNoSymbol.Any()); // obsahuje chybu (není dle čeho párovat)
			Assert.IsFalse(errorsEntryWithGeneratedPrimaryKeyAndWithSymbol.Any()); // neobsahuje chybu
			Assert.IsFalse(errorsEntryWithPrimaryKeyAndNoSymbol.Any()); // neobsahuje chybu
			Assert.IsTrue(errorsEntryWithPrimaryKeyAndWithSymbol.Any()); // obsahuje chybu (duplicitní možnost párování)

		}
	}
}
