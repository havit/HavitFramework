using System;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Bázová třída pro předpis seedovaných dat.
	/// </summary>
	/// <example><code>
	/// public override void SeedData()
	/// {
	///		Language czechLanguage = languageEntries.Czech;
	/// 
	///		StavZapisu draft = new StavZapisu
	///		{
	///			Symbol = StavZapisu.Entry.Draft.ToString(),
	///			Localizations = new List&lt;StavZapisuLocalization&gt;
	///			{
	/// 			new StavZapisuLocalization { LanguageId = czechLanguage.Id, Nazev = "Rozpracovaný" }
	///			}
	///		};
	/// 
	///		StavZapisu final = new StavZapisu
	///		{
	/// 		Symbol = StavZapisu.Entry.Final.ToString(),
	///			Localizations = new List&lt;StavZapisuLocalization&gt;
	///			{
	///				new StavZapisuLocalization { LanguageId = czechLanguage.Id, Nazev = "Úplný" }
	///			}
	///		};
	///
	///		Seed(For(draft, final));			
	///	}
	///	</code> </example>
	public abstract class DataSeed : IDataSeed
	{
		private IDataSeedPersister currentDataSeedPersister;

		/// <summary>
		/// Předpis seedování dat.
		/// </summary>
		public abstract void SeedData();

		/// <summary>
		/// Vrací seznam (typů) DataSeedů, na kterých je seedování závislé, tj. vrací seznam dataseedů, které musejí být zpracovány před tímto data seedem.
		/// Ve výchozí implementaci vrací prázdný seznam.
		/// </summary>
		public virtual IEnumerable<Type> GetPrerequisiteDataSeeds()
		{
			return Enumerable.Empty<Type>();
		}

		/// <summary>
		/// Provede persistenci seedovaných dat. Učeno pro volání z implementace metody SeedData.
		/// </summary>
		protected void Seed<TEntity>(IDataSeedFor<TEntity> dataSeedFor)
			where TEntity : class
		{
			currentDataSeedPersister.Save<TEntity>(dataSeedFor.Configuration);
		}

		/// <summary>
		/// Získá objekt pro konfiguraci seedování dat.
		/// </summary>
		/// <param name="data">Objekty, které mají být seedovány.</param>
		public static IDataSeedFor<TEntity> For<TEntity>(params TEntity[] data)
			where TEntity : class
		{
			return new DataSeedFor<TEntity>(data);
		}

		/// <summary>
		/// Provede seedování dat s persistencí.
		/// </summary>
		void IDataSeed.SeedData(IDataSeedPersister dataSeedPersister)
		{
			this.currentDataSeedPersister = dataSeedPersister;
			SeedData();
			this.currentDataSeedPersister = null;
		}

	}
}
