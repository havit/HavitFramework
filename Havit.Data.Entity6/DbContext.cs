using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.Entity.Conventions;
using ForeignKeyIndexConvention = Havit.Data.Entity.Conventions.ForeignKeyIndexConvention;

namespace Havit.Data.Entity
{
	/// <summary>
	/// DbContext. Konfiguruje standardní DbContext (vypíná ProxyCreation a LazyLoading, odebírá a přidává některé konvence).
	/// </summary>
	public abstract class DbContext : System.Data.Entity.DbContext, IDbContext
	{
		/// <summary>
		/// Registr akcí k provedení po uložení změn.
		/// </summary>
		private List<Action> afterSaveChangesActions;

		/// <summary>
		/// Konstruktor. Použije "DefaultConnectionString" jako název používaného connection stringu.
		/// </summary>
		protected DbContext() : this("DefaultConnectionString")
		{

		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected DbContext(string nameOrConnectionString) : base(nameOrConnectionString)
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

			this.Set<object>(); // Pro podporu EntityFramework.MappingAPI - podpora pro Code First funguje až po prvním zavolání Set<T>().

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

			// Vlastnosti typu string nechceme nechat mít hodnoty null, abychom nemuseli rozlišovat při dotazování řešit či rozlišovat null a prázdný text.
			modelBuilder.Conventions.Add<RequiredStringPropertiesConvention>();

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
			int result = await ExecuteWithSaveChangesExceptionHandling<Task<int>>(base.SaveChangesAsync);
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
		}

		/// <summary>
		/// Registruje akci k provedení po save changes. Akce je provedena metodou AfterSaveChanges.
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
		/// Vrátí objekty v daném stavu.
		/// </summary>		
		public object[] GetObjectsInState(EntityState state)
		{
			return ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntries(state).Select(item => item.Entity).ToArray();
		}

		/// <summary>
		/// Vrací stav entity z change trackeru.
		/// </summary>
		public EntityState GetEntityState<TEntity>(TEntity entity)
			where TEntity : class
		{
			return Entry(entity).State;
		}

		/// <summary>
		/// Nastaví objekt do požadovaného stavu.
		/// </summary>
		public void SetEntityState<TEntity>(TEntity entity, EntityState entityState)
			where TEntity : class
		{
			this.Entry(entity).State = entityState;
		}

		public bool IsEntityReferenceLoaded<TEntity>(TEntity entity, string propertyName)
			where TEntity : class
		{
			return Entry(entity).Reference(propertyName).IsLoaded;
		}

		public bool IsEntityCollectionLoaded<TEntity>(TEntity entity, string propertyName)
			where TEntity : class
		{
			return Entry(entity).Collection(propertyName).IsLoaded;
		}

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
		Task IDbContext.SaveChangesAsync()
		{
			return this.SaveChangesAsync();
		}

	}
}
