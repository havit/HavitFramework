using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	internal class PropertyLambdaExpressionStore : IPropertyLambdaExpressionStore
	{
		private ConcurrentDictionary<StoreKey, object> storyDictionary = new ConcurrentDictionary<StoreKey, object>();

		public bool TryGet<TEntity, TProperty>(string propertyName, out PropertyLambdaExpression<TEntity, TProperty> result)
		{
			if (storyDictionary.TryGetValue(GetStoreKey<TEntity, TProperty>(propertyName), out object dictionaryValue))
			{
				result = (PropertyLambdaExpression<TEntity, TProperty>) dictionaryValue;
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}

		public void Store<TEntity, TProperty>(string propertyName, PropertyLambdaExpression<TEntity, TProperty> propertyLambdaExpression)
		{
			storyDictionary.TryAdd(GetStoreKey<TEntity, TProperty>(propertyName), propertyLambdaExpression);
		}

		private StoreKey GetStoreKey<TEntity, TProperty>(string propertyName)
		{
			return new StoreKey(typeof(TEntity), propertyName);
		}

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
