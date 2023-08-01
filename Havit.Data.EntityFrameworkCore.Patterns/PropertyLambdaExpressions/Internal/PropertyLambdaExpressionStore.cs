using System.Collections.Concurrent;

namespace Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;

/// <summary>
/// Úložiště PropertyLambdaExpression.
/// </summary>
public class PropertyLambdaExpressionStore : IPropertyLambdaExpressionStore
{
	private readonly ConcurrentDictionary<StoreKey, object> storeDictionary = new ConcurrentDictionary<StoreKey, object>();

	/// <summary>
	/// Vrací PropertyLambdaExpression, pokud v úložiští existuje.
	/// </summary>
	public bool TryGet<TEntity, TProperty>(string propertyName, out PropertyLambdaExpression<TEntity, TProperty> result)
	{
		if (storeDictionary.TryGetValue(GetStoreKey<TEntity, TProperty>(propertyName), out object dictionaryValue))
		{
			result = (PropertyLambdaExpression<TEntity, TProperty>)dictionaryValue;
			return true;
		}
		else
		{
			result = null;
			return false;
		}
	}

	/// <summary>
	/// Uloží PropertyLambdaExpression do úložiště.
	/// </summary>
	public void Store<TEntity, TProperty>(string propertyName, PropertyLambdaExpression<TEntity, TProperty> propertyLambdaExpression)
	{
		storeDictionary.TryAdd(GetStoreKey<TEntity, TProperty>(propertyName), propertyLambdaExpression);
	}

	private StoreKey GetStoreKey<TEntity, TProperty>(string propertyName)
	{
		return new StoreKey(typeof(TEntity), typeof(TProperty), propertyName);
	}

	/// <summary>
	/// Reprezentuje klíč do úložiště.
	/// </summary>
	internal sealed class StoreKey
	{
		public Type EntityType { get; }
		public Type PropertyType { get; }
		public string PropertyName { get; }

		public StoreKey(Type entityType, Type propertyType, string propertyName)
		{
			EntityType = entityType;
			PropertyType = propertyType;
			PropertyName = propertyName;
		}

		public override int GetHashCode()
		{
			return EntityType.GetHashCode() ^ PropertyType.GetHashCode() ^ PropertyName.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			StoreKey objStoreKey = obj as StoreKey;
			if (objStoreKey == null)
			{
				return false;
			}

			return (this.EntityType == objStoreKey.EntityType) && (this.PropertyType == objStoreKey.PropertyType) && (this.PropertyName == objStoreKey.PropertyName);
		}
	}
}
