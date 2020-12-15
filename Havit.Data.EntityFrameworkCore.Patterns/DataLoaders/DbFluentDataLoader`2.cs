using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Podpora Fluent API pro DbDataLoader.
	/// Použit pro výsledek načtení kolekcí, zejména pro možnost implementace substituce (Xy -> XyIncludingDeleted).
	/// Protože vlastnost Xy, která je zapsána v kódu, má jiný datový typ než XyIncludingDeleted, pro kterou se data načítají, máme zde 2 generické parametry:
	/// - TContractEntity je typ vlastnosti Xy, použije se jen pro snadný zápis fluent API, jiný význam nemá.
	/// - TItem je typ, který je prvkem kolekce Xy.
	/// </summary>
	internal class DbFluentDataLoader<TContractEntity, TItem> : IFluentDataLoader<TContractEntity>
		where TContractEntity : class
		where TItem : class
	{
		internal DbDataLoader Loader { get; }
		internal TItem[] Data { get; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbFluentDataLoader(DbDataLoader loader, TItem[] data)
		{
			this.Loader = loader;
			this.Data = data;
		}

		/// <inheritdoc />
		IFluentDataLoader<TProperty> IFluentDataLoader<TContractEntity>.Load<TProperty>(Expression propertyPath)
		{
			return Loader.LoadAll(Data, (Expression<Func<TItem, TProperty>>)propertyPath);
		}

		/// <inheritdoc />
		async Task<IFluentDataLoader<TProperty>> IFluentDataLoader<TContractEntity>.LoadAsync<TProperty>(Expression propertyPath, CancellationToken cancellationToken /* no default */)
		{
			return await Loader.LoadAllAsync(Data, (Expression<Func<TItem, TProperty>>)propertyPath, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		IFluentDataLoader<TWrappedEntity> IFluentDataLoader<TContractEntity>.Unwrap<TWrappedEntity>()
		{
			return this.Unwrap<TWrappedEntity>();
		}

		private DbFluentDataLoader<TWrappedEntity, TWrappedEntity> Unwrap<TWrappedEntity>()
			where TWrappedEntity : class
		{
			if (typeof(IEnumerable<TWrappedEntity>).IsAssignableFrom(typeof(TContractEntity)))
			{
				TWrappedEntity[] unwrappedData = Data.Cast<TWrappedEntity>().ToArray();
				return new DbFluentDataLoader<TWrappedEntity, TWrappedEntity>(this.Loader, unwrappedData);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

	}
}