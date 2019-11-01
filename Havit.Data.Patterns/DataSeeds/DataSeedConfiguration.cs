using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Reprezentuje konfiguraci seedování - párování, callback, atp.
	/// </summary>
	public class DataSeedConfiguration<TEntity>
	{
		/// <summary>
		/// Seedovaná data.
		/// </summary>
		public IEnumerable<TEntity> SeedData { get; private set; }

		/// <summary>
		/// Indikuje, zda je povolen update seedovaných vlastností. Pokud jení povolen, provádí se jen insert neexistujících objektů.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UpdateEnabled { get; set; }

		/// <summary>
		/// Párování objektů.
		/// </summary>
		public List<Expression<Func<TEntity, object>>> PairByExpressions { get; set; }

		/// <summary>
		/// Vlastnosti, které nejsou updatovány.
		/// </summary>
		public List<Expression<Func<TEntity, object>>> ExcludeUpdateExpressions { get; set; }

		/// <summary>
		/// Akce, které jsou provedeny před uložením (persistencí) seedovaných dat.
		/// </summary>
		public List<Action<BeforeSaveDataArgs<TEntity>>> BeforeSaveActions { get; set; }

		/// <summary>
		/// Akce, které jsou provedeny po uložení (persistenci) seedovaných dat.
		/// </summary>
		/// <example>Nastavení ParentId lokalizačních objektů (default u lokalizovaných objektů).</example>
		public List<Action<AfterSaveDataArgs<TEntity>>> AfterSaveActions { get; set; }

		/// <summary>
		/// Závislé seedování dat.		
		/// </summary>
		/// <example>
		/// Seedování lokalizací objektů (default u lokalizovaných objektů).
		/// </example>
		public List<ChildDataSeedConfigurationEntry> ChildrenSeeds { get; set; }

		/// <summary>
		/// Podmínka, která se má použít při načtení dat z databáze.
		/// Musí zajistit načtení všech potřebných záznamů.
		/// Avak může způsobit načtení i nepotřených záznamů, s tím se seedování vypořádá a ignoruje je.
		/// </summary>
		public Expression<Func<TEntity, bool>> CustomQueryCondition { get; set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="seedData">Seedovaná data.</param>
		public DataSeedConfiguration(TEntity[] seedData)
		{
			this.SeedData = seedData;
			this.UpdateEnabled = true;			
		}
	}
}
