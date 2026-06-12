using Havit.Data.EntityFrameworkCore.ModelValidation;
using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure;
using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation;

[TestClass]
public class ModelValidatorTests
{
	[TestMethod]
	public void ModelValidator_CheckWhenEnabled()
	{
		// Arrange
		ModelValidator modelValidator = new ModelValidator();

		// Act + Assert
		Assert.IsFalse(modelValidator.CheckWhenEnabled(false, () => throw new InvalidOperationException()).Any()); // jednak se nevolá action a jednak nic nevrátí
		Assert.IsTrue(modelValidator.CheckWhenEnabled(true, () => new List<string> { "ok" }).Contains("ok")); // jednak se volá action a je jeho hodnota ve výsledku
	}

	[TestMethod]
	public void ModelValidator_CheckPrimaryKeyName_ReportsNonIdKey()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckPrimaryKeyName(modelValidatingDbContext.Model.FindEntityType(typeof(NonIdKeyClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("has a primary key named", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckPrimaryKeyName_DoesNotReportIdKey()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckPrimaryKeyName(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckPrimaryKeyIsNotComposite_ReportsMorePrimaryKeys()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckPrimaryKeyIsNotComposite(modelValidatingDbContext.Model.FindEntityType(typeof(MoreInvalidKeysClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("only one is expected", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckPrimaryKeyIsNotComposite_DoesNotReportOnePrimaryKeys()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckPrimaryKeyIsNotComposite(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckPrimaryKeyType_ReportsUnsupportedKeys()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckPrimaryKeyType(modelValidatingDbContext.Model.FindEntityType(typeof(DateTimeIdClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("of unsupported type", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckIdPascalCaseNamingConvention_ReportsCapitalId()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckIdPascalCaseNamingConvention(modelValidatingDbContext.Model.FindEntityType(typeof(CapitalIDClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("which ends with", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckIdPascalCaseNamingConvention_DoesNotReportPascalCaseId()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckIdPascalCaseNamingConvention(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_ReportsNegativeMaxLengthAttribute()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(NegativeMaxLengthAttributeClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("negative value", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_ReportsZeroMaxLengthAttribute()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(ZeroMaxLengthAttributeClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("zero value", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_ReportsMissingMaxLengthAttribute()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(NoMaxLengthAttributeClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("MaxLengthAttribute on property is expected", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_DoesNotReportMaxLengthAttributeWithPositiveValue()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(MaxLengthAttributeWithPositiveValueClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_DoesNotReportMissingMaxLengthAttributeOnComputedColumns()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(WithComputedColumns))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_SupportsModelInheritance()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckStringsHaveMaxLengths(modelValidatingDbContext.Model.FindEntityType(typeof(Descendant))).ToArray();

		// Assert
		Assert.AreEqual(2, errors.Count(item => item.Contains("MaxLengthAttribute on property is expected")));
	}

	[TestMethod]
	public void ModelValidator_CheckSupportedNestedTypes_ReportsNestedClass()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedClassClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("unsupported nested type", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckSupportedNestedTypes_ReportsNonEntryEnum()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedEnumOtherClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("unsupported nested type", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckSupportedNestedTypes_DoesNotReportEntryEnum()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckSupportedNestedTypes(modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedEnumEntryClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckNavigationPropertiesHaveForeignKeys_ReportsNavigationPropertyWithoutForeignKey()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckNavigationPropertiesHaveForeignKeys(modelValidatingDbContext.Model.FindEntityType(typeof(NavigationPropertyWithoutForeignKeyClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("no foreign key", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckNavigationPropertiesHaveForeignKeys_DoesNotReportOwnedTypes()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckNavigationPropertiesHaveForeignKeys(modelValidatingDbContext.Model.FindEntityType(typeof(NavigationPropertyByOwnedType))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckNavigationPropertiesHaveForeignKeys_DoesNotReportNavigationPropertyWithForeignKey()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckNavigationPropertiesHaveForeignKeys(modelValidatingDbContext.Model.FindEntityType(typeof(NavigationPropertyWithForeignKeyClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_ReportsGeneratedPrimaryKeyWithoutSymbol()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithGeneratedPrimaryKeyAndNoSymbol))).ToArray();

		// Assert
		Assert.HasCount(1, errors); // obsahuje chybu (není dle čeho párovat)
	}

	[TestMethod]
	public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_DoesNotReportGeneratedPrimaryKeyWithSymbol()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithGeneratedPrimaryKeyAndWithSymbol))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_DoesNotReportNotGeneratedPrimaryKeyWithoutSymbol()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithPrimaryKeyAndNoSymbol))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_ReportsNotGeneratedPrimaryKeyWithSymbol()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithPrimaryKeyAndWithSymbol))).ToArray();

		// Assert
		Assert.HasCount(1, errors); // obsahuje chybu (duplicitní možnost párování)
	}

	[TestMethod]
	public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_DoesNotReportSequencePrimaryKeyWithoutSymbol()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithSequencePrimaryKeyAndNoSymbol))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckOnlyForeignKeysEndsWithId_ReportsNonForeignKeyWithId()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckOnlyForeignKeysEndsWithId(modelValidatingDbContext.Model.FindEntityType(typeof(IdWithNoForeignKey))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
	}

	[TestMethod]
	public void ModelValidator_CheckOnlyForeignKeysEndsWithId_DoesNotReportNonForeignKeyWithExternalId()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckOnlyForeignKeysEndsWithId(modelValidatingDbContext.Model.FindEntityType(typeof(ExternalIdWithNoForeignKey))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckOnlyForeignKeysEndsWithId_DoesNotReportAllowedNonForeignKeyWithId()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckOnlyForeignKeysEndsWithId(modelValidatingDbContext.Model.FindEntityType(typeof(IdWithNoForeignKeyButAllowed))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckOnlyForeignKeysEndsWithId_DoesNotReportForeignKeyWithId()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckOnlyForeignKeysEndsWithId(modelValidatingDbContext.Model.FindEntityType(typeof(IdWithForeignKey))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckAllForeignKeysEndsWithId_ReportsForeignKeyWithoutId()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckAllForeignKeysEndsWithId(modelValidatingDbContext.Model.FindEntityType(typeof(IdWithPoorlyNamedForeignKey))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
	}

	[TestMethod]
	public void ModelValidator_CheckAllForeignKeysEndsWithId_DoesNotReportForeignKeyWithId()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckAllForeignKeysEndsWithId(modelValidatingDbContext.Model.FindEntityType(typeof(IdWithForeignKey))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckNoOwnedIsRegistered_DoesNotReportNonOwnedType()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckNoOwnedIsRegistered(modelValidatingDbContext.Model.FindEntityType(typeof(NonOwnedType))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckNoOwnedIsRegistered_ReportsOwnedType()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckNoOwnedIsRegistered(modelValidatingDbContext.Model.FindEntityType(typeof(OwnedType))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
	}

	[TestMethod]
	public void ModelValidator_CheckInheritanceIsNotUsed_ReportsDescendantType()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
		ModelValidator modelValidator = new ModelValidator();

		// Act
		string[] errors = modelValidator.CheckInheritanceIsNotUsed(modelValidatingDbContext.Model.FindEntityType(typeof(Descendant))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
	}

}