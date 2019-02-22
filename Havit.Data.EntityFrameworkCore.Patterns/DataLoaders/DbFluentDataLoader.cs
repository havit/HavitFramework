using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Podpora Fluent API pro DbDataLoader.
	/// </summary>
	internal class DbFluentDataLoader<TEntity> : IFluentDataLoader<TEntity>, IFluentDataLoaderAsync<TEntity>
		where TEntity : class
	{
		internal DbDataLoader Loader { get; }
		internal TEntity[] Data { get; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbFluentDataLoader(DbDataLoader loader, TEntity[] data)
		{
			this.Loader = loader;
			this.Data = data;
		}

		/// <inheritdoc />
		IFluentDataLoader<TProperty> IFluentDataLoader<TEntity>.Load<TProperty>(Expression propertyPath)
		{
			return Loader.LoadAll(Data, (Expression<Func<TEntity, TProperty>>)propertyPath);
		}

		/// <inheritdoc />
		async Task<IFluentDataLoaderAsync<TProperty>> IFluentDataLoaderAsync<TEntity>.LoadAsync<TProperty>(Expression propertyPath)
		{
			return await Loader.LoadAllAsync(Data, (Expression<Func<TEntity, TProperty>>)propertyPath).ConfigureAwait(false);
		}

		/// <inheritdoc />
		IFluentDataLoaderAsync<TWrappedEntity> IFluentDataLoaderAsync<TEntity>.Unwrap<TWrappedEntity>()
		{
			return this.Unwrap<TWrappedEntity>();
		}

		/// <inheritdoc />
		IFluentDataLoader<TWrappedEntity> IFluentDataLoader<TEntity>.Unwrap<TWrappedEntity>()
		{
			return this.Unwrap<TWrappedEntity>();
		}

		private DbFluentDataLoader<TWrappedEntity> Unwrap<TWrappedEntity>()
			where TWrappedEntity : class
		{
			if (typeof(IEnumerable<TWrappedEntity>).IsAssignableFrom(typeof(TEntity)))
			{
				TWrappedEntity[] unwrappedData = Data.Cast<IEnumerable<TWrappedEntity>>().SelectMany(item => item).ToArray();
				return new DbFluentDataLoader<TWrappedEntity>(this.Loader, unwrappedData);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

	}
}