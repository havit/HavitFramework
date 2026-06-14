using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.ModelValidation;

/// <summary>
/// Kontroluje pravidla modelu.
/// Pro použití v unit testu.
/// </summary>
public class ModelValidator
{
	/// <summary>
	/// Kontroluje pravidla modelu. Jsou použita výchozí pravidla.
	/// </summary>
	/// <returns>Vrací seznam chyb (nebo prázdný řetězec).</returns>
	public string Validate(DbContext dbContext)
	{
		return Validate(dbContext, new ValidationRules());
	}

	/// <summary>
	/// Kontroluje pravidla modelu.
	/// </summary>
	/// <returns>Vrací seznam chyb (nebo prázdný řetězec).</returns>
	public string Validate(DbContext dbContext, ValidationRules validationRules, Func<IReadOnlyEntityType, bool> entityTypeFilter = null)
	{
		IModel model = dbContext.Model;

		List<string> errors = model.GetApplicationEntityTypes(includeManyToManyEntities: false)
			.WhereIf(entityTypeFilter != null, entityTypeFilter)
			.SelectMany(entityType => CheckWhenEnabled(validationRules.CheckPrimaryKeyIsNotComposite, () => CheckPrimaryKeyIsNotComposite(entityType))
				.Concat(CheckWhenEnabled(validationRules.CheckPrimaryKeyName, () => CheckPrimaryKeyName(entityType)))
				.Concat(CheckWhenEnabled(validationRules.CheckPrimaryKeyType, () => CheckPrimaryKeyType(entityType)))
				.Concat(CheckWhenEnabled(validationRules.CheckIdPascalCaseNamingConvention, () => CheckIdPascalCaseNamingConvention(entityType)))
				.Concat(CheckWhenEnabled(validationRules.CheckNavigationPropertiesHaveForeignKeys, () => CheckNavigationPropertiesHaveForeignKeys(entityType)))
				.Concat(CheckWhenEnabled(validationRules.CheckStringsHaveMaxLengths, () => CheckStringsHaveMaxLengths(entityType)))
				.Concat(CheckWhenEnabled(validationRules.CheckSupportedNestedTypes, () => CheckSupportedNestedTypes(entityType)))
				.Concat(CheckWhenEnabled(validationRules.CheckSymbolVsPrimaryKeyForEntries, () => CheckSymbolVsPrimaryKeyForEntries(entityType)))
				.Concat(CheckWhenEnabled(validationRules.CheckOnlyForeignKeysEndWithId, () => CheckOnlyForeignKeysEndsWithId(entityType)))
				.Concat(CheckWhenEnabled(validationRules.CheckAllForeignKeysEndWithId, () => CheckAllForeignKeysEndsWithId(entityType))))
		   .Concat(model.GetEntityTypes()
				.SelectMany(entityType => CheckWhenEnabled(validationRules.CheckNoOwnedIsRegistered, () => CheckNoOwnedIsRegistered(entityType))
					.Concat(CheckWhenEnabled(validationRules.CheckInheritanceNotUsed, () => CheckInheritanceIsNotUsed(entityType)))))
			.ToList();

		return String.Join(Environment.NewLine, errors);
	}

	/// <summary>
	/// Vrací výsledek action, pokud je enabled true. Jinak vrací prázdný enumerátor.
	/// </summary>
	internal IEnumerable<string> CheckWhenEnabled(bool enabled, Func<IEnumerable<string>> action)
	{
		return enabled ? action() : Enumerable.Empty<string>();
	}

	/// <summary>
	/// Kontroluje, zda třída obsahuje právě jeden primární klíč.
	/// </summary>
	internal IEnumerable<string> CheckPrimaryKeyIsNotComposite(IReadOnlyEntityType entityType)
	{
		// Primární klíč je definován na kořeni hierarchie, odvozené typy jej pouze dědí (FindPrimaryKey vrací klíč kořene).
		// Kontrolujeme jej proto jen jednou - na kořeni - abychom jej u dědičnosti nehlásili opakovaně za každého potomka.
		if (entityType.BaseType != null)
		{
			yield break;
		}

		if (entityType.FindPrimaryKey().Properties.Count > 1)
		{
			yield return $"Class {entityType.ClrType.Name} has {entityType.FindPrimaryKey().Properties.Count} key members but only one is expected.";
		}
	}

