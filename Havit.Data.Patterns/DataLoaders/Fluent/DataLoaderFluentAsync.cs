using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.DataLoaders.Fluent
{
	/// <summary>
	/// Rozšíření fluent API pro explicity data loader.
	/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
	/// Načítání lze zřetězit.
	/// <code>dataLoader.For(...).Load(item => item.Property1).Load(property1 => property1.Property2);</code>
	/// </summary>
	public class DataLoaderFluentAsync<TEntity> : IDataLoaderFluentAsync<TEntity>
		where TEntity : class
	{
		private readonly IDataLoaderAsync dataLoaderAsync;
		private readonly TEntity[] entities;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DataLoaderFluentAsync(IDataLoaderAsync dataLoaderAsync, TEntity[] entities)
		{
			this.dataLoaderAsync = dataLoaderAsync;
			this.entities = entities;
		}

		/// <summary>
		/// Omezuje načtení jen některých záznamů při zřetězení načítání.
		/// </summary>
		/// <param name="predicate">Podmínka, kterou musí splnit záznamy, aby byla načteny následujícím loadem.</param>
		/// <example>
		/// <code>dataLoader.For(subjekt).Load(item =&gt; item.Faktury).Where(faktura =&gt; faktura.Castka &gt; 0).Load(faktura =&gt; faktura.RadkyFaktury);</code>
		/// </example>
		public IDataLoaderFluentAsync<TEntity> Where(Func<TEntity, bool> predicate)
		{
			return new DataLoaderFluentAsync<TEntity>(dataLoaderAsync, entities.Where(predicate).ToArray());
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public Task<IDataLoaderFluentAsync<TProperty>> LoadAsync<TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TProperty : class
		{
			return dataLoaderAsync.LoadAllAsync(entities, propertyPath);
		}
	}
}
