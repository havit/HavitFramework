using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.DataLoaders.Fluent
{
	/// <summary>
	/// Extension metody k <see cref="IDataLoaderFluentAsync{TEntity}"/> umožňující pohodlné zřetězení volání asynchronních metod.
	/// <code>await dbDataLoader.LoadAsync(item, item => item.Property1).LoadAsync(property1 => property1.Property2);</code>
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
		public static Task<IDataLoaderFluentAsync<TEntity>> Where<TEntity>(this Task<IDataLoaderFluentAsync<TEntity>> dbDataLoaderForTask, Func<TEntity, bool> predicate)
			where TEntity : class
		{
			return dbDataLoaderForTask.ContinueWith(t => t.Result.Where(predicate));
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="dbDataLoaderForTask">IDbDataLoaderForAsync</param>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public static Task<IDataLoaderFluentAsync<TProperty>> LoadAsync<TEntity, TProperty>(this Task<IDataLoaderFluentAsync<TEntity>> dbDataLoaderForTask, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return dbDataLoaderForTask.ContinueWith(t => t.Result.LoadAsync(propertyPath)).Unwrap();
		}
	}
}