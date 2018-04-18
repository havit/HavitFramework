using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Havit.Data.Entity.Mapping.Internal;
using Havit.Data.Entity.Model;
using MappingApiExtensions = EntityFramework.MappingAPI.Extensions.MappingApiExtensions;

namespace Havit.Data.Entity.Validators
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
			RegisteredEntity[] entityMaps = dbContext.Db();
			
			List<string> errors = entityMaps.
                Where(item => item.Type != typeof(DataSeedVersion))
                .SelectMany(entityMap => CheckPrimaryKey(entityMap)
				    .Concat(CheckIdNamingConvention(entityMap))
				    .Concat(CheckStringMaxLengthConvention(entityMap))
				    .Concat(CheckNestedMembers(entityMap))
				    .Concat(CheckSymbolVsPrimaryKeyForEntries(entityMap))
				    ).ToList();

			return String.Join(Environment.NewLine, errors);
		}

		/// <summary>
		/// Kontroluje, zda třída obsahuje právě jeden primární klíč pojmenovaný "Id".
		/// </summary>
		internal IEnumerable<string> CheckPrimaryKey(RegisteredEntity entityMap)
		{
			if (entityMap.PrimaryKeys.Count > 1)
			{
				yield return $"Class {entityMap.Type.Name} has {entityMap.PrimaryKeys.Count} key members but only one is expected.";
			}

			foreach (var keyMember in entityMap.PrimaryKeys)
			{
				if (keyMember.PropertyName != "Id")
				{
					yield return $"Class {entityMap.Type.Name} has a primary key named '{keyMember.PropertyName}' but 'Id' is expected.";
				}

				if (keyMember.Type != typeof(int))
				{
					yield return $"Class {entityMap.Type.Name} has a primary key named '{keyMember.PropertyName}' of type {keyMember.Type.Name}, but type int (System.Int32) is expected.";
				}

			}
		}

		/// <summary>
		/// Kontroluje, že žádná vlastnost nekončí na "ID" (kapitálkami).
		/// </summary>
		internal IEnumerable<string> CheckIdNamingConvention(RegisteredEntity entityMap)
		{
			foreach (var property in entityMap.Properties)
			{
				if (property.PropertyName.EndsWith("ID", false, CultureInfo.InvariantCulture))
				{
					yield return $"Class {entityMap.Type.Name} has a property {property.PropertyName} which ends with 'ID' but expected is 'Id'.";
				}
			}
		}

		/// <summary>
		/// Kontroluje, že všechny stringové vlastnosti mají uvedenu maximální délku.
		/// </summary>
		internal IEnumerable<string> CheckStringMaxLengthConvention(RegisteredEntity entityMap)
		{
			foreach (var property in entityMap.Properties)
			{
				if (property.Type == typeof(string))
				{
					MaxLengthAttribute maxLengthAttribute = entityMap.Type.GetProperty(property.PropertyName).GetCustomAttribute<MaxLengthAttribute>();
					if (maxLengthAttribute == null)
					{
						yield return $"Class {entityMap.Type.Name} has a string property {property.PropertyName} without known maximum length, MaxLengthAttribute on property is expected.";
					}
					else
					{
						if (maxLengthAttribute.Length == 0)
						{
							yield return $"Class {entityMap.Type.Name} has a string property {property.PropertyName} with zero value, it is expected to be greater then 0 (or -1 as 'max allowable').";
						}
						// -1 == MaxAllowableLength --> NOOP
						if (maxLengthAttribute.Length < -1) 
						{
							yield return $"Class {entityMap.Type.Name} has a string property {property.PropertyName} with negative value, it is expected to be greater then 0 (or -1 as 'max allowable').";
						}
					}
				}
			}
		}

		/// <summary>
		/// Kontroluje, že nejsou použity nested classes.
		/// </summary>
		internal IEnumerable<string> CheckNestedMembers(RegisteredEntity entityMap)
		{
			Type[] nestedTypes = entityMap.Type.GetNestedTypes();

			foreach (Type nestedType in nestedTypes)
			{
				if (!(nestedType.IsEnum && nestedType.Name == "Entry"))
				{
					yield return $"Class {entityMap.Type.Name} has a unsupported nested type {nestedType.Name}. Only enum type Entry is supported.";
				}
			}
		}

		/// <summary>
		/// Kontroluje, že nejsou použity nested classes.
		/// </summary>
		internal IEnumerable<string> CheckForeignKeyForNavigationProperties(RegisteredEntity entityMap)
		{
			foreach (RegisteredProperty navigationProperty in entityMap.Properties.Where(item => item.IsNavigationProperty))
			{
				if (navigationProperty.ForeignKey == null)
				{
					yield return $"Class {entityMap.Type.Name} has a navigation property {navigationProperty.PropertyName} but no foreign key.";
				}
			}
		}

		internal IEnumerable<string> CheckSymbolVsPrimaryKeyForEntries(RegisteredEntity entityMap)
		{
			bool hasEntryEnum = entityMap.Type.GetNestedTypes().Any(nestedType => nestedType.IsEnum && (nestedType.Name == "Entry"));

			if (hasEntryEnum)
			{
				bool pkWithIdentity = entityMap.HasDatabaseGeneratedIdentity;
				bool symbolExists = entityMap.Properties.Any(item => item.PropertyName == "Symbol");

				if (pkWithIdentity && !symbolExists)
				{
					yield return $"Class {entityMap.Type.Name} has Enum mapped to table with primary key with identity and withoud column Symbol (unable to pair items).";
				}

				if (!pkWithIdentity && symbolExists)
				{
					yield return $"Class {entityMap.Type.Name} has Enum mapped to table with primary key without identity and with column Symbol (ambiguous pairing fields).";
				}
			}
		}
	}
}