	/// <summary>
	/// Kontroluje, zda je primární klíč pojmenovaný "Id".
	/// </summary>
	internal IEnumerable<string> CheckPrimaryKeyName(IReadOnlyEntityType entityType)
	{
		// Primární klíč je definován na kořeni hierarchie, odvozené typy jej pouze dědí (FindPrimaryKey vrací klíč kořene).
		// Kontrolujeme jej proto jen jednou - na kořeni - abychom jej u dědičnosti nehlásili opakovaně za každého potomka.
		if (entityType.BaseType != null)
		{
			yield break;
		}

		foreach (IReadOnlyProperty keyProperty in entityType.FindPrimaryKey().Properties)
		{
			if (keyProperty.Name != "Id")
			{
				yield return $"Class {entityType.ClrType.Name} has a primary key named '{keyProperty.Name}' but 'Id' is expected.";
			}
		}
	}

	// source: https://learn.microsoft.com/en-us/dotnet/standard/numerics
	private readonly HashSet<Type> supportedKeyTypes = new HashSet<Type>
	{
		// signed integer types
		typeof(SByte),
		typeof(Int16),
		typeof(Int32),
		typeof(Int64),
		//typeof(Int128),
		//typeof(BigIngeter),

		// unsigned integer types
		typeof(Byte),
		typeof(UInt16),
		typeof(UInt32),
		typeof(UInt64),
		//typeof(UInt128),

		// other types
		typeof(string),
		typeof(Guid)
	};

	// Celočíselné typy použitelné pro párování entries podle primárního klíče (hodnota enumu = hodnota PK).
	// Podmnožina supportedKeyTypes bez string a Guid.
	private readonly HashSet<Type> integerKeyTypes = new HashSet<Type>
	{
		// signed integer types
		typeof(SByte),
		typeof(Int16),
		typeof(Int32),
		typeof(Int64),

		// unsigned integer types
		typeof(Byte),
		typeof(UInt16),
		typeof(UInt32),
		typeof(UInt64),
	};

	/// <summary>
	/// Kontroluje typ primárního klíče.
	/// </summary>
	internal IEnumerable<string> CheckPrimaryKeyType(IReadOnlyEntityType entityType)
	{
		// Primární klíč je definován na kořeni hierarchie, odvozené typy jej pouze dědí (FindPrimaryKey vrací klíč kořene).
		// Kontrolujeme jej proto jen jednou - na kořeni - abychom jej u dědičnosti nehlásili opakovaně za každého potomka.
		if (entityType.BaseType != null)
		{
			yield break;
		}

		foreach (IReadOnlyProperty keyProperty in entityType.FindPrimaryKey().Properties)
		{
			if (!supportedKeyTypes.Contains(keyProperty.ClrType))
			{
				string supportedTypes = String.Join(", ", supportedKeyTypes.Select(type => type.FullName).OrderBy(item => item));
				yield return $"Class {entityType.ClrType.Name} has a primary key named '{keyProperty.Name}' of unsupported type {keyProperty.ClrType}. Supported types are {supportedTypes}.";
			}
		}
	}

	/// <summary>
	/// Kontroluje, aby žádná vlastnost nekončila na "ID" (kapitálkami).
	/// </summary>
	internal IEnumerable<string> CheckIdPascalCaseNamingConvention(IReadOnlyEntityType entityType)
	{
		// GetDeclaredProperties (nikoliv GetProperties) - zděděné vlastnosti zkontrolujeme na předkovi, kde jsou deklarovány (jinak bychom je u dědičnosti hlásili opakovaně).
		foreach (IReadOnlyProperty property in entityType.GetDeclaredProperties())
		{
			if (property.Name.EndsWith("ID", false, CultureInfo.InvariantCulture))
			{
				yield return $"Class {entityType.ClrType.Name} has a property {property.Name} which ends with 'ID' but expected is 'Id'.";
			}
		}
	}

