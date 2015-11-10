using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Havit.Data.Entity.Tests
{
	/// <summary>
	/// Kontroluje pravidla modelu.
	/// Pro použití např. v unit testu.
	/// </summary>
	public class DbContextConventionsValidator
	{
		/// <summary>
		/// Kontroluje pravidla modelu.
		/// </summary>
		/// <returns>Vrací seznam chyb (nebo prázdný řetězec).</returns>
		public string Validate(DbContext dbContext)
		{
			ObjectContext objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;

			List<string> errors = new List<string>();
			
			errors.AddRange(CheckPrimaryKey(objectContext));
			errors.AddRange(CheckIdNamingConvention(objectContext));
			errors.AddRange(CheckStringMaxLengthConvention(objectContext));
			errors.AddRange(CheckNestedMembers(objectContext));

			return String.Join(Environment.NewLine, errors);
		}

		private IEnumerable<string> CheckPrimaryKey(ObjectContext objectContext)
		{
			List<EntityType> entityTypes = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).ToList();
			foreach (var entityType in entityTypes)
			{
				if (entityType.KeyMembers.Count > 1)
				{
					yield return String.Format("Class {0} has {1} key members but only one is expected.", entityType.Name, entityType.KeyMembers.Count);
				}

				foreach (var keyMember in entityType.KeyMembers)
				{
					if (keyMember.Name != "Id")
					{
						yield return String.Format("Class {0} has a primary key named '{1}' but 'Id' is expected.", entityType.Name, keyMember.Name);
					}					
				}
			}
		}

		private IEnumerable<string> CheckIdNamingConvention(ObjectContext objectContext)
		{
			List<EntityType> entityTypes = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).ToList();
			foreach (var entityType in entityTypes)
			{
				foreach (var property in entityType.DeclaredMembers)
				{
					if (property.Name.EndsWith("ID", false, CultureInfo.InvariantCulture))
					{
						yield return String.Format("Class {0} has a property {1} which ends with 'ID' but expected is 'Id'.", entityType.Name, entityType.KeyMembers.Count);
					}
				}
			}
		}

		private IEnumerable<string> CheckStringMaxLengthConvention(ObjectContext objectContext)
		{
			List<EntityType> entityTypes = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).ToList();
			foreach (var entityType in entityTypes)
			{
				Type type = (Type)entityType.GetType().GetProperty("ClrType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(entityType); // TODO: Duplikace kódu!
				foreach (EdmProperty property in entityType.DeclaredProperties)
				{					
					if (property.IsPrimitiveType && ((PrimitiveType)property.TypeUsage.EdmType).ClrEquivalentType == typeof(string)) // TODO: Duplikace kódu
					{
						MaxLengthAttribute maxLengthAttribute = type.GetProperty(property.Name).GetCustomAttribute<MaxLengthAttribute>();

						if (maxLengthAttribute == null)
						{
							yield return String.Format("Class {0} has a string property {1} without known maximum length, MaxLengthAttribute on property is expected.", entityType.Name, property.Name);
						}
						else
						{
							if (maxLengthAttribute.Length == 0)
							{
								yield return String.Format("Class {0} has a string property {1} with the maximum length equal to 0, it is expected to be more then zero.", entityType.Name, property.Name);
							}
						}
					}
				}
			}
		}

		private IEnumerable<string> CheckNestedMembers(ObjectContext objectContext)
		{
			List<EntityType> entityTypes = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).ToList();
			foreach (var entityType in entityTypes)
			{
				Type type = (Type)entityType.GetType().GetProperty("ClrType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(entityType); // TODO: Duplikace kódu!

				Type[] nestedTypes = type.GetNestedTypes();

				foreach (Type nestedType in nestedTypes)
				{
					bool supported = false;

					if (nestedType.IsEnum && nestedType.Name == "Entry")
					{
						supported = true;
					}

					if (!supported)
					{
						yield return String.Format("Class {0} has a unsupported nested type {1}. Only enum type Entry is supported.", entityType.Name, nestedType.Name);
					}
				}
			}
		}
	}
}
