using Havit.Data.EntityFrameworkCore.Internal;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore;

/// <inheritdoc cref="Microsoft.EntityFrameworkCore.DbContext" />
public abstract class DbContext : Microsoft.EntityFrameworkCore.DbContext, IDbContext
{
	/// <summary>
	/// Registr akcí k provedení po uložení změn.
	/// </summary>
	private List<Action> _afterSaveChangesActions;

	private Dictionary<Type, object> _dbSetsDictionary;

#pragma warning disable EF1001 // Internal EF Core API usage.
	/// <summary>
	/// StateManager DbContextu.
	/// Z výkonových důvodů (úspora alokací EntityEntry/NavigationEntry, closures a delegátů na často volaných cestách) používáme interní EF Core API.
	/// </summary>
	internal IStateManager StateManager => _stateManager ??= this.GetService<IStateManager>();
	private IStateManager _stateManager;
#pragma warning restore EF1001 // Internal EF Core API usage.

	/// <summary>
	/// Konstruktor. Viz <see cref="Microsoft.EntityFrameworkCore.DbContext()"/>.
	/// </summary>
	protected DbContext() : this(new DbContextOptions<DbContext>())
	{

	}

	/// <summary>
	/// Konstruktor. Viz <see cref="Microsoft.EntityFrameworkCore.DbContext(DbContextOptions)"/>.
	/// </summary>
	protected DbContext(DbContextOptions options) : base(options)
	{
	}

	/// <inheritdoc />
	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		base.ConfigureConventions(configurationBuilder);

		var conventionsOptions = GetConventionOptions();

		if (conventionsOptions.CacheAttributeToAnnotationConventionEnabled)
		{
			configurationBuilder.Conventions.Add(sp => new CacheAttributeToAnnotationConvention(sp.GetRequiredService<ProviderConventionSetBuilderDependencies>()));
		}

		if (conventionsOptions.CascadeDeleteToRestrictConventionEnabled)
		{
			// SQL Server provider nahrazuje (Replace) konvenci CascadeDeleteConvention konvencí SqlServerOnDeleteConvention,
			// u jiných providerů (např. InMemory v testech) zůstává v sadě CascadeDeleteConvention. Odebíráme proto obě,
			// aby naše konvence byla jedinou cascade delete konvencí v sadě (a nespoléhali jsme na pořadí konvencí).
			configurationBuilder.Conventions.Remove(typeof(SqlServerOnDeleteConvention));
			configurationBuilder.Conventions.Remove(typeof(CascadeDeleteConvention));
			configurationBuilder.Conventions.Add(sp => new CascadeDeleteToRestrictConvention(sp.GetRequiredService<ProviderConventionSetBuilderDependencies>()));
		}

		if (conventionsOptions.DataTypeAttributeConventionEnabled)
		{
			configurationBuilder.Conventions.Add(sp => new DataTypeAttributeConvention(sp.GetRequiredService<ProviderConventionSetBuilderDependencies>()));
		}

		if (conventionsOptions.ManyToManyEntityKeyDiscoveryConventionEnabled)
		{
			configurationBuilder.Conventions.Add(_ => new ManyToManyEntityKeyDiscoveryConvention());
		}

		if (conventionsOptions.StringPropertiesDefaultValueConventionEnabled)
		{
			configurationBuilder.Conventions.Add(_ => new StringPropertiesDefaultValueConvention());
		}

		if (conventionsOptions.LocalizationTableIndexConventionEnabled)
		{
			configurationBuilder.Conventions.Add(_ => new LocalizationTableIndexConvention());
		}
	}

	/// <summary>
	/// Vrátí nastavení pro registraci konvencí.
	/// </summary>
	protected virtual ConventionsOptions GetConventionOptions()
	{
		return new ConventionsOptions();
	}


	/// <inheritdoc />
	protected override sealed void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		RegisterDataSeedVersion(modelBuilder);

		CustomizeModelCreating(modelBuilder);

		ModelCreatingCompleting(modelBuilder);
	}

	/// <summary>
	/// Zaregistruje třídu DataSeedVersion do modelu
	/// </summary>
	protected void RegisterDataSeedVersion(ModelBuilder modelBuilder)
	{
		EntityTypeBuilder<DataSeedVersion> dataSeedVersionEntity = modelBuilder.Entity<DataSeedVersion>();
		dataSeedVersionEntity.ToTable("__DataSeed");
		dataSeedVersionEntity.HasKey(item => item.ProfileName).HasName("PK_DataSeed");
		dataSeedVersionEntity.Property(item => item.Version);
	}

	/// <summary>
	/// Template metoda pro registraci modelu.
	/// </summary>
	protected virtual void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		// NOOP - template method
	}

	/// <summary>
	/// Metoda volaná po registraci modelu.
	/// </summary>
	protected virtual void ModelCreatingCompleting(ModelBuilder modelBuilder)
	{
		// NOOP - template method
	}

	/// <summary>
	/// Uloží registrované změny. Viz <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges(bool)"/>.
	/// </summary>
	public override int SaveChanges(bool acceptAllChangesOnSuccess)
	{
		int result = base.SaveChanges(acceptAllChangesOnSuccess);
		AfterSaveChanges();
		return result;
	}

	/// <summary>
	/// Uloží registrované změny. Viz <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(bool, System.Threading.CancellationToken)"/>.
	/// </summary>
	public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
	{
		int result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
		AfterSaveChanges();
		return result;
	}

	/// <summary>
	/// Zajišťuje volání registrovaných after save changes akcí.
	/// Spuštěno z metody SaveChanges po volání bázové SaveChanges(Async).
	/// </summary>
	protected internal virtual void AfterSaveChanges()
	{
		if (_afterSaveChangesActions != null)
		{
			List<Action> afterSaveChangesActions = _afterSaveChangesActions;
			// Registr vyčistíme před spuštěním akcí:
			// - pokud akce vyhodí výjimku, nedojde při dalším SaveChanges k opakovanému spuštění již proběhlých akcí,
			// - akce zaregistrovaná z jiné akce nemodifikuje enumerovanou kolekci (provede se po dalším SaveChanges).
			_afterSaveChangesActions = null;

			foreach (var afterSaveChangesAction in afterSaveChangesActions)
			{
				afterSaveChangesAction.Invoke();
			}
		}
	}

	/// <summary>
	/// Registruje akci k jednorázovému provedení po save changes. Akce je provedena metodou DbContext.AfterSaveChanges.
	/// Při opakovaném volání DbContext.SaveChanges není akce volána opakovaně.
	/// Akce je provedena po nejbližším úspěšném SaveChanges - pokud uložení změn selže, registrace zůstává (změny zůstávají v change trackeru rozpracované
	/// a akce se provede až po jejich úspěšném uložení). Při dispose DbContextu jsou neprovedené akce zahozeny.
	/// </summary>
	public void RegisterAfterSaveChangesAction(Action action)
	{
		if (_afterSaveChangesActions == null)
		{
			_afterSaveChangesActions = new List<Action>([action]);
		}
		else
		{
			_afterSaveChangesActions.Add(action);
		}
	}

	/// <summary>
	/// Provede akci s AutoDetectChangesEnabled nastaveným na false, přičemž je poté AutoDetectChangesEnabled nastaven na původní hodnotu.
	/// </summary>
	internal TResult ExecuteWithoutAutoDetectChanges<TResult>(Func<TResult> action)
	{
		if (ChangeTracker.AutoDetectChangesEnabled)
		{
			try
			{
				ChangeTracker.AutoDetectChangesEnabled = false;
				return action();
			}
			finally
			{
				ChangeTracker.AutoDetectChangesEnabled = true;
			}
		}
		else
		{
			return action();
		}
	}

	/// <summary>
	/// Provede akci s AutoDetectChangesEnabled nastaveným na false, přičemž je poté AutoDetectChangesEnabled nastaven na původní hodnotu.
	/// </summary>
	private async Task<TResult> ExecuteWithoutAutoDetectChangesAsync<TResult>(Func<Task<TResult>> actionAsync)
	{
		if (ChangeTracker.AutoDetectChangesEnabled)
		{
			try
			{
				ChangeTracker.AutoDetectChangesEnabled = false;
				return await actionAsync().ConfigureAwait(false);
			}
			finally
			{
				ChangeTracker.AutoDetectChangesEnabled = true;
			}
		}
		else
		{
			return await actionAsync().ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Vrátí objekty v daných stavech.
	/// </summary>
	IEnumerable<EntityEntry> IDbContext.GetEntries(bool suppressDetectChanges)
	{
		IEnumerable<EntityEntry> getObjectInStatesFunc() => this.ChangeTracker.Entries();

		return suppressDetectChanges
			? ExecuteWithoutAutoDetectChanges(getObjectInStatesFunc)
			: getObjectInStatesFunc();
	}

	/// <summary>
	/// Vrací true, pokud EF považuje vlastnost za načtenou.
	/// </summary>
#pragma warning disable EF1001 // Internal EF Core API usage.
	bool IDbContext.IsNavigationLoaded<TEntity>(TEntity entity, string propertyName)
	{
		// Z výkonových důvodů nepoužíváme this.Entry(entity).Navigation(propertyName).IsLoaded,
		// které alokuje EntityEntry a NavigationEntry a vyžaduje potlačení detekce změn.
		InternalEntityEntry entry = StateManager.TryGetEntry(entity);
		if (entry == null)
		{
			return false; // netrackovaná entita - navigaci nepovažujeme za načtenou
		}

		return entry.IsLoaded(GetNavigation(entry, propertyName));
	}
#pragma warning restore EF1001 // Internal EF Core API usage.

#pragma warning disable EF1001 // Internal EF Core API usage.
	void IDbContext.MarkNavigationAsLoaded<TEntity>(TEntity entity, string propertyName)
	{
		// Z výkonových důvodů nepoužíváme this.Entry(entity).Navigation(propertyName).IsLoaded = true,
		// které alokuje EntityEntry a NavigationEntry a vyžaduje potlačení detekce změn.
		InternalEntityEntry entry = StateManager.GetOrCreateEntry(entity);
		entry.SetIsLoaded(GetNavigation(entry, propertyName));
	}
#pragma warning restore EF1001 // Internal EF Core API usage.

#pragma warning disable EF1001 // Internal EF Core API usage.
	private static INavigationBase GetNavigation(InternalEntityEntry entry, string propertyName)
	{
		return entry.EntityType.FindNavigation(propertyName)
			?? (INavigationBase)entry.EntityType.FindSkipNavigation(propertyName)
			?? throw new InvalidOperationException($"Navigation '{propertyName}' was not found on entity type '{entry.EntityType.DisplayName()}'.");
	}
#pragma warning restore EF1001 // Internal EF Core API usage.

	/// <summary>
	/// Vrací DbSet pro danou entitu.
	/// Pro snazší možnost mockování konzumentů DbSetu je vytvořena abstrakce do interface IDbSet&lt;TEntity&gt;.
	/// </summary>
	IDbSet<TEntity> IDbContext.Set<TEntity>()
	{
		_dbSetsDictionary ??= new Dictionary<Type, object>();
		if (_dbSetsDictionary.TryGetValue(typeof(TEntity), out var foundDbSet))
		{
			return (IDbSet<TEntity>)foundDbSet;
		}

		var dbSetInternal = new DbSetInternal<TEntity>(this);
		_dbSetsDictionary.Add(typeof(TEntity), dbSetInternal);
		return dbSetInternal;
	}

	/// <summary>
	/// Vrací EntityEntry pro danou entitu.
	/// </summary>
#pragma warning disable EF1001 // Internal EF Core API usage.
	public EntityEntry GetEntry(object entity, bool suppressDetectChanges = true)
	{
		// Z výkonových důvodů nepoužíváme pro suppressDetectChanges potlačení detekce změn přes ChangeTracker.AutoDetectChangesEnabled
		// (alokace closure a delegátu), ale vytvoříme EntityEntry přímo ze state manageru (ten detekci změn nespouští).
		return suppressDetectChanges
			? new EntityEntry(StateManager.GetOrCreateEntry(entity))
			: this.Entry(entity);
	}
#pragma warning restore EF1001 // Internal EF Core API usage.

	/// <summary>
	/// Vrátí stav entity v DbContextu (resp. v jeho ChangeTrackeru).
	/// </summary>
#pragma warning disable EF1001 // Internal EF Core API usage.
	EntityState IDbContext.GetEntityState<TEntity>(TEntity entity)
	{
		return StateManager.TryGetEntry(entity)?.EntityState ?? EntityState.Detached;
	}
#pragma warning restore EF1001 // Internal EF Core API usage.

	/// <summary>
	/// Uloží změny.
	/// </summary>
	void IDbContext.SaveChanges()
	{
		this.SaveChanges();
	}

	/// <summary>
	/// Uloží změny.
	/// </summary>
	void IDbContext.SaveChanges(bool suppressDetectChanges)
	{
		if (suppressDetectChanges)
		{
			ExecuteWithoutAutoDetectChanges(() => this.SaveChanges());
		}
		else
		{
			this.SaveChanges();
		}
	}

	/// <summary>
	/// Uloží změny.
	/// </summary>
	Task IDbContext.SaveChangesAsync(CancellationToken cancellationToken)
	{
		return SaveChangesAsync(cancellationToken);
	}

	/// <summary>
	/// Uloží změny.
	/// </summary>
	async Task IDbContext.SaveChangesAsync(bool suppressDetectChanges, CancellationToken cancellationToken)
	{
		if (suppressDetectChanges)
		{
			await ExecuteWithoutAutoDetectChangesAsync(async () => await this.SaveChangesAsync(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
		}
		else
		{
			await this.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public override void Dispose()
	{
		// Neprovedené after save changes akce končí se životností contextu (u poolovaného DbContextu se zápůjčkou z poolu).
		// EF custom stav odvozeného DbContextu při vracení do poolu neresetuje, akce by tak přežily do dalšího použití instance z poolu.
		_afterSaveChangesActions = null;
		base.Dispose();
	}

	/// <inheritdoc />
	public override ValueTask DisposeAsync()
	{
		// Neprovedené after save changes akce končí se životností contextu (u poolovaného DbContextu se zápůjčkou z poolu).
		// EF custom stav odvozeného DbContextu při vracení do poolu neresetuje, akce by tak přežily do dalšího použití instance z poolu.
		_afterSaveChangesActions = null;
		return base.DisposeAsync();
	}
}