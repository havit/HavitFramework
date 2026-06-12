using Havit.Data.EntityFrameworkCore.ModelValidation;
using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure;
using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation;

[TestClass]
public class ModelValidatorTests
{
	// MSTest vytváří pro každý test novou instanci třídy, testy jsou tak izolované.
	// EF Core model se cachuje per typ DbContextu, takže opakované vytváření contextu je levné.
	private readonly ModelValidatingDbContext _modelValidatingDbContext = new ModelValidatingDbContext();
	private readonly ModelValidator _modelValidator = new ModelValidator();

	[TestMethod]
	public void ModelValidator_CheckWhenEnabled()
	{
		// Act + Assert
		Assert.IsFalse(_modelValidator.CheckWhenEnabled(false, () => throw new InvalidOperationException()).Any()); // jednak se nevolá action a jednak nic nevrátí
		Assert.IsTrue(_modelValidator.CheckWhenEnabled(true, () => new List<string> { "ok" }).Contains("ok")); // jednak se volá action a je jeho hodnota ve výsledku
	}

	[TestMethod]
	public void ModelValidator_CheckPrimaryKeyName_ReportsNonIdKey()
	{
		// Act
		string[] errors = _modelValidator.CheckPrimaryKeyName(_modelValidatingDbContext.Model.FindEntityType(typeof(NonIdKeyClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("has a primary key named", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckPrimaryKeyName_DoesNotReportIdKey()
	{
		// Act
		string[] errors = _modelValidator.CheckPrimaryKeyName(_modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckPrimaryKeyIsNotComposite_ReportsMorePrimaryKeys()
	{
		// Act
		string[] errors = _modelValidator.CheckPrimaryKeyIsNotComposite(_modelValidatingDbContext.Model.FindEntityType(typeof(MoreInvalidKeysClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("only one is expected", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckPrimaryKeyIsNotComposite_DoesNotReportOnePrimaryKeys()
	{
		// Act
		string[] errors = _modelValidator.CheckPrimaryKeyIsNotComposite(_modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckPrimaryKeyType_ReportsUnsupportedKeys()
	{
		// Act
		string[] errors = _modelValidator.CheckPrimaryKeyType(_modelValidatingDbContext.Model.FindEntityType(typeof(DateTimeIdClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("of unsupported type", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckIdPascalCaseNamingConvention_ReportsCapitalId()
	{
		// Act
		string[] errors = _modelValidator.CheckIdPascalCaseNamingConvention(_modelValidatingDbContext.Model.FindEntityType(typeof(CapitalIDClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("which ends with", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckIdPascalCaseNamingConvention_DoesNotReportPascalCaseId()
	{
		// Act
		string[] errors = _modelValidator.CheckIdPascalCaseNamingConvention(_modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_ReportsNegativeMaxLengthAttribute()
	{
		// Act
		string[] errors = _modelValidator.CheckStringsHaveMaxLengths(_modelValidatingDbContext.Model.FindEntityType(typeof(NegativeMaxLengthAttributeClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("negative value", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_ReportsZeroMaxLengthAttribute()
	{
		// Act
		string[] errors = _modelValidator.CheckStringsHaveMaxLengths(_modelValidatingDbContext.Model.FindEntityType(typeof(ZeroMaxLengthAttributeClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("zero value", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_ReportsMissingMaxLengthAttribute()
	{
		// Act
		string[] errors = _modelValidator.CheckStringsHaveMaxLengths(_modelValidatingDbContext.Model.FindEntityType(typeof(NoMaxLengthAttributeClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("MaxLengthAttribute on property is expected", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_DoesNotReportMaxLengthAttributeWithPositiveValue()
	{
		// Act
		string[] errors = _modelValidator.CheckStringsHaveMaxLengths(_modelValidatingDbContext.Model.FindEntityType(typeof(MaxLengthAttributeWithPositiveValueClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_DoesNotReportMissingMaxLengthAttributeOnComputedColumns()
	{
		// Act
		string[] errors = _modelValidator.CheckStringsHaveMaxLengths(_modelValidatingDbContext.Model.FindEntityType(typeof(WithComputedColumns))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckStringsHaveMaxLengths_SupportsModelInheritance()
	{
		// Act
		string[] errors = _modelValidator.CheckStringsHaveMaxLengths(_modelValidatingDbContext.Model.FindEntityType(typeof(Descendant))).ToArray();

		// Assert
		Assert.AreEqual(2, errors.Count(item => item.Contains("MaxLengthAttribute on property is expected")));
	}

	[TestMethod]
	public void ModelValidator_CheckSupportedNestedTypes_ReportsNestedClass()
	{
		// Act
		string[] errors = _modelValidator.CheckSupportedNestedTypes(_modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedClassClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("unsupported nested type", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckSupportedNestedTypes_ReportsNonEntryEnum()
	{
		// Act
		string[] errors = _modelValidator.CheckSupportedNestedTypes(_modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedEnumOtherClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("unsupported nested type", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckSupportedNestedTypes_DoesNotReportEntryEnum()
	{
		// Act
		string[] errors = _modelValidator.CheckSupportedNestedTypes(_modelValidatingDbContext.Model.FindEntityType(typeof(WithNestedEnumEntryClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckNavigationPropertiesHaveForeignKeys_ReportsNavigationPropertyWithoutForeignKey()
	{
		// Act
		string[] errors = _modelValidator.CheckNavigationPropertiesHaveForeignKeys(_modelValidatingDbContext.Model.FindEntityType(typeof(NavigationPropertyWithoutForeignKeyClass))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
		Assert.Contains("no foreign key", errors[0]);
	}

	[TestMethod]
	public void ModelValidator_CheckNavigationPropertiesHaveForeignKeys_DoesNotReportOwnedTypes()
	{
		// Act
		string[] errors = _modelValidator.CheckNavigationPropertiesHaveForeignKeys(_modelValidatingDbContext.Model.FindEntityType(typeof(NavigationPropertyByOwnedType))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckNavigationPropertiesHaveForeignKeys_DoesNotReportNavigationPropertyWithForeignKey()
	{
		// Act
		string[] errors = _modelValidator.CheckNavigationPropertiesHaveForeignKeys(_modelValidatingDbContext.Model.FindEntityType(typeof(NavigationPropertyWithForeignKeyClass))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_ReportsGeneratedPrimaryKeyWithoutSymbol()
	{
		// Act
		string[] errors = _modelValidator.CheckSymbolVsPrimaryKeyForEntries(_modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithGeneratedPrimaryKeyAndNoSymbol))).ToArray();

		// Assert
		Assert.HasCount(1, errors); // obsahuje chybu (není dle čeho párovat)
	}

	[TestMethod]
	public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_DoesNotReportGeneratedPrimaryKeyWithSymbol()
	{
		// Act
		string[] errors = _modelValidator.CheckSymbolVsPrimaryKeyForEntries(_modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithGeneratedPrimaryKeyAndWithSymbol))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_DoesNotReportNotGeneratedPrimaryKeyWithoutSymbol()
	{
		// Act
		string[] errors = _modelValidator.CheckSymbolVsPrimaryKeyForEntries(_modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithPrimaryKeyAndNoSymbol))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_ReportsNotGeneratedPrimaryKeyWithSymbol()
	{
		// Act
		string[] errors = _modelValidator.CheckSymbolVsPrimaryKeyForEntries(_modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithPrimaryKeyAndWithSymbol))).ToArray();

		// Assert
		Assert.HasCount(1, errors); // obsahuje chybu (duplicitní možnost párování)
	}

	[TestMethod]
	public void ModelValidator_CheckSymbolVsPrimaryKeyForEntries_DoesNotReportSequencePrimaryKeyWithoutSymbol()
	{
		// Act
		string[] errors = _modelValidator.CheckSymbolVsPrimaryKeyForEntries(_modelValidatingDbContext.Model.FindEntityType(typeof(EntryWithSequencePrimaryKeyAndNoSymbol))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckOnlyForeignKeysEndsWithId_ReportsNonForeignKeyWithId()
	{
		// Act
		string[] errors = _modelValidator.CheckOnlyForeignKeysEndsWithId(_modelValidatingDbContext.Model.FindEntityType(typeof(IdWithNoForeignKey))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
	}

	[TestMethod]
	public void ModelValidator_CheckOnlyForeignKeysEndsWithId_DoesNotReportNonForeignKeyWithExternalId()
	{
		// Act
		string[] errors = _modelValidator.CheckOnlyForeignKeysEndsWithId(_modelValidatingDbContext.Model.FindEntityType(typeof(ExternalIdWithNoForeignKey))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckOnlyForeignKeysEndsWithId_DoesNotReportAllowedNonForeignKeyWithId()
	{
		// Act
		string[] errors = _modelValidator.CheckOnlyForeignKeysEndsWithId(_modelValidatingDbContext.Model.FindEntityType(typeof(IdWithNoForeignKeyButAllowed))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckOnlyForeignKeysEndsWithId_DoesNotReportForeignKeyWithId()
	{
		// Act
		string[] errors = _modelValidator.CheckOnlyForeignKeysEndsWithId(_modelValidatingDbContext.Model.FindEntityType(typeof(IdWithForeignKey))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckAllForeignKeysEndsWithId_ReportsForeignKeyWithoutId()
	{
		// Act
		string[] errors = _modelValidator.CheckAllForeignKeysEndsWithId(_modelValidatingDbContext.Model.FindEntityType(typeof(IdWithPoorlyNamedForeignKey))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
	}

	[TestMethod]
	public void ModelValidator_CheckAllForeignKeysEndsWithId_DoesNotReportForeignKeyWithId()
	{
		// Act
		string[] errors = _modelValidator.CheckAllForeignKeysEndsWithId(_modelValidatingDbContext.Model.FindEntityType(typeof(IdWithForeignKey))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckNoOwnedIsRegistered_DoesNotReportNonOwnedType()
	{
		// Act
		string[] errors = _modelValidator.CheckNoOwnedIsRegistered(_modelValidatingDbContext.Model.FindEntityType(typeof(NonOwnedType))).ToArray();

		// Assert
		Assert.IsEmpty(errors);
	}

	[TestMethod]
	public void ModelValidator_CheckNoOwnedIsRegistered_ReportsOwnedType()
	{
		// Act
		string[] errors = _modelValidator.CheckNoOwnedIsRegistered(_modelValidatingDbContext.Model.FindEntityType(typeof(OwnedType))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
	}

	[TestMethod]
	public void ModelValidator_CheckInheritanceIsNotUsed_ReportsDescendantType()
	{
		// Act
		string[] errors = _modelValidator.CheckInheritanceIsNotUsed(_modelValidatingDbContext.Model.FindEntityType(typeof(Descendant))).ToArray();

		// Assert
		Assert.HasCount(1, errors);
	}

}