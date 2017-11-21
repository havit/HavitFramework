using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	/// <summary>
	/// Úložiště PropertyLambdaExpression.
	/// </summary>
	internal class PropertyLambdaExpressionStore : IPropertyLambdaExpressionStore
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
			return new StoreKey(typeof(TEntity), propertyName);
		}

		/// <summary>
		/// Reprezentuje klíč do úložiště.
		/// </summary>
		internal class StoreKey
		{
			public Type EntityType { get; }
			public string PropertyName { get; }

			public StoreKey(Type entityType, string propertyName)
			{
				EntityType = entityType;
				PropertyName = propertyName;
			}

			public override int GetHashCode()
			{
				return EntityType.GetHashCode() ^ PropertyName.GetHashCode();
			}
			
			public override bool Equals(object obj)
			{
				StoreKey objStoreKey = obj as StoreKey;
				if (objStoreKey == null)
				{
					return false;
				}

				return (this.EntityType == objStoreKey.EntityType) && (this.PropertyName == objStoreKey.PropertyName);
			}
		}
	}
}
