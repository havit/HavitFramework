using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.DataSeeds;

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
public abstract class DataSeed<TDataSeedProfile> : IDataSeed
	where TDataSeedProfile : IDataSeedProfile, new() // new() nás chrání před použitím abstraktní třídy Profile, jinou funkci zde nemá, instanci nevytváříme
{
	private IDataSeedPersister currentDataSeedPersister;

	/// <summary>
	/// Předpis seedování dat.
	/// </summary>
	public virtual void SeedData()
	{
		// NOOP
	}

	/// <summary>
	/// Předpis seedování dat.
	/// </summary>
	public virtual Task SeedDataAsync(CancellationToken cancellationToken)
	{
		// "NOOP"
		return Task.CompletedTask;
	}

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
	/// Provede persistenci seedovaných dat. Učeno pro volání z implementace metody SeedData.
	/// </summary>
	protected async Task SeedAsync<TEntity>(IDataSeedFor<TEntity> dataSeedFor, CancellationToken cancellationToken = default)
		where TEntity : class
	{
		await currentDataSeedPersister.SaveAsync<TEntity>(dataSeedFor.Configuration, cancellationToken).ConfigureAwait(false);
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
	/// Vrátí profil, do kterého daný předpis seedování patří.
	/// </summary>
	Type IDataSeed.ProfileType
	{
		get
		{
			return typeof(TDataSeedProfile);
		}
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

	/// <summary>
	/// Provede seedování dat s persistencí.
	/// </summary>
	async Task IDataSeed.SeedDataAsync(IDataSeedPersister dataSeedPersister, CancellationToken cancellationToken)
	{
		this.currentDataSeedPersister = dataSeedPersister;
		await SeedDataAsync(cancellationToken).ConfigureAwait(false);
		this.currentDataSeedPersister = null;
	}
}
