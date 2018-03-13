using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.EFCore;
using Havit.Data.EFCore.Metadata.Builders;
using Havit.Model.Localizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// TODO JK: Namespace vs. assembly name, aktuálně jsou v rozporu
// TODO JK: Doplnit NotNullAttribute na všechna volání, overrides, atp.

namespace Havit.EntityFrameworkCore
{
	public abstract class DbContext : Microsoft.EntityFrameworkCore.DbContext, IDbContext
    {
	    /// <summary>
	    /// Registr akcí k provedení po uložení změn.
	    /// </summary>
	    private List<Action> afterSaveChangesActions;

		protected DbContext()
		{
			Initialize();
		}

	    protected DbContext(DbContextOptions options) : base(options)
	    {
			Initialize();    
	    }

	    private void Initialize()
	    {
			// NOOP	    
	    }

	    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
	    {
		    base.OnModelCreating(modelBuilder);			
			
			CustomizeModelCreating(modelBuilder);
		    // TODO JK: System types (dataseed)
		    ApplyConventions(modelBuilder);

	    }

	    protected virtual void CustomizeModelCreating(ModelBuilder modelBuilder)
	    {		    
	    }

	    protected virtual void ApplyConventions(ModelBuilder modelBuilder)
	    {
			// TODO JK: Co s touhle strukturální hrůzou?

		    foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
		    {
			    foreach (IMutableProperty property in entityType.GetProperties())
			    {
				    if (property.PropertyInfo != null)
				    {
					    foreach (DataTypeAttribute dataTypeAttribute in property.PropertyInfo.GetCustomAttributes(typeof(DataTypeAttribute), false))
					    {
						    if (dataTypeAttribute.DataType == DataType.Date)
						    {
							    modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasColumnType("date");
						    }
						    else
						    {
							    throw new NotSupportedException($"DataType.{dataTypeAttribute.DataType} is not supported, the only supported value on DataTypeAttribute is DataType.Date.");
						    }
					    }
				    }
			    }
		    }

			// TODO JK: Podpora pro Suppress!!!
		    foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
		    {
			    foreach (IMutableProperty property in entityType.GetProperties())
			    {
				    if (property.Name == "Symbol")
				    {
						// pokud je Symbol nullable, je index automaticky filtrovaný
					    modelBuilder.Entity(entityType.ClrType).HasUniqueIndex(property.Name);
				    }
			    }
		    }

		    // TODO JK: Podpora pro Suppress!!!
		    foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
		    {
			    bool isLocalizationType = entityType.ClrType.GetInterfaces().Any(item => item.IsGenericType && (item.GetGenericTypeDefinition() == typeof(ILocalization<,>))); // zjistíme, zda entityTypeCSpace implementuje typeof(ILocalization<,>
			    if (isLocalizationType)
			    {
					IMutableNavigation parentProperty = entityType.FindNavigation("Parent");
					IMutableNavigation languageProperty = entityType.FindNavigation("Language");

					Console.WriteLine("Index here");
					Console.WriteLine(parentProperty == null);
					Console.WriteLine(languageProperty == null);
					// pokud máme k dispozici vlastnosti (sloupce) LanguageId a ParentId (teoreticky mohou být v předkovi nebo nemusí vůbec existovat, protože interface ILocalization<,> je nepředepisuje, apod.)
					if ((parentProperty != null) && (languageProperty != null))
					{
						modelBuilder.Entity(entityType.ClrType).HasUniqueIndex(
							parentProperty.ForeignKey.Properties.Select(property => property.Name)
							.Concat(languageProperty.ForeignKey.Properties.Select(property => property.Name)).ToArray());
					}
				}				    
		    }

			// TODO JK: Šlo by unikátním indexům přidat na začátek "U"?
	    }

	    /// <summary>
	    /// Saves all changes made in this context to the database.
		/// See Microsoft.EntityFrameworkCore.DbContext.SaveChanges method for next details.
	    /// </summary>
	    public override int SaveChanges()
	    {
		    int result = ExecuteWithSaveChangesExceptionHandling(base.SaveChanges);			
		    AfterSaveChanges();
		    return result;
	    }

		/// <summary>
		/// Saves all changes made in this context to the database.
		/// See Microsoft.EntityFrameworkCore.DbContext.SaveChanges method for next details.
		/// </summary>
	    public override int SaveChanges(bool acceptAllChangesOnSuccess)
	    {
		    int result = ExecuteWithSaveChangesExceptionHandling(() => base.SaveChanges(acceptAllChangesOnSuccess));			
		    AfterSaveChanges();
		    return result;		    
	    }

	    /// <summary>
	    /// Asynchronously saves all changes made in this context to the database.
		/// See Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync method for next details.
	    /// </summary>
	    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
	    {
		    int result = await ExecuteWithSaveChangesExceptionHandling<Task<int>>(() => base.SaveChangesAsync(cancellationToken));
		    AfterSaveChanges();
		    return result;
	    }

	    /// <summary>
	    /// Asynchronously saves all changes made in this context to the database.
	    /// See Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync method for next details.
	    /// </summary>
	    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
	    {
		    int result = await ExecuteWithSaveChangesExceptionHandling<Task<int>>(() => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
		    AfterSaveChanges();
		    return result;
	    }

	    protected virtual T ExecuteWithSaveChangesExceptionHandling<T>(Func<T> protectedAction)
	    {
		    try
		    {
			    return protectedAction.Invoke();
		    }
		    catch (DbUpdateException dbUpdateException)
		    {
				// DbUpdateException je ošlivě formátovaná, tak vytvoříme novou instanci
				// (tím neměníme typ vyhazované výjimky), nastavíme jí hezčí Message,
				// předáme Entries a pro všechny případe původní výjimku vložíme do inner exception.
				// K dispozici však nejsou dbUpdateException.Entries (konstruktor přijímá jiný typ, než který je publikován, tj. jsou třídou zpracovány a nebudeme je předělávat zpět, abychom je mohli hodit konstruktoru)
				throw new DbUpdateException(dbUpdateException.FormatErrorMessage(), dbUpdateException);
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
    }
}
