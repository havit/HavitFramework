﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.ModelValidation
{
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
		public string Validate(DbContext dbContext, ValidationRules validationRules)
		{
			IModel model = dbContext.Model;

			List<string> errors = model.GetApplicationEntityTypes(includeManyToManyEntities: false)
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
                    .SelectMany(entityType => CheckWhenEnabled(validationRules.CheckNoOwnedIsRegistered, () => CheckNoOwnedIsRegistered(entityType))))
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
		internal IEnumerable<string> CheckPrimaryKeyName(IEntityType entityType)
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
		internal IEnumerable<string> CheckIdPascalCaseNamingConvention(IEntityType entityType)
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
				// Pro Owned types nemůžeme mít cizí klíč (Bug 41479).
				if ((!navigationProperty.ForeignKey.IsOwnership) && navigationProperty.ForeignKey.Properties.Any(item => item.IsShadowProperty()))
				{
					yield return $"Class {entityType.ClrType.Name} has a navigation property {navigationProperty.Name} with no foreign key.";
				}
			}
		}

		/// <summary>
		/// Kontroluje třídy, které mají Entry. Třídy, které mají vlastnost symbol, nesmí mít generovaný klíč a zároveň naopak třídy, které nemají vlastnost Symbol, musí mít generovaný klíč.
		/// </summary>
		internal IEnumerable<string> CheckSymbolVsPrimaryKeyForEntries(IEntityType entityType)
		{
			bool hasEntryEnum = entityType.ClrType.GetNestedTypes().Any(nestedType => nestedType.IsEnum && (nestedType.Name == "Entry"));
			bool primaryKeySequence = entityType.FindPrimaryKey().Properties.Single().GetDefaultValueSql()?.ToUpper().Contains("NEXT VALUE FOR") ?? false;

			if (hasEntryEnum && !primaryKeySequence)
			{
				bool primaryKeyGenerated = (entityType.FindPrimaryKey().Properties.Single().ValueGenerated == ValueGenerated.OnAdd);
				bool symbolExists = entityType.GetProperties().Any(item => item.Name == "Symbol");

				if (primaryKeyGenerated && !symbolExists && !primaryKeySequence)
				{
					yield return $"Class {entityType.ClrType.Name} has Enum mapped to a table with primary key with identity and without column Symbol (unable to pair items).";
				}

				if (!primaryKeyGenerated && symbolExists)
				{
					yield return $"Class {entityType.ClrType.Name} has Enum mapped to a table with primary key without identity and with column Symbol (ambiguous pairing fields).";
				}
			}
		}

		/// <summary>
		/// Kontroluje, zda všechny vlastnosti, jejichž název končí 'Id' jsou cizím klíčem.
		/// </summary>
		internal IEnumerable<string> CheckOnlyForeignKeysEndsWithId(IEntityType entityType)
		{
			foreach (var property in entityType.GetProperties().Where(property => !property.IsShadowProperty()))
			{
				if (property.Name.EndsWith("Id") && !property.IsForeignKey() && !property.IsKey())
				{
					yield return $"Class {entityType.ClrType.Name} has a property named {property.Name} which is not a foreign key. The property name ends with 'Id' which is allowed only for foreign keys.";
				}
			}
		}

		/// <summary>
		/// Kontroluje, zda názvy všech cizích klíčů končí 'Id'.
		/// </summary>
		internal IEnumerable<string> CheckAllForeignKeysEndsWithId(IEntityType entityType)
		{
			foreach (var property in entityType.GetProperties().Where(property => !property.IsShadowProperty()))
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
        internal IEnumerable<string> CheckNoOwnedIsRegistered(IEntityType entityType)
        {
            if (entityType.IsOwned())
            {
                yield return $"Class {entityType.ClrType.Name} is a registered owned type. Owned types are not supported.";
            }
        }

    }
}