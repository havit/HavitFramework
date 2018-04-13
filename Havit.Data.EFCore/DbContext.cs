using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.Entity.Conventions;
using Havit.Data.Entity.Internal;
using Havit.Data.Entity.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

// TODO JK: Doplnit NotNullAttribute na všechna volání, overrides, atp.

namespace Havit.Data.Entity
{
	/// <inheritdoc cref="Microsoft.EntityFrameworkCore.DbContext" />
	public abstract class DbContext : Microsoft.EntityFrameworkCore.DbContext, IDbContext
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
	    protected override void OnModelCreating(ModelBuilder modelBuilder)
	    {			
		    base.OnModelCreating(modelBuilder);			
		    
		    // TODO JK: System types (dataseed)

		    ConventionSet conventionSet = modelBuilder.GetInfrastructure().GetConventionSet();
		    SetConventions(conventionSet);
	    }

		/// <summary>
		/// Zajistí nastavení služeb convencí.
		/// Aktuálně přidává tyto konvence:
		/// <list type="bullet">
		///		<item>
		///			<term>DataTypeAttributeConvention</term>
		///			<description><see cref="DataTypeAttributeConvention"/></description>
		///		</item>
		///		<item>
		///			<term>SymbolPropertyIndexConvention</term>
		///			<description><see cref="SymbolPropertyIndexConvention"/></description>
		///		</item>
		/// </list>
		/// Žádné standardní konvence nejsou odebrány.
		/// </summary>		
		//TODO JK:  Odkomentovat po dodělání
		//		<item>
		//			<term>LocalizationTableIndexConvention</term>
		//			<description><see cref="LocalizationTableIndexConvention"/></description>
		//		</item>		 
	    protected virtual void SetConventions(ConventionSet conventionSet)
	    {
			conventionSet.PropertyAddedConventions.Add(new DataTypeAttributeConvention());
			conventionSet.PropertyAddedConventions.Add(new SymbolPropertyIndexConvention());

			// TODO JK: Dopnit po zprovoznění implementace
			//conventionSet.PropertyAddedConventions.Add(new LocalizationTableIndexConvention());
			// TODO JK: Šlo by unikátním indexům přidat na začátek "U"?
	    }

	    /// <summary>
	    /// Uloží registrované změny. Viz <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()"/>.
	    /// Při případném vyhození DbUpdateException dojde k jejímu přebalení s upřesněním Message.
	    /// <seealso cref="DbContext.ExecuteWithDbUpdateExceptionHandling" />
	    /// </summary>
	    public override int SaveChanges()
	    {
		    int result = ExecuteWithDbUpdateExceptionHandling(base.SaveChanges);			
		    AfterSaveChanges();
		    return result;
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
	    /// Uloží registrované změny. Viz <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/>.
	    /// Při případném vyhození DbUpdateException dojde k jejímu přebalení s upřesněním Message.
	    /// <seealso cref="DbContext.ExecuteWithDbUpdateExceptionHandling" />
	    /// </summary>
	    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
	    {
		    int result = await ExecuteWithDbUpdateExceptionHandling<Task<int>>(() => base.SaveChangesAsync(cancellationToken));
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
		    int result = await ExecuteWithDbUpdateExceptionHandling<Task<int>>(() => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
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
	    public object[] GetObjectsInState(EntityState state, bool suppressDetectChanges)
	    {
		    return GetObjectsInStates(new EntityState[] { state }, suppressDetectChanges: suppressDetectChanges);
	    }

	    /// <summary>
	    /// Vrátí objekty v daných stavech.
	    /// </summary>
	    public object[] GetObjectsInStates(EntityState[] states, bool suppressDetectChanges)
	    {
		    Func<object[]> getObjectInStatesFunc = () => this.ChangeTracker.Entries().Where(entry => states.Contains(entry.State)).Select(item => item.Entity).ToArray();
		    return suppressDetectChanges
			    ? ExecuteWithoutAutoDetectChanges(getObjectInStatesFunc)
			    : getObjectInStatesFunc();
	    }

	    /// <summary>
	    /// Vrací true, pokud je EF považuje referenci za načtenou.
	    /// </summary>
	    public bool IsEntityReferenceLoaded<TEntity>(TEntity entity, string propertyName)
		    where TEntity : class
	    {
		    return ExecuteWithoutAutoDetectChanges(() => this.Entry(entity).Reference(propertyName).IsLoaded);
	    }

	    /// <summary>
	    /// Vrací true, pokud je EF považuje kolekci za načtenou.
	    /// </summary>
	    public bool IsEntityCollectionLoaded<TEntity>(TEntity entity, string propertyName)
		    where TEntity : class
	    {
		    return ExecuteWithoutAutoDetectChanges(() => this.Entry(entity).Collection(propertyName).IsLoaded);
	    }

	    //public void SetEntityCollectionLoaded<TEntity>(TEntity entity, string propertyName, bool isLoaded)
		   // where TEntity : class
	    //{
		   // ExecuteWithoutAutoDetectChanges(() => this.Entry(entity).Collection(propertyName).IsLoaded = isLoaded);
	    //}

	    /// <summary>
		/// Vrací DbSet pro danou entitu.
		/// Pro snažší možnost mockování konzumentů DbSetu je vytvořena abstrakce do interface IDbSet&lt;TEntity&gt;.
		/// </summary>
	    IDbSet<TEntity> IDbContext.Set<TEntity>()
	    {  
		    return new DbSetInternal<TEntity>(this);
	    }

		/// <summary>
		/// Vrátí stav entity v DbContextu (resp. v jeho ChangeTrackeru).
		/// </summary>
	    public EntityState GetEntityState<TEntity>(TEntity entity)
			where TEntity : class
	    {
		    return ExecuteWithoutAutoDetectChanges(() => Entry(entity).State);
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
