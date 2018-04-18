using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.ModelConfiguration.Edm;

namespace Havit.Data.Entity.Mapping.Internal
{
	/// <summary>
	/// It will get registered entities from the ObjectContext.
	/// </summary>
	public static class MappingApiExtensions
	{
		private static List<RegisteredEntity> registeredEntities;
		private static Converter converter;

		static MappingApiExtensions()
		{
			converter = new Converter();
		}

		/// <summary>
		/// It will get RegisteredEntities.
		/// </summary>
		public static RegisteredEntity[] GetRegisteredEntities(this DbContext ctx)
		{
			ObjectContext objectContext = ((IObjectContextAdapter)ctx).ObjectContext;

			List<RegisteredEntity> result = GetRegisteredEntities(objectContext);

			return result.ToArray();
		}

		/// <summary>
		/// It will get propriate RegisteredEntity with same type.
		/// </summary>
		public static RegisteredEntity GetRegisteredEntities(this DbContext ctx, Type type)
		{
			return GetRegisteredEntities(ctx).FirstOrDefault(r => r.Type == type);
		}

		/// <summary>
		/// It will get RegisteredEntities.
		/// </summary>
		public static List<RegisteredEntity> GetRegisteredEntities(ObjectContext objectContext)
		{
			lock (objectContext)
			{
				if (registeredEntities == null)
				{
					List<EntityType> entityTypes = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).ToList();

					registeredEntities = (from entityType in entityTypes
										  let type = (Type)entityType.GetType().GetProperty("ClrType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(entityType)
										  where type != typeof(Model.DataSeedVersion)
										  orderby entityType.FullName
										  select new RegisteredEntity
										  {
											  NamespaceName = entityType.NamespaceName,
											  ClassName = entityType.Name,
											  FullName = entityType.FullName,
											  Type = type,
											  HasEntryEnum = type.GetNestedType("Entry")?.IsEnum ?? false,
											  HasDatabaseGeneratedIdentity = HasDatabaseGeneratedIdentity(entityType, objectContext), // TODO: TW: RegisteredProperty.StoreGeneratedPattern zatím nefunguje, vracíme se tedy k HasDatabaseGeneratedIdentity, pak je třeba dořešit
											  Properties = GetProperties(entityType, type, objectContext),
											  PrimaryKeys = GetPrimaryKeys(entityType, type, objectContext),
										  }).ToList();
				}
				return new List<RegisteredEntity>(registeredEntities);
			}
		}

		private static List<RegisteredProperty> GetPrimaryKeys(EntityType entityType, Type type, ObjectContext objectContext)
		{
			var result = entityType.KeyProperties.Select(p => converter.EdmMemberToRegisteredProperty(p, type, entityType, objectContext)).ToList();

			return result;
		}

		private static List<RegisteredProperty> GetProperties(EntityType entityType, Type type, ObjectContext objectContext)
		{
			List<RegisteredProperty> result =
				entityType.DeclaredMembers.Select(p => converter.EdmMemberToRegisteredProperty(p, type, entityType, objectContext)).ToList();

			return result;
		}

		private static NavigationProperty GetNavigationProperty(string propertyName, EntityType entityType)
		{
			NavigationProperty result = entityType.DeclaredNavigationProperties.Where(dnp => dnp.Name.Equals(propertyName)).FirstOrDefault();

			return result;
		}

		private static bool IsNavigationProperty(EdmMember member)
		{
			bool result = member.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty;

			return result;
		}

		private static int? GetMaxLength(Type type, string propertyName)
		{
			MaxLengthAttribute maxLengthAttribute = type.GetProperty(propertyName).GetCustomAttribute<MaxLengthAttribute>();
			return (maxLengthAttribute != null) ? maxLengthAttribute.Length : (int?)null;
		}

		private static bool HasDatabaseGeneratedIdentity(EntityType entityType, ObjectContext objectContext)
		{
			EntitySet entitySet = objectContext.MetadataWorkspace
				.GetItems<EntityContainer>(DataSpace.SSpace)
				.First()
				.BaseEntitySets
				.OfType<EntitySet>()
				.Single(item => item.Name == entityType.Name);

			return entitySet.ElementType.KeyProperties.Any(item => item.IsStoreGeneratedIdentity);
		}

		internal class Converter
		{
			internal RegisteredProperty EdmMemberToRegisteredProperty(EdmMember member, Type type, EntityType entityType, ObjectContext objectContext)
			{
				var result = new RegisteredProperty()
				{
					
					PropertyName = member.Name,
					Type = ((member is EdmProperty) && (((EdmProperty)member).IsPrimitiveType))
						? ((PrimitiveType)member.TypeUsage.EdmType).ClrEquivalentType
						: null,
					MaxLength = GetMaxLength(type, member.Name),
					//ForeignKey = , // Vyplnit pokud se jedná o navigation property
					//IsIdentity = , // nahrazuje ji StoreGeneratedPattern
					//IsNavigationProperty = IsNavigationProperty(member, entityType),
					IsNavigationProperty = IsNavigationProperty(member),
					//NavigationProperty = GetNavigationProperty(property, entityType),
					IsStoreGeneratedIdentity = member.IsStoreGeneratedIdentity,
				};

				if (member is NavigationProperty)
				{
					result.Nullable = true;

					EntityType cEntityType = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.CSpace).Single(item => item.Name == ((NavigationProperty)member).DeclaringType.Name);
					IEnumerable<EdmProperty> foreignKeys = ((NavigationProperty)cEntityType.DeclaredMembers.Single(m => m.Name == member.Name)).GetDependentProperties();
					result.ForeignKey = foreignKeys.FirstOrDefault();
					result.ForeignKeys.AddRange(foreignKeys);
				}
				else if (member is EdmProperty)
				{
					// StoreGeneratedPatter:
					// OSpace Child {Id-None, MasterId-None, Master}
					// SSpace Child {ChildId-Identity, MasterId-None}
					// CSpace Child {Master, Id-None, MasterId-None}
					// TODO: TW: Vymyslet, jak přes OSpace.member dohledat patřičný SSpace.member (mohou mít různé názvy)
					result.StoreGeneratedPattern = ((EdmProperty)member).StoreGeneratedPattern; // je vždy None - tato informace je obsažena v SSpace
					//var sEntityType = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.SSpace).Single(item => item.Name == ((EdmProperty)member).DeclaringType.Name);
					//result.StoreGeneratedPattern = ((EdmProperty) sEntityType.DeclaredMembers.Where(dm => dm.Name == member.Name))
					//	.StoreGeneratedPattern;
					result.Nullable = ((EdmProperty)member).Nullable;
					result.IsKeyProperty = entityType.KeyProperties.Contains(member.Name);

					//EntitySet entitySet = objectContext.MetadataWorkspace
					//	.GetItems<EntityContainer>(DataSpace.SSpace)
					//	.First()
					//	.BaseEntitySets
					//	.OfType<EntitySet>()
					//	.Single(item => item.Name == entityType.Name);
				}

				return result;
			}
		}
	}
}
