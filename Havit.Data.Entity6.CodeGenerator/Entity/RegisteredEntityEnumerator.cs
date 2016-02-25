using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;

namespace Havit.Data.Entity.CodeGenerator.Entity
{
	public class RegisteredEntityEnumerator
	{
		private readonly ObjectContext objectContext;

		public RegisteredEntityEnumerator(ObjectContext objectContext)
		{
			this.objectContext = objectContext;
		}

		public List<RegisteredEntity> GetRegisteredEntities()
		{						
			lock (objectContext)
			{
				if (registeredEntities == null)
				{				
					List<EntityType> entityTypes = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).ToList();
					registeredEntities = (from entityType in entityTypes
										  let type = (Type)entityType.GetType().GetProperty("ClrType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(entityType)
										  orderby entityType.FullName
						select new RegisteredEntity
						{
							NamespaceName = entityType.NamespaceName,
							ClassName = entityType.Name,
							FullName = entityType.FullName,
							Type = type,
							Properties = GetProperties(entityType, type)
						}).ToList();
				}
				return new List<RegisteredEntity>(registeredEntities);
			}
		}
		private List<RegisteredEntity> registeredEntities;

		private List<RegisteredProperty> GetProperties(EntityType entityType, Type type)
		{
			return (from property in entityType.DeclaredProperties
				select new RegisteredProperty
				{
					PropertyName = property.Name,
					Type = property.IsPrimitiveType
							? ((PrimitiveType)property.TypeUsage.EdmType).ClrEquivalentType							
							: null,
					Nullable = property.Nullable,
					MaxLength = GetMaxLength(type, property)
				}).ToList();
		}

		private static int? GetMaxLength(Type type, EdmProperty property)
		{
			MaxLengthAttribute maxLengthAttribute = type.GetProperty(property.Name).GetCustomAttribute<MaxLengthAttribute>();
			return (maxLengthAttribute != null) ? maxLengthAttribute.Length : (int?)null;
		}
	}
}