	/// <summary>
	/// Kontroluje, zda mají všechny stringové vlastnosti uvedenu maximální délku.
	/// </summary>
	internal IEnumerable<string> CheckStringsHaveMaxLengths(IReadOnlyEntityType entityType)
	{
		// GetDeclaredProperties (nikoliv GetProperties) - zděděné vlastnosti zkontrolujeme na předkovi, kde jsou deklarovány (jinak bychom je u dědičnosti hlásili opakovaně).
		foreach (IReadOnlyProperty property in entityType.GetDeclaredProperties())
		{
			if (property.ClrType == typeof(string)
				&& !property.IsShadowProperty() // nejde o Discriminator
				&& (property.PropertyInfo != null) // nejde o field-mapped property (nemáme kde hledat MaxLengthAttribute)
				&& String.IsNullOrEmpty(property.GetComputedColumnSql())) // nejde o computed column
			{
				MaxLengthAttribute maxLengthAttribute = property.PropertyInfo.GetCustomAttribute<MaxLengthAttribute>();
				if (maxLengthAttribute == null)
				{
					yield return $"Class {entityType.ClrType.Name} has a string property {property.Name} without known maximum length, MaxLengthAttribute on property is expected.";
				}
				else
				{
					if (maxLengthAttribute.Length == 0)
					{
						yield return $"Class {entityType.ClrType.Name} has a string property {property.Name} with zero value, it is expected to be greater than 0 (or -1 as 'max allowable').";
					}

					// -1 == MaxAllowableLength --> NOOP
					if (maxLengthAttribute.Length < -1)
					{
						yield return $"Class {entityType.ClrType.Name} has a string property {property.Name} with negative value, it is expected to be greater than 0 (or -1 as 'max allowable').";
					}
				}
			}
		}
	}

	/// <summary>
	/// Kontroluje, zda jsou použity pouze podporované nested types.
	/// </summary>
	internal IEnumerable<string> CheckSupportedNestedTypes(IReadOnlyEntityType entityMap)
	{
		Type[] nestedTypes = entityMap.ClrType.GetNestedTypes();

		foreach (Type nestedType in nestedTypes)
		{
			if (!(nestedType.IsEnum && nestedType.Name == "Entry"))
			{
				yield return $"Class {entityMap.ClrType.Name} has an unsupported nested type {nestedType.Name}. Only enum type Entry is supported.";
			}
		}
	}

	/// <summary>
	/// Kontroluje, zda mají navigační vlastnosti cizí klíč.
	/// </summary>
	internal IEnumerable<string> CheckNavigationPropertiesHaveForeignKeys(IReadOnlyEntityType entityType)
	{
		// GetDeclaredNavigations (nikoliv GetNavigations) - zděděné navigace zkontrolujeme na předkovi, kde jsou deklarovány (jinak bychom je u dědičnosti hlásili opakovaně).
		foreach (IReadOnlyNavigation navigationProperty in entityType.GetDeclaredNavigations())
		{
			// Pro Owned types nemůžeme mít cizí klíč (Bug 41479).
			if ((!navigationProperty.ForeignKey.IsOwnership) && navigationProperty.ForeignKey.Properties.Any(item => item.IsShadowProperty()))
			{
				yield return $"Class {entityType.ClrType.Name} has a navigation property {navigationProperty.Name} with no foreign key.";
			}
		}
	}

