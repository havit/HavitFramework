using Havit.Data.EntityFrameworkCore.Internal;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
			configurationBuilder.Conventions.Remove(typeof(SqlServerOnDeleteConvention));
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
			foreach (var afterSaveChangesAction in _afterSaveChangesActions)
			{
				afterSaveChangesAction.Invoke();
			}
			_afterSaveChangesActions = null;
		}
	}

	/// <summary>
	/// Registruje akci k jednorázovému provedení po save changes. Akce je provedena metodou DbContext.AfterSaveChanges.
	/// Při opakovaném volání DbContext.SaveChanges není akce volána opakovaně.
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
	private TResult ExecuteWithoutAutoDetectChanges<TResult>(Func<TResult> action)
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
	bool IDbContext.IsNavigationLoaded<TEntity>(TEntity entity, string propertyName)
	{
		return GetEntry(entity, suppressDetectChanges: true).Navigation(propertyName).IsLoaded;
	}

	void IDbContext.MarkNavigationAsLoaded<TEntity>(TEntity entity, string propertyName)
	{
		ExecuteWithoutAutoDetectChanges(() => this.Entry(entity).Navigation(propertyName).IsLoaded = true);
	}

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
	public EntityEntry GetEntry(object entity, bool suppressDetectChanges = true)
	{
		return suppressDetectChanges
			? ExecuteWithoutAutoDetectChanges(() => this.Entry(entity))
			: this.Entry(entity);
	}

	/// <summary>
	/// Vrátí stav entity v DbContextu (resp. v jeho ChangeTrackeru).
	/// </summary>
	EntityState IDbContext.GetEntityState<TEntity>(TEntity entity)
	{
		return GetEntry(entity, suppressDetectChanges: true).State;
	}

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

}