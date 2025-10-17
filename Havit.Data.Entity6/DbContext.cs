using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using Havit.Data.Entity.Conventions;
using Havit.Data.Entity.Model;
using ForeignKeyIndexConvention = Havit.Data.Entity.Conventions.ForeignKeyIndexConvention;

namespace Havit.Data.Entity;

/// <summary>
/// DbContext. Konfiguruje standardní DbContext (vypíná ProxyCreation a LazyLoading, odebírá a přidává některé konvence).
/// </summary>
public abstract class DbContext : System.Data.Entity.DbContext, IDbContext
{
	/// <summary>
	/// Singleton pro použití v konstruktoru používající DbContextDefaultDatabase.
	/// </summary>
	public static DbContextDefaultDatabase DefaultDatabase { get; } = new DbContextDefaultDatabase();

	/// <summary>
	/// Registr akcí k provedení po uložení změn.
	/// </summary>
	private List<Action> afterSaveChangesActions;

	/// <summary>
	/// Zpřístupňuje Configuration.AutoDetectChangesEnabled.
	/// </summary>
	public bool AutoDetectChangesEnabled
	{
		get { return Configuration.AutoDetectChangesEnabled; }
		set { Configuration.AutoDetectChangesEnabled = value; }
	}

	/// <summary>
	/// Konstruktor.
	/// Použije "name=DefaultConnectionString" jako 'nameOrConnectionString'.
	/// </summary>
	protected DbContext() : base("name=DefaultConnectionString")
	{
		Initialize();
	}

	/// <summary>
	/// Konstruktor. Použije se výchozí pojmenování databáze a výchozí datbázový server (voláním bezparametrického bázového konstruktoru).
	/// Určeno pro použití konstruktoru tam, kde je takový třeba:
	/// <code>
	/// public class MyDbContext : DbContext
	/// {
	///		public MyDbContext() : base(DefaultDatabase)
	/// 	{
	/// 		// NOOP
	/// 	}
	/// }
	/// </code>
	/// </summary>
#pragma warning disable S3253 // "base()" constructor calls should not be explicitly made // JK: Chci ho zde pro přehlednost!
	protected DbContext(DbContextDefaultDatabase _) : base()
#pragma warning restore S3253 // "base()" constructor calls should not be explicitly made
	{
		Initialize();
	}

	/// <summary>
	/// Constructs a new context instance using the given string as the name or connection
	///     string for the database to which a connection will be made. See the class remarks
	///     for how this is used to create a connection.
	/// </summary>
	/// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
	protected DbContext(string nameOrConnectionString) : base(nameOrConnectionString)
	{
		Initialize();
	}

	private void Initialize()
	{
		Configuration.LazyLoadingEnabled = false;
		Configuration.ProxyCreationEnabled = false;
	}

	/// <summary>
	/// Konfiguruje model - přidává a odebírá konvence.
	/// </summary>
	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		EntityTypeConfiguration<DataSeedVersion> dataSeedVersionEntity = modelBuilder.Entity<DataSeedVersion>();
		dataSeedVersionEntity.ToTable("__DataSeed");
		dataSeedVersionEntity.HasKey(item => item.ProfileName);
		dataSeedVersionEntity.Property(item => item.Version);

		// EF standardně pojmenovává tabulky v databázi v množném čísle (anglicky).
		// Chceme pojmenovat tabulky v jednotném čísle (a nemrvnit češtinu ala "Fakturas"),
		// proto odebereme konvenci zajišťující pojmenování v množném čísle.
		modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

		modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
		modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

		// EF má standardně dost divné chování při pojmenování cizích klíčů:
		// A) Pokud mám vlastnost 
		//      public Language Language { get; set; }
		//    pak se sloupec v databázi jmenuje Language_Id
		// B) Pokud mám vlastnosti
		//      public Language Langugage { get; set; }
		//      public int LanguageId { get; set; }
		//    pak se sloupec v databázi jmenuje LanguageId.
		// To nevyhovuje, protože nemáme svobodu doplnit vlastnost LanguageId bez změny datového schématu.
		// Proto přidáváme konvenci, která toto řeší, jak na vazbách 1:N, tak na vztahových tabulkách M:N.
		modelBuilder.Conventions.Add<ForeignKeyNamingConvention>();

		// EF standardně pojmenovává sloupec primárního klíče (obvykle Id) stejně.
		// Ze zvyku chceme mít pojmenovaný klíč jako "TabulkaId", proto přidáme konvenci, která toto řeší.
		modelBuilder.Conventions.Add<PrefixTableNameToPrimaryKeyNamingConvention>();

		// Podpora nastavení datového typu Date pomocí atributu.
		modelBuilder.Conventions.Add<DataTypePropertyAttributeConvention>();

		// Standardní convention pro cizí klíče nefunguje korektně, neumožňuje sloupec použít ve více indexech.
		// Použijeme proto vlastní konvenci, která toto řeší (a řeší i pojmenování indexu).
		modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.ForeignKeyIndexConvention>();
		modelBuilder.Conventions.Add<ForeignKeyIndexConvention>();

		// Pro sloupce pojmenované "Symbol" automaticky vytvoří unikátní index.
		modelBuilder.Conventions.Add<SymbolPropertyIndexConvention>();

		// Pro lokalizační tabulky vytvoří unikátní index na sloupcích ParentId a LanguageId.
		modelBuilder.Conventions.Add<LocalizationTableIndexConvention>();
	}

	/// <summary>
	/// Saves all changes made in this context to the underlying database.
	/// </summary>
	public override int SaveChanges()
	{
		int result = ExecuteWithSaveChangesExceptionHandling(base.SaveChanges);
		AfterSaveChanges();
		return result;
	}

	/// <summary>
	/// Asynchronously saves all changes made in this context to the underlying database.
	/// </summary>
	public override async Task<int> SaveChangesAsync()
	{
		int result = await ExecuteWithSaveChangesExceptionHandling<Task<int>>(base.SaveChangesAsync).ConfigureAwait(false);
		AfterSaveChanges();
		return result;
	}

	private T ExecuteWithSaveChangesExceptionHandling<T>(Func<T> protectedAction)
	{
		try
		{
			return protectedAction.Invoke();
		}
		catch (DbEntityValidationException dbEntityValidationException)
		{
			// DbEntityValidationException je ošlivě formátovaná, tak vytvoříme novou instanci
			// (tím neměníme typ vyhazované výjimky), nastavíme jí hezčí Message,
			// předáme EntityValidationErrors a pro všechny případe původní výjimku vložíme do inner exception.
			throw new DbEntityValidationException(dbEntityValidationException.FormatErrorMessage(), dbEntityValidationException.EntityValidationErrors, dbEntityValidationException);
		}
	}
	/// <summary>
	/// Spuštěno po save changes.
	/// Zajišťuje volání registrovaných after save changes akcí (viz RegisterAfterSaveChangesAction).
	/// </summary>
	protected internal virtual void AfterSaveChanges()
	{
		afterSaveChangesActions?.ForEach(item => item.Invoke());
		afterSaveChangesActions = null;
	}

	/// <summary>
	/// Registruje akci k provedení po save changes. Akce je provedena metodou AfterSaveChanges.
	/// Při opakovaném volání SaveChanges není akce volána opakovaně.
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
	/// Vrátí objekt pro přímý přístup k databázi.
	/// </summary>
	IDbContextDatabase IDbContext.Database
	{
		get
		{
			if (dbContextDatabase == null)
			{
				dbContextDatabase = new DbContextDatabase(this);
			}
			return dbContextDatabase;
		}
	}
	private IDbContextDatabase dbContextDatabase;

	/// <summary>
	/// Uloží evidované změny.
	/// </summary>
	void IDbContext.SaveChanges()
	{
		this.SaveChanges();
	}

	/// <summary>
	/// Uloží evidované změny.
	/// </summary>
	Task IDbContext.SaveChangesAsync(CancellationToken cancellationToken) // = default --> The default value specified for parameter 'cancellationToken' will have no effect because it applies to a member that is used in contexts that do not allow optional arguments...
	{
		return this.SaveChangesAsync(cancellationToken);
	}

	/// <summary>
	/// Vrátí objekty v daném stavu.
	/// </summary>		
	object[] IDbContext.GetObjectsInState(EntityState state)
	{
		return ExecuteWithoutAutoDetectChanges(() => ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager
			.GetObjectStateEntries(state) // vrací pole abstraktních ObjectStateEntry, které má dva potomky (bohužel jsou internal)
			.Where(item => item.Entity != null) // a protože je internal, odlišujeme EntityEntry odlišit od RelationshipEntry podle toho, zda má hodnotu ve vlastnosti Entity
			.Select(item => item.Entity).ToArray());
	}

	/// <summary>
	/// Vrací stav entity z change trackeru.
	/// </summary>
	EntityState IDbContext.GetEntityState<TEntity>(TEntity entity)
	{
		return ExecuteWithoutAutoDetectChanges(() => Entry(entity).State);
	}

	/// <summary>
	/// Nastaví objekt do požadovaného stavu.
	/// </summary>
	void IDbContext.SetEntityState<TEntity>(TEntity entity, EntityState entityState)
	{
		ExecuteWithoutAutoDetectChanges(() =>
		{
			this.Entry(entity).State = entityState;
			return (object)null;
		});
	}

	/// <summary>
	/// Vrací true, pokud je daná vlastnost na entitě načtena.
	/// </summary>
	bool IDbContext.IsEntityReferenceLoaded<TEntity>(TEntity entity, string propertyName)
	{
		return ExecuteWithoutAutoDetectChanges(() => Entry(entity).Reference(propertyName).IsLoaded);
	}

	/// <summary>
	/// Vrací true, pokud je daná vlastnost (kolekce) na entitě načtena.
	/// </summary>
	bool IDbContext.IsEntityCollectionLoaded<TEntity>(TEntity entity, string propertyName)
	{
		return ExecuteWithoutAutoDetectChanges(() => Entry(entity).Collection(propertyName).IsLoaded);
	}

	/// <summary>
	/// Nastaví informaci o tom, zda byla daná vlastnost dané entity načtena. Viz DbReferenceEntry.IsLoaded.
	/// </summary>
	void IDbContext.SetEntityReferenceLoaded<TEntity>(TEntity entity, string propertyName, bool loadedValue)
	{
		ExecuteWithoutAutoDetectChanges(() => Entry(entity).Reference(propertyName).IsLoaded = loadedValue);
	}

	/// <summary>
	/// Nastaví informaci o tom, zda byla daná vlastnost dané entity načtena. Viz DbCollectionEntry.IsLoaded.
	/// </summary>
	void IDbContext.SetEntityCollectionLoaded<TEntity>(TEntity entity, string propertyName, bool loadedValue)
	{
		ExecuteWithoutAutoDetectChanges(() => Entry(entity).Collection(propertyName).IsLoaded = loadedValue);
	}

	/// <summary>
	/// Volá DetectChanges na ChangeTrackeru.
	/// </summary>
	void IDbContext.DetectChanges()
	{
		this.ChangeTracker.DetectChanges();
	}

	/// <summary>
	/// Provede akci s AutoDetectChangesEnabled nastaveným na false, přičemž je poté AutoDetectChangesEnabled nastaven na původní hodnotu.
	/// </summary>
	public TResult ExecuteWithoutAutoDetectChanges<TResult>(Func<TResult> action)
	{
		if (AutoDetectChangesEnabled)
		{
			try
			{
				AutoDetectChangesEnabled = false;
				return action();
			}
			finally
			{
				AutoDetectChangesEnabled = true;
			}
		}
		else
		{
			return action();
		}
	}

	/// <summary>
	/// Provede akci s Configuration.UseDatabaseNullSemantics nastaveným na true, přičemž je poté Configuration.UseDatabaseNullSemantics nastaven na původní hodnotu.
	/// </summary>
	public TResult ExecuteWithDatabaseNullSemantics<TResult>(Func<TResult> action)
	{
		if (!Configuration.UseDatabaseNullSemantics)
		{
			try
			{
				Configuration.UseDatabaseNullSemantics = true;
				return action();
			}
			finally
			{
				Configuration.UseDatabaseNullSemantics = false;
			}
		}
		else
		{
			return action();
		}
	}

	/// <summary>
	/// Pouze pro existenci přetížení konstruktoru DbContextu, díky kterému se použije bezparametrický konstruktor předka.
	/// </summary>
	public sealed class DbContextDefaultDatabase
	{
		internal DbContextDefaultDatabase()
		{
		}
	}
}
