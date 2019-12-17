using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Internal;
using Havit.Data.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.EntityFrameworkCore
{
	/// <inheritdoc cref="Microsoft.EntityFrameworkCore.DbContext" />
	public abstract class DbContext : Microsoft.EntityFrameworkCore.DbContext, IDbContext, IDbContextTransient
    {
	    /// <summary>
	    /// Registr akcí k provedení po uložení změn.
	    /// </summary>
	    private List<Action> afterSaveChangesActions;

		/// <summary>
		/// Konstruktor. Viz <see cref="Microsoft.EntityFrameworkCore.DbContext()"/>.
		/// </summary>
		protected DbContext()
		{
		}

		/// <summary>
		/// Konstruktor. Viz <see cref="Microsoft.EntityFrameworkCore.DbContext(DbContextOptions)"/>.
		/// </summary>
	    protected DbContext(DbContextOptions options) : base(options)
	    {
	    }

	    /// <inheritdoc />
	    /// <remarks>
	    /// Zajistí nastavení služeb convencí.
	    /// </remarks>
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
		protected virtual void RegisterDataSeedVersion(ModelBuilder modelBuilder)
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
		/// Zajišťuje volání konvencí.
		/// </summary>
		protected virtual void ModelCreatingCompleting(ModelBuilder modelBuilder)
		{
			var conventions = GetModelConventions().ToList();
			foreach (var convention in conventions)
			{
				convention.Apply(modelBuilder);
			}
		}

		/// <summary>
		/// Vrací konvence použité v modelu.
		/// Aktuálně vrací tyto konvence:
		/// <list type="bullet">
		///		<item>
		///			<term>DataTypeAttributeConvention</term>
		///			<description><see cref="DataTypeAttributeConvention"/></description>
		///		</item>
		/// </list>
		/// </summary>
		protected virtual IEnumerable<IModelConvention> GetModelConventions()
	    {			
			yield return new ManyToManyEntityKeyDiscoveryConvention();
			yield return new DataTypeAttributeConvention();
		    yield return new CascadeDeleteToRestrictConvention();
			yield return new CacheAttributeToAnnotationConvention();
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
	    /// Vrátí objekty v daném stavu.
	    /// </summary>
	    object[] IDbContext.GetObjectsInState(EntityState state, bool suppressDetectChanges)
	    {
		    return ((IDbContext)this).GetObjectsInStates(new EntityState[] { state }, suppressDetectChanges: suppressDetectChanges);
	    }

	    /// <summary>
	    /// Vrátí objekty v daných stavech.
	    /// </summary>
	    object[] IDbContext.GetObjectsInStates(EntityState[] states, bool suppressDetectChanges)
	    {
		    Func<object[]> getObjectInStatesFunc = () => this.ChangeTracker.Entries().Where(entry => states.Contains(entry.State)).Select(item => item.Entity).ToArray();
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
		    return new DbSetInternal<TEntity>(this);
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

}
