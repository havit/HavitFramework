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
	public class DataLoaderFluent<TEntity> : IDataLoaderFluent<TEntity>
		where TEntity : class
	{
		private readonly IDataLoader dataLoader;
		private readonly TEntity[] entities;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DataLoaderFluent(IDataLoader dataLoader, TEntity[] entities)
		{
			this.dataLoader = dataLoader;
			this.entities = entities;
		}

		/// <summary>
		/// Omezuje načtení jen některých záznamů při zřetězení načítání.
		/// </summary>
		/// <param name="predicate">Podmínka, kterou musí splnit záznamy, aby byla načteny následujícím loadem.</param>
		/// <example>
		/// <code>dataLoader.For(subjekt).Load(item =&gt; item.Faktury).Where(faktura =&gt; faktura.Castka &gt; 0).Load(faktura =&gt; faktura.RadkyFaktury);</code>
		/// </example>
		IDataLoaderFluent<TEntity> IDataLoaderFluent<TEntity>.Where(Func<TEntity, bool> predicate)
		{
			return new DataLoaderFluent<TEntity>(dataLoader, entities.Where(predicate).ToArray());
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public IDataLoaderFluent<TProperty> Load<TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TProperty : class
		{
			return dataLoader.LoadAll(entities, propertyPath);
		}
	}
}
