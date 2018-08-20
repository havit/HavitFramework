using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havit.Data.Patterns.DataLoaders;

namespace Havit.Data.Entity.Patterns.DataLoaders
{
	/// <summary>
	/// Implementace IFluentDataLoader, která na veškerá volání vyhazuje NotSupportedException.
	/// </summary>
	public class NotSuportedFluentDataLoader<TEntity> : IFluentDataLoader<TEntity>, IFluentDataLoaderAsync<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Vyhazuje výjimku NotSupportedException.
		/// </summary>
		IFluentDataLoader<TProperty> IFluentDataLoader<TEntity>.Load<TProperty>(Expression propertyPath)
		{
			throw CreateNotSupportedException();
		}

		/// <summary>
		/// Vyhazuje výjimku NotSupportedException.
		/// </summary>
		Task<IFluentDataLoaderAsync<TProperty>> IFluentDataLoaderAsync<TEntity>.LoadAsync<TProperty>(Expression propertyPath)
		{
			throw CreateNotSupportedException();
		}

		/// <summary>
		/// Vyhazuje výjimku NotSupportedException.
		/// </summary>
		IFluentDataLoader<TWrappedEntity> IFluentDataLoader<TEntity>.Unwrap<TWrappedEntity>()
		{
			throw CreateNotSupportedException();
		}

		/// <summary>
		/// Vyhazuje výjimku NotSupportedException.
		/// </summary>
		IFluentDataLoaderAsync<TWrappedEntity> IFluentDataLoaderAsync<TEntity>.Unwrap<TWrappedEntity>()
		{
			throw CreateNotSupportedException();
		}

		private NotSupportedException CreateNotSupportedException()
		{
			return new NotSupportedException("Fluent API on DataLoader for Entity Framework 6 is not supported.");
		}
	}
}