	/// <summary>
	/// Kontroluje třídy, které mají Entry. Třídy, které mají vlastnost Symbol, nesmí mít generovaný klíč a zároveň naopak třídy, které nemají vlastnost Symbol, musí mít generovaný klíč.
	/// </summary>
	internal IEnumerable<string> CheckSymbolVsPrimaryKeyForEntries(IReadOnlyEntityType entityType)
	{
		bool hasEntryEnum = entityType.ClrType.GetNestedTypes().Any(nestedType => nestedType.IsEnum && (nestedType.Name == "Entry"));
		// Properties.All: Sice čekáme jediný sloupec, nicméně v databázi můžeme mít na tabulce složený primární klíč (což sice nechceme, ale být to tam může)
		bool primaryKeySequence = entityType.FindPrimaryKey().Properties.All(property => property.GetDefaultValueSql()?.ToUpper().Contains("NEXT VALUE FOR") ?? false);

		if (hasEntryEnum && !primaryKeySequence)
		{
			// Properties.Any: Sice čekáme jediný sloupec, nicméně primární klíč může být složený (viz výše). Pokud je generovaná byť jen část klíče, nelze podle klíče párovat.
			bool primaryKeyGenerated = entityType.FindPrimaryKey().Properties.Any(property => property.ValueGenerated == ValueGenerated.OnAdd);
			bool symbolExists = entityType.GetProperties().Any(item => item.Name == "Symbol");

			if (primaryKeyGenerated && !symbolExists && !primaryKeySequence)
			{
				yield return $"Class {entityType.ClrType.Name} has Enum mapped to a table with primary key with identity and without column Symbol (unable to pair items).";
			}

			// Bez vlastnosti Symbol se entries párují podle primárního klíče (hodnota enumu = hodnota PK), což vyžaduje celočíselný primární klíč.
			if (!primaryKeyGenerated && !symbolExists && !primaryKeySequence
				&& !entityType.FindPrimaryKey().Properties.All(property => integerKeyTypes.Contains(property.ClrType)))
			{
				yield return $"Class {entityType.ClrType.Name} has Enum mapped to a table paired by primary key without column Symbol, but an integer type is expected for the primary key.";
			}

			if (!primaryKeyGenerated && symbolExists)
			{
				yield return $"Class {entityType.ClrType.Name} has Enum mapped to a table with primary key without identity and with column Symbol (ambiguous pairing fields).";
			}
		}
	}

	/// <summary>
	/// Kontroluje, zda všechny vlastnosti, jejichž název končí 'Id' jsou cizím klíčem.
	/// Výjimkou z pravidla jsou vlastnosti končíci na ExternalId (tedy např. ExternalId nebo IdentityProviderExternalId mohou končit na Id,
	/// přestože nejsou cizím ani primárním klíčem.
	/// </summary>
	internal IEnumerable<string> CheckOnlyForeignKeysEndsWithId(IReadOnlyEntityType entityType)
	{
		// GetDeclaredProperties (nikoliv GetProperties) - zděděné vlastnosti zkontrolujeme na předkovi, kde jsou deklarovány (jinak bychom je u dědičnosti hlásili opakovaně).
		foreach (IReadOnlyProperty property in entityType.GetDeclaredProperties().Where(property => !property.IsShadowProperty()))
		{
			if (property.Name.EndsWith("Id") && !property.Name.EndsWith("ExternalId") && !property.IsModelValidatorRuleSupressed(ModelValidatorRule.OnlyForeignKeyPropertiesCanEndWithId) && !property.IsForeignKey() && !property.IsKey())
			{
				yield return $"Class {entityType.ClrType.Name} has a property named {property.Name} which is not a foreign key. The property name ends with 'Id' which is allowed only for foreign keys. Rename the property or suppress the validation rule.";
			}
		}
	}

	/// <summary>
	/// Kontroluje, zda názvy všech cizích klíčů končí 'Id'.
	/// </summary>
	internal IEnumerable<string> CheckAllForeignKeysEndsWithId(IReadOnlyEntityType entityType)
	{
		// GetDeclaredProperties (nikoliv GetProperties) - zděděné vlastnosti zkontrolujeme na předkovi, kde jsou deklarovány (jinak bychom je u dědičnosti hlásili opakovaně).
		foreach (IReadOnlyProperty property in entityType.GetDeclaredProperties().Where(property => !property.IsShadowProperty()))
		{
			if (!property.Name.EndsWith("Id") && property.IsForeignKey())
			{
				yield return $"Class {entityType.ClrType.Name} has a property named {property.Name} which is a foreign key. The property name does not end with 'Id'.";
			}
		}
	}

	/// <summary>
	/// Kontroluje, zda není registrován žádný Owned Type.
	/// </summary>
	internal IEnumerable<string> CheckNoOwnedIsRegistered(IReadOnlyEntityType entityType)
	{
		if (entityType.IsOwned())
		{
			yield return $"Class {entityType.ClrType.Name} is a registered owned type. Owned types are not supported.";
		}
	}

	/// <summary>
	/// Kontroluje, zda v modelu není použita dědičnost.
	/// </summary>
	internal IEnumerable<string> CheckInheritanceIsNotUsed(IReadOnlyEntityType entityType)
	{
		if (entityType.BaseType != null)
		{
			yield return $"Class {entityType.ClrType.Name} is a descendant of {entityType.BaseType.ClrType.Name}. Inheritance is not supported.";
		}
	}
}