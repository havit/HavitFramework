﻿using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataLoaders;

/// <summary>
/// Podpora Fluent API pro DbDataLoader.
/// Použit pro výsledek načtení kolekcí, zejména pro možnost implementace substituce (Xy -> XyIncludingDeleted).
/// Protože vlastnost Xy, která je zapsána v kódu, má jiný datový typ než XyIncludingDeleted, pro kterou se data načítají, máme zde 2 generické parametry:
/// - TContractEntity je typ vlastnosti Xy, použije se jen pro snadný zápis fluent API, jiný význam nemá.
/// - TItem je typ, který je prvkem kolekce Xy.
/// </summary>
internal class FluentDataLoader<TContractEntity, TItem> : IFluentDataLoader<TContractEntity>
	where TContractEntity : class
	where TItem : class
{
	internal IDataLoader Loader { get; }
	internal IEnumerable<TItem> Data { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public FluentDataLoader(IDataLoader loader, IEnumerable<TItem> data)
	{
		Loader = loader;
		Data = data;
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

	private FluentDataLoader<TWrappedEntity, TWrappedEntity> Unwrap<TWrappedEntity>()
		where TWrappedEntity : class
	{
		if (typeof(IEnumerable<TWrappedEntity>).IsAssignableFrom(typeof(TContractEntity)))
		{
			IEnumerable<TWrappedEntity> unwrappedData = Data.Cast<TWrappedEntity>();
			return new FluentDataLoader<TWrappedEntity, TWrappedEntity>(this.Loader, unwrappedData);
		}
		else
		{
			throw new InvalidOperationException();
		}
	}

}