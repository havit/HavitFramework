using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using DbContext = Havit.EntityFrameworkCore.DbContext;

namespace Havit.Data.EFCore.ModelValidation
{
	/// <summary>
	/// Kontroluje pravidla modelu.
	/// Pro použití např. v unit testu.
	/// </summary>
	public class ModelValidator
	{
		/// <summary>
		/// Kontroluje pravidla modelu.
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
		public string Validate(DbContext dbContext, ValidationRules validationRules)
		{
			IModel model = dbContext.Model;

			List<string> errors = model.GetEntityTypes()
				.Where(entitytype => !IsSystemType(entitytype))
				.Where(entitytype => !IsManyToManyEntity(entitytype))
				.SelectMany(entityType => CheckWhenEnabled(validationRules.CheckPrimaryKeyIsNotComposite, () => CheckPrimaryKeyIsNotComposite(entityType))
					.Concat(CheckWhenEnabled(validationRules.CheckPrimaryKeyNamingConvention, () => CheckPrimaryKeyNamingConvention(entityType)))
					.Concat(CheckWhenEnabled(validationRules.CheckPrimaryKeyType, () => CheckPrimaryKeyType(entityType)))
					.Concat(CheckWhenEnabled(validationRules.CheckIdNamingConvention, () => CheckIdNamingConvention(entityType)))
					.Concat(CheckWhenEnabled(validationRules.CheckStringsHaveMaxLengths, () => CheckStringsHaveMaxLengths(entityType)))
					.Concat(CheckWhenEnabled(validationRules.CheckSupportedNestedTypes, () => CheckSupportedNestedTypes(entityType)))
					.Concat(CheckWhenEnabled(validationRules.CheckSymbolVsPrimaryKeyForEntries, () => CheckSymbolVsPrimaryKeyForEntries(entityType))))
				.ToList();

			return String.Join(Environment.NewLine, errors);
		}

		internal bool IsSystemType(IEntityType entityType)
		{
			// TODO JK: DataSeedVersion
			/*.Where(item => item.ClrType != typeof(DataSeedVersion))*/
			return false;
		}

		/// <summary>
		/// Vrací true, pokud je entita vztahovou entitou M:N vztahu.
		/// </summary>
		internal bool IsManyToManyEntity(IEntityType entityType)
		{
			// GetProperties neobsahuje vlastnosti z nadřazených tříd, v tomto scénáři to nevadí, dědičnost pro tabulky se dvěma sloupci primárního klíče neuvažujeme
			return (entityType.FindPrimaryKey().Properties.Count == 2) // třída má složený primární klíč ze svou vlastností
				&& (entityType.GetProperties().Count() == 2) // třída má právě dvě (skalární) vlastnosti
				&& entityType.GetNavigations().All(item => item.ForeignKey.Properties.All(p => p.IsKey())); // klíče všechn navigačních vlastností jsou součástí primárního klíče
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
		internal IEnumerable<string> CheckPrimaryKeyIsNotComposite(IEntityType entityType)
		{
			if (entityType.FindPrimaryKey().Properties.Count > 1)
			{
				yield return $"Class {entityType.ClrType.Name} has {entityType.GetProperties().Count()} key members but only one is expected.";
			}
		}

		/// <summary>
		/// Kontroluje, zda je primární klíč pojmenovaný "Id".
		/// </summary>
		internal IEnumerable<string> CheckPrimaryKeyNamingConvention(IEntityType entityType)
		{
			foreach (IProperty keyProperty in entityType.FindPrimaryKey().Properties)
			{
				if (keyProperty.Name != "Id")
				{
					yield return $"Class {entityType.ClrType.Name} has a primary key named '{keyProperty.Name}' but 'Id' is expected.";
				}
			}
		}

		/// <summary>
		/// Kontroluje, zda je primární klíč typu System.Int32.
		/// </summary>
		internal IEnumerable<string> CheckPrimaryKeyType(IEntityType entityType)
		{
				// TODO JK: Opravdu chceme test na int?
			foreach (IProperty keyProperty in entityType.FindPrimaryKey().Properties)
			{
				if (keyProperty.ClrType != typeof(int))
				{
					yield return $"Class {entityType.ClrType.Name} has a primary key named '{keyProperty.Name}' of type {keyProperty.ClrType}, but type int (System.Int32) is expected.";
				}
			}
		}

		/// <summary>
		/// Kontroluje, aby žádná vlastnost nekončila na "ID" (kapitálkami).
		/// </summary>
		internal IEnumerable<string> CheckIdNamingConvention(IEntityType entityType)
		{
			foreach (IProperty property in entityType.GetProperties())
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
		internal IEnumerable<string> CheckStringsHaveMaxLengths(IEntityType entityType)
		{
			foreach (IProperty property in entityType.GetProperties())
			{
				if (property.ClrType == typeof(string))
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
							yield return $"Class {entityType.ClrType.Name} has a string property {property.Name} with zero value, it is expected to be greater then 0 (or -1 as 'max allowable').";
						}

						// -1 == MaxAllowableLength --> NOOP
						if (maxLengthAttribute.Length < -1)
						{
							yield return $"Class {entityType.ClrType.Name} has a string property {property.Name} with negative value, it is expected to be greater then 0 (or -1 as 'max allowable').";
						}
					}
				}
			}
		}

		/// <summary>
		/// Kontroluje, zda jsou použity pouze podporované nested types.
		/// </summary>
		internal IEnumerable<string> CheckSupportedNestedTypes(IEntityType entityMap)
		{
			Type[] nestedTypes = entityMap.ClrType.GetNestedTypes();

			foreach (Type nestedType in nestedTypes)
			{
				if (!(nestedType.IsEnum && nestedType.Name == "Entry"))
				{
					yield return $"Class {entityMap.ClrType.Name} has a unsupported nested type {nestedType.Name}. Only enum type Entry is supported.";
				}
			}
		}

		/// <summary>
		/// Kontroluje, zda mají navigační vlastnosti cizí klíč.
		/// </summary>
		internal IEnumerable<string> CheckNavigationPropertiesHaveForeignKeys(IEntityType entityType)
		{
			foreach (INavigation navigationProperty in entityType.GetNavigations())
			{
				if (navigationProperty.ForeignKey.Properties.Any(item => item.IsShadowProperty))
				{
					yield return $"Class {entityType.ClrType.Name} has a navigation property {navigationProperty.Name} but no foreign key.";
				}
			}
		}

		/// <summary>
		/// Kontroluje třídy, které mají Entry. Třídy, které mají vlastnost symbol, nesmí mít generovaný klíč a zároveň naopak třídy, které nemají vlastnost Symbo, musí mít generovaný klíč.
		/// </summary>
		internal IEnumerable<string> CheckSymbolVsPrimaryKeyForEntries(IEntityType entityType)
		{
			// TODO JK: Rozhodnout o symbol jako autogenerated vs. sequence.
			bool hasEntryEnum = entityType.ClrType.GetNestedTypes().Any(nestedType => nestedType.IsEnum && (nestedType.Name == "Entry"));

			if (hasEntryEnum)
			{
				bool primaryKeyGenerated = entityType.FindPrimaryKey().Properties.Single().ValueGenerated == ValueGenerated.OnAdd;
				bool symbolExists = entityType.GetProperties().Any(item => item.Name == "Symbol");

				if (primaryKeyGenerated && !symbolExists)
				{
					yield return $"Class {entityType.ClrType.Name} has Enum mapped to table with primary key with identity and withoud column Symbol (unable to pair items).";
				}

				if (!primaryKeyGenerated && symbolExists)
				{
					yield return $"Class {entityType.ClrType.Name} has Enum mapped to table with primary key without identity and with column Symbol (ambiguous pairing fields).";
				}
			}
		}
	}
}