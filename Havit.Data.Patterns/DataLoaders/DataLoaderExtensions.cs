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
		#region ThenLoad
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
		#endregion

		#region ThenLoadAsync
		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		public static async Task<IFluentDataLoaderAsync<TProperty>> ThenLoadAsync<TEntity, TProperty>(this Task<IFluentDataLoaderAsync<TEntity>> source, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return await (await source).LoadAsync<TProperty>(propertyPath);
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
		public static async Task<IFluentDataLoaderAsync<TProperty>> ThenLoadAsync<TEntity, TProperty>(this Task<IFluentDataLoaderAsync<ICollection<TEntity>>> source, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return await (await source).Unwrap<TEntity>().LoadAsync<TProperty>(propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		public static async Task<IFluentDataLoaderAsync<TProperty>> ThenLoadAsync<TEntity, TProperty>(this Task<IFluentDataLoaderAsync<IList<TEntity>>> source, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return await (await source).Unwrap<TEntity>().LoadAsync<TProperty>(propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		public static async Task<IFluentDataLoaderAsync<TProperty>> ThenLoadAsync<TEntity, TProperty>(this Task<IFluentDataLoaderAsync<List<TEntity>>> source, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return await (await source).Unwrap<TEntity>().LoadAsync<TProperty>(propertyPath);
		}
		#endregion
	}
}