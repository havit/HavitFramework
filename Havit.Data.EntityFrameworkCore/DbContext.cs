using Havit.Data.EntityFrameworkCore.Internal;
using Havit.Data.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.EntityFrameworkCore;

/// <inheritdoc cref="Microsoft.EntityFrameworkCore.DbContext" />
public abstract class DbContext : Microsoft.EntityFrameworkCore.DbContext, IDbContext
{
	/// <summary>
	/// Registr akcí k provedení po uložení změn.
	/// </summary>
	private List<Action> afterSaveChangesActions;

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
	protected override sealed void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		RegisterDataSeedVersion(modelBuilder);

		CustomizeModelCreating(modelBuilder);

		ModelCreatingCompleting(modelBuilder);
	}

	/// <inheritdoc />
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		// Nebude fungovat v DbContext poolingu.
		// Pro DbContext pooling se přidávají tyto konvence v AddDbContext(Pool).
		optionsBuilder.UseFrameworkConventions();
		optionsBuilder.UseDbLockedMigrator();
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
	/// Při případném vyhození DbUpdateException dojde k jejímu přebalení s upřesněním Message.
	/// <seealso cref="DbContext.ExecuteWithDbUpdateExceptionHandling" />
	/// </summary>
	public override int SaveChanges(bool acceptAllChangesOnSuccess)
	{
		int result = ExecuteWithDbUpdateExceptionHandling(() => base.SaveChanges(acceptAllChangesOnSuccess));
		AfterSaveChanges();
		return result;
	}

	/// <summary>
	/// Uloží registrované změny. Viz <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(bool, System.Threading.CancellationToken)"/>.
	/// Při případném vyhození DbUpdateException dojde k jejímu přebalení s upřesněním Message.
	/// <seealso cref="DbContext.ExecuteWithDbUpdateExceptionHandling" />
	/// </summary>
	public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
	{
		int result = await ExecuteWithDbUpdateExceptionHandling<Task<int>>(() => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)).ConfigureAwait(false);
		AfterSaveChanges();
		return result;
	}

	/// <summary>
	/// Zavolá výkonný kód (action). Pokud je během volání vyhozena výjimka <see cref="DbUpdateException"/>, přebalí ji do nové instance <see cref="DbUpdateException"/>, která obsahuje detailnější Message.
	/// </summary>
	protected internal virtual T ExecuteWithDbUpdateExceptionHandling<T>(Func<T> action)
	{
		try
		{
			return action.Invoke();
		}
		catch (DbUpdateException dbUpdateException)
		{
			// DbUpdateException je ošlivě formátovaná, tak vytvoříme novou instanci
			// (tím neměníme typ vyhazované výjimky), nastavíme jí hezčí Message.
			// K dispozici však nejsou dbUpdateException.Entries (konstruktor přijímá jiný typ, než který je publikován, tj. jsou třídou zpracovány a nebudeme je předělávat zpět, abychom je mohli hodit konstruktoru)
			throw new DbUpdateException(dbUpdateException.FormatErrorMessage(), dbUpdateException);
		}
	}

	/// <summary>
	/// Zajišťuje volání registrovaných after save changes akcí.
	/// Spuštěno z metody SaveChanges po volání bázové SaveChanges(Async).
	/// </summary>
	protected internal virtual void AfterSaveChanges()
	{
		afterSaveChangesActions?.ForEach(item => item.Invoke());
		afterSaveChangesActions = null;
	}

	/// <summary>
	/// Registruje akci k jednorázovému provedení po save changes. Akce je provedena metodou DbContext.AfterSaveChanges.
	/// Při opakovaném volání DbContext.SaveChanges není akce volána opakovaně.
	/// </summary>
	public void RegisterAfterSaveChangesAction(Action action)
	{
		if (afterSaveChangesActions == null)
		{
			afterSaveChangesActions = new List<Action>();
		}
		afterSaveChangesActions.Add(action);
	}

	/// <summary>
	/// Provede akci s AutoDetectChangesEnabled nastaveným na false, přičemž je poté AutoDetectChangesEnabled nastaven na původní hodnotu.
	/// </summary>
#if BENCHMARKING
	internal TResult ExecuteWithoutAutoDetectChanges<TResult>(Func<TResult> action)
#else
	private TResult ExecuteWithoutAutoDetectChanges<TResult>(Func<TResult> action)
#endif
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
	/// Vrátí objekty v daných stavech.
	/// </summary>
	EntityEntry[] IDbContext.GetEntries(bool suppressDetectChanges)
	{
		EntityEntry[] getObjectInStatesFunc() => this.ChangeTracker.Entries().ToArray();

		return suppressDetectChanges
			? ExecuteWithoutAutoDetectChanges(getObjectInStatesFunc)
			: getObjectInStatesFunc();
	}

	/// <summary>
	/// Vrací true, pokud je EF považuje vlastnosti za načtenou.
	/// </summary>
	bool IDbContext.IsNavigationLoaded<TEntity>(TEntity entity, string propertyName)
	{
		return GetEntry(entity, suppressDetectChanged: true).Navigation(propertyName).IsLoaded;
	}

	void IDbContext.MarkNavigationAsLoaded<TEntity>(TEntity entity, string propertyName)
	{
		ExecuteWithoutAutoDetectChanges(() => this.Entry(entity).Navigation(propertyName).IsLoaded = true);
	}

	/// <summary>
	/// Vrací DbSet pro danou entitu.
	/// Pro snažší možnost mockování konzumentů DbSetu je vytvořena abstrakce do interface IDbSet&lt;TEntity&gt;.
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
	public EntityEntry GetEntry(object entity, bool suppressDetectChanged = true)
	{
		return suppressDetectChanged
			? ExecuteWithoutAutoDetectChanges(() => this.Entry(entity))
			: this.Entry(entity);
	}

	/// <summary>
	/// Vrátí stav entity v DbContextu (resp. v jeho ChangeTrackeru).
	/// </summary>
	EntityState IDbContext.GetEntityState<TEntity>(TEntity entity)
	{
		return GetEntry(entity, suppressDetectChanged: true).State;
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
	Task IDbContext.SaveChangesAsync(CancellationToken cancellationToken)
	{
		return SaveChangesAsync(cancellationToken);
	}
}