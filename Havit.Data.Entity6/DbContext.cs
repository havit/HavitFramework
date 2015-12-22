using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
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
		/// Vrací stav entity z change trackeru.
		/// </summary>
		public EntityState GetEntityState(object entity)
		{
			return Entry(entity).State;
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
		async Task IDbContext.SaveChangesAsync()
		{
			await this.SaveChangesAsync();
		}

		/// <summary>
		/// Vrátí objekty v daném stavu.
		/// </summary>		
		public object[] GetObjectsInState(EntityState state)
		{
			return ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntries(state).Select(item => item.Entity).ToArray();
		}

		/// <summary>
		/// Nastaví objekt do požadovaného stavu.
		/// </summary>
		public void SetEntityState<TEntity>(TEntity entity, EntityState entityState)
			where TEntity : class
		{
			this.Entry(entity).State = entityState;
		}
	}
}
