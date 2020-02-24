using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Extension metody k data loaderu pro možnost použití fluent API.
	/// </summary>
	public static class DataLoaderExtensions
	{
		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		public static IFluentDataLoader<TProperty> ThenLoad<TEntity, TProperty>(this IFluentDataLoader<TEntity> source, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return source.Load<TProperty>(propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		public static IFluentDataLoader<TProperty> ThenLoad<TEntity, TProperty>(this IFluentDataLoader<IEnumerable<TEntity>> source, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return source.Unwrap<TEntity>().Load<TProperty>(propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		public static async Task<IFluentDataLoader<TProperty>> ThenLoadAsync<TEntity, TProperty>(this Task<IFluentDataLoader<TEntity>> source, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return await (await source.ConfigureAwait(false)).LoadAsync<TProperty>(propertyPath).ConfigureAwait(false);
		}

		// **************************************************************************************************************************************
		//
		// Protože Task<...> není Task<out ...>, není možné extension metody použít hezky, jako v případě synchronních metod na IEnumerable.
		// Proto zde máme několik přetížení nad ICollection, IList, List, atp.
		//
		// **************************************************************************************************************************************

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		public static async Task<IFluentDataLoader<TProperty>> ThenLoadAsync<TEntity, TProperty>(this Task<IFluentDataLoader<ICollection<TEntity>>> source, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return await (await source.ConfigureAwait(false)).Unwrap<TEntity>().LoadAsync<TProperty>(propertyPath).ConfigureAwait(false);
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		public static async Task<IFluentDataLoader<TProperty>> ThenLoadAsync<TEntity, TProperty>(this Task<IFluentDataLoader<IList<TEntity>>> source, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return await (await source.ConfigureAwait(false)).Unwrap<TEntity>().LoadAsync<TProperty>(propertyPath).ConfigureAwait(false);
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		public static async Task<IFluentDataLoader<TProperty>> ThenLoadAsync<TEntity, TProperty>(this Task<IFluentDataLoader<List<TEntity>>> source, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return await (await source.ConfigureAwait(false)).Unwrap<TEntity>().LoadAsync<TProperty>(propertyPath).ConfigureAwait(false);
		}
	}
}