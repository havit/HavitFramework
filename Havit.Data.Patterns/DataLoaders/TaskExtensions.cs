using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Extension metody k <see cref="IDbDataLoaderForAsync{TEntity}"/> umožňující pohodlné zřetězení volání asynchronních metod.
	/// <code>await dbDataLoader.For(...).LoadAsync(item => item.Property1).LoadAsync(property1 => property1.Property2);</code>
	/// </summary>
	public static class TaskExtensions
	{
		/// <summary>
		/// Omezuje načtení jen některých záznamů při zřetězení načítání.
		/// </summary>
		/// <param name="dbDataLoaderForTask">IDbDataLoaderForAsync</param>
		/// <param name="predicate">Podmínka, kterou musí splnit záznamy, aby byla načteny následujícím loadem.</param>
		/// <example>
		/// dbDataLoader.For(subjekt).Load(item => item.Faktury).Where(faktura => faktura.Castka > 0).Load(faktura => faktura.RadkyFaktury);
		/// </example>
		public static Task<IDbDataLoaderForAsync<TEntity>> Where<TEntity>(this Task<IDbDataLoaderForAsync<TEntity>> dbDataLoaderForTask, Func<TEntity, bool> predicate)
			where TEntity : class
		{
			return dbDataLoaderForTask.ContinueWith(t => t.Result.Where(predicate));
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="dbDataLoaderForTask">IDbDataLoaderForAsync</param>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public static Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TEntity, TProperty>(this Task<IDbDataLoaderForAsync<TEntity>> dbDataLoaderForTask, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return dbDataLoaderForTask.ContinueWith(t => t.Result.LoadAsync(propertyPath)).Unwrap();
		}

		/// <summary>
		/// Asynchronně načte kolekci třídy, pokud již není načtena.
		/// Nenačtená kolekce má hodnotu null, načtená kolekce má instanci.
		/// </summary>
		/// <param name="dbDataLoaderForTask">IDbDataLoaderForAsync</param>
		/// <param name="propertyPath">Vlastnost kolekce, která má být načtena.</param>
		public static Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TEntity, TProperty>(this Task<IDbDataLoaderForAsync<TEntity>> dbDataLoaderForTask, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return dbDataLoaderForTask.ContinueWith(t => t.Result.LoadAsync(propertyPath)).Unwrap();
		}

		/// <summary>
		/// Asynchronně načte kolekci třídy, pokud již není načtena.
		/// Nenačtená kolekce má hodnotu null, načtená kolekce má instanci.
		/// </summary>
		/// <param name="dbDataLoaderForTask">IDbDataLoaderForAsync</param>
		/// <param name="propertyPath">Vlastnost kolekce, která má být načtena.</param>
		public static Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TEntity, TProperty>(this Task<IDbDataLoaderForAsync<TEntity>> dbDataLoaderForTask, Expression<Func<TEntity, ICollection<TProperty>>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return dbDataLoaderForTask.ContinueWith(t => t.Result.LoadAsync(propertyPath)).Unwrap();
		}

		/// <summary>
		/// Asynchronně načte kolekci třídy, pokud již není načtena.
		/// Nenačtená kolekce má hodnotu null, načtená kolekce má instanci.
		/// </summary>
		/// <param name="dbDataLoaderForTask">IDbDataLoaderForAsync</param>
		/// <param name="propertyPath">Vlastnost kolekce, která má být načtena.</param>
		public static Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TEntity, TProperty>(this Task<IDbDataLoaderForAsync<TEntity>> dbDataLoaderForTask, Expression<Func<TEntity, List<TProperty>>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return dbDataLoaderForTask.ContinueWith(t => t.Result.LoadAsync(propertyPath)).Unwrap();
		}

	}
}