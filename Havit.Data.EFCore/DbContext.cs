using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.Entity.Conventions;
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

		/// <inheritdoc />
		protected DbContext()
		{
		}

		/// <inheritdoc />
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
		///		<item>
		///			<term>LocalizationTableIndexConvention</term>
		///			<description><see cref="LocalizationTableIndexConvention"/></description>
		///		</item>
		/// </list>
		/// Žádné standardní konvence nejsou odebrány.
		/// </summary>
	    protected virtual void SetConventions(ConventionSet conventionSet)
	    {
			conventionSet.PropertyAddedConventions.Add(new DataTypeAttributeConvention());
			conventionSet.PropertyAddedConventions.Add(new SymbolPropertyIndexConvention());

			// TODO JK: Dopnit po zprovoznění implementace
			//conventionSet.PropertyAddedConventions.Add(new LocalizationTableIndexConvention());

			// TODO JK: Šlo by unikátním indexům přidat na začátek "U"?
	    }

	    /// <inheritdoc />
	    /// <remarks>
	    /// <seealso cref="DbContext.ExecuteWithDbUpdateExceptionHandling" />
	    /// </remarks>
	    public override int SaveChanges()
	    {
		    int result = ExecuteWithDbUpdateExceptionHandling(base.SaveChanges);			
		    AfterSaveChanges();
		    return result;
	    }

	    /// <inheritdoc />
	    /// <remarks>
	    /// <seealso cref="DbContext.ExecuteWithDbUpdateExceptionHandling" />
	    /// </remarks>
	    public override int SaveChanges(bool acceptAllChangesOnSuccess)
	    {
		    int result = ExecuteWithDbUpdateExceptionHandling(() => base.SaveChanges(acceptAllChangesOnSuccess));			
		    AfterSaveChanges();
		    return result;		    
	    }

	    /// <inheritdoc />
	    /// <remarks>
	    /// <seealso cref="DbContext.ExecuteWithDbUpdateExceptionHandling" />
	    /// </remarks>
	    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
	    {
		    int result = await ExecuteWithDbUpdateExceptionHandling<Task<int>>(() => base.SaveChangesAsync(cancellationToken));
		    AfterSaveChanges();
		    return result;
	    }

	    /// <inheritdoc />
	    /// <remarks>
	    /// <seealso cref="DbContext.ExecuteWithDbUpdateExceptionHandling" />
	    /// </remarks>
	    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
	    {
		    int result = await ExecuteWithDbUpdateExceptionHandling<Task<int>>(() => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
		    AfterSaveChanges();
		    return result;
	    }

		/// <summary>
		/// Zavolá výkonný kód (action). Pokud je věhem volání vyhozena výjimka <see cref="DbUpdateException"/>, přebalí ji do nové instance <see cref="DbUpdateException"/>, která obsahuje detailnější <see cref="DbUpdateException.Message"/>.
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
	    /// Zajišťuje volání registrovaných after <see cref="DbContext.SaveChanges"/> akcí (viz <see cref="DbContext.RegisterAfterSaveChangesAction"/>).
	    /// Spuštěno z metody SaveChanges po volání bázové <see cref="DbContext.SaveChanges"/>().
	    /// </summary>
	    protected internal virtual void AfterSaveChanges()
	    {
		    afterSaveChangesActions?.ForEach(item => item.Invoke());
		    afterSaveChangesActions = null;
	    }

	    /// <summary>
	    /// Registruje akci k jednorázovému provedení po <see cref="DbContext.SaveChanges"/>. Akce je provedena metodou <see cref="DbContext.AfterSaveChanges"/>.
	    /// Při opakovaném volání <see cref="DbContext.SaveChanges"/> není akce volána opakovaně.
	    /// </summary>
	    public void RegisterAfterSaveChangesAction(Action action)
	    {
		    if (afterSaveChangesActions == null)
		    {
			    afterSaveChangesActions = new List<Action>();
		    }
		    afterSaveChangesActions.Add(action);
	    }
    }
}
