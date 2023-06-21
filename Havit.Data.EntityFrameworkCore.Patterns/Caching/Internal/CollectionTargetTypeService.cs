using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class CollectionTargetTypeService : ICollectionTargetTypeService
{
	private readonly ICollectionTargetTypeStorage collectionTargetTypeStorage;
	private readonly IDbContext dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public CollectionTargetTypeService(ICollectionTargetTypeStorage collectionTargetTypeStorage, IDbContext dbContext)
	{
		this.collectionTargetTypeStorage = collectionTargetTypeStorage;
		this.dbContext = dbContext;
	}

	/// <inheritdoc />
	public Type GetCollectionTargetType(Type type, string propertyName)
	{
		if (collectionTargetTypeStorage.Value == null)
		{
			lock (collectionTargetTypeStorage)
			{
				if (collectionTargetTypeStorage.Value == null)
				{
					collectionTargetTypeStorage.Value = dbContext.Model.GetApplicationEntityTypes()
					.SelectMany(entityType => entityType.GetNavigations())
					.Where(navigation => navigation.IsCollection)
					.ToDictionary(
						navigation => new TypePropertyName(navigation.DeclaringEntityType.ClrType, navigation.Name),
						navigation => navigation.TargetEntityType.ClrType);
				}
			}
		}

		if (collectionTargetTypeStorage.Value.TryGetValue(new TypePropertyName(type, propertyName), out Type result))
		{
			return result;
		}
		else
		{
			throw new InvalidOperationException(String.Format("Target type of entity type {0} and property {1} not found.", type.FullName, propertyName));
		}
	}
}
