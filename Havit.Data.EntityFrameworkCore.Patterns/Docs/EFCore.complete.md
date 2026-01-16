# Obsah

* [Model](#model)
* [Entity](#entity)
* [Data Layer](#datalayer)
    * [DataSources](#datasources)
    * [Repositories](#repositories)
    * [DataEntries](#dataentries)
	* [LookupServices](#lookupservices)
	* [Lokalizace](#lokalizace)
	* [DataLoader](#dataloader)
	* [Cachování](#cachovani)
	* [Generovaná metadata](#generovana-metadata)
	* [SoftDeleteManager](#softdeletemanager)
	* [UnitOfWork](#unitofwork)
* [Seedování dat](#seedovani-dat)
* [Dependency Injection](#dependency-injection)

# Model

### Úvod
Konvence datového modelu a výchozí chování jsou velmi dobře popsány v oficiální dokumentaci EF Core, proto zde není smysluplné dokumentaci opakovat.
Viz https://docs.microsoft.com/en-us/ef/core/modeling/

Pojmenování tříd a modelu je v angličtině ev. v primárním jazyce projektu.

### Primární klíč

Používáme primární klíč typu int pojmenovaný Id.
Primátní klíč může být i jiného typu (celočíselný `SByte`, `Int16`, `Int64`, `Byte`, `UInt16`, `UInt32`, `UInt64`, dále `string` nebo `Guid`),
podpora těchto typů zatím není kompletní (chybí minimálně podpora [DataLoaderu](#DataLoader)).

```csharp
public int Id { get; set; }
```

Přítomnost a pojmenování primárního klíče je kontrolována unit testem.

```csharp
public int Id { get; set; }
```

### Délky stringů

U všech vlastností typu `string` je nutno uvést jejich maximální délku pomocí attributu `[MaxLength]`.
Pokud nemá být délka omezená, atributu nezadáváme hodnotu nebo použijeme hodnotu `Int32.MaxValue`.

Ze zadaných hodnot jsou [vygenerována metadata](#generovana-metadata), např. pro snadné omezení maximální délky textu v UI.

```csharp
[MaxLength(128)]
public string PasswordHash { get; set; }

[MaxLength(8)]
public string PasswordSalt { get; set; }

...

[MaxLength] // pro maximální možnou délku
public string Note { get; set; }
```

### Výchozí hodnoty vlastností

Výchozí hodnoty vlastností definujeme přímo v kódu:

```csharp
public bool IsActive { get; set; } = true;
```

### Reference / cizí klíče

Není-li jiná potřeba, definujeme v páru cizí klíč (vlastnost typu `int` nesoucí hodnotu cizího klíče) a navigation property (obvykle reference na cílový objekt).
Pro pojmenování konvenci `EntityId` a `Entity`.

Důvodem jsou možnosti pro dotazování či možnosti podpory seedování dat.

```csharp
public Pohlavi Parent { get; set; }
public int ParentId { get; set; }

public Language Language { get; set; }
public int LanguageId { get; set; }
```

Unit test kontroluje, že jsou vlastnosti v páru, tedy že každá navigation property má i foreign key property.
Dále kontroluje pojmenování vlastností končících na `Id` a nikoliv `ID`.

### Kolekce One-To-Many (1:N)

- Obvykle používáme `List<T>`, ale stristriktně předepsáno to není.
- Kolekce mají smysl např. pro:
  - aggregate root (`Order` + `OrderLines`)
  - lokalizace (`Country` + `CountryLocalizations`)
  - členství uživatelů v rolích (`User` + `Memberships` + `Role`)
- Kolekce zásadně **nepoužíváme** tam, kde jsou v kolekcích velké objemy příznakem smazaných dat. Důvodem je nemožnost rozumně načíst jen nesmazané záznamy.
- Kolekce definujeme jako **readonly** a inicializujeme je v pomocí auto-property initializeru (nebo v konstruktoru).

```csharp
public List<CountryLocalization> Localizations { get; } = new List<CountryLocalization>();
```

### Kolekce Many-To-Many (M:N)

> ⚠️ Entity Framework Core 5.x přináší podporu pro vazby typu M:N (viz dokumentace), avšak HFW pro práci s kolekcemi nemá podporu.

Vazby M:N doporučujeme **dekomponovat na dvě vazby 1:N** (postup známý z EF Core 2.x a 3.x).
Ve výchozím chování EF Core je třeba této entitě nakonfigurovat složený primární klíč (pomocí data anotations nelze definovat složený primární klíč), nám se klíč nastaví sám (pokud není ručně nastaven) konvencí. Pokud je to třeba, nastavíme pouze název databázové tabulky, do které je entita mapována.

#### Příklad
Pokud má mít `User` kolekci `Roles`, musíme zavést entity `Membership` se dvěma vlastnostmi. `User` pak bude mít kolekci nikoliv rolí, ale těchto `Membership`ů.

```csharp
public class User
{
    public int Id { get; set; }

    public List<Membership> Roles { get; } = new List<Membership>();

    ...
}
public class Role
{
    public int Id { get; set; }
    ...
}
public class Membership
{
    public User User { get; set; }
    public int UserId { get; set; }

    public Role Role { get; set; }
    public int RoleId { get; set; }
}
```

### Kolekce s filtrováním smazaných záznamů

Viz [Kolekce s filtrováním smazaných záznamů](#kolekce-s-filtrovánim-smazanych-zaznamu).

### Mazání příznakem (Soft Delete)

[Podpora mazání příznakem](#softdeletemanager) je na objektech, které obsahují vlastnost `Deleted` typu `Nullable<DateTime>`. Podpora není implementovatelná na dočítání kolekcí modelových objektů, tj. **při načítání kolekcí objektů jsou načítány i smazané objekty**.

```csharp
public DateTime? Deleted { get; set; }
```

### Lokalizace

V aplikaci je třeba definovat:

- Třídu `Language` implementující `Havit.Model.Localizations.ILanguage`
- interface `ILocalized<TLocalizationEntity>` dědící z `Havit.Model.Localizations.ILocalized<TLocalizationEntity, Language>` pro označení tříd, které jsou lokalizovány
- interface `ILocalization<TLocalizedEntity>` dědící z `Havit.Model.Localizations.ILocalization<TLocalizedEntity, Language>` pro označení tříd lokalizujících základní třídu (předchozí bod)

Datové třídy pak definujeme s těmito interfaces.

```csharp
public class Country : ILocalized<CountryLocalization>
{
	public int Id { get; set; }

	...

	public List<CountryLocalization> Localizations { get; } = new List<CountryLocalization>();
}

public class CountryLocalization : ILocalization<Country>
{
	public int Id { get; set; }

	public Country Parent { get; set; }
	public int ParentId { get; set; }

	public Language Language { get; set; }
	public int LanguageId { get; set; }

	[MaxLength]
	public string Name { get; set; }
}
```

### Entries / systémové záznamy (EnumClass)
Pokud má třída sloužit jako systémový číselník se známými hodnotami, použijeme vnořený veřejný enum `Entry` s hodnotami.
Pokud mají mít záznamy v databázi stejné Id, což je obvyklé, je třeba uvést položkám hodnotu.

Na základě tohoto enumu pak generátor zakládá DataEntries.


#### Příklad

```csharp
public class Role
{
    ...

    public enum Entry
    {
        Administrator = -1,
        CustomerAdministrator = -2,
        BookReader = -3,
        PublisherAdministrator = -4
    }
}
```

## Kolekce s filtrováním smazaných záznamů

Bohužel není možné **načíst** jen nesmazané záznamy. Můžeme však načíst do paměti všechny záznamy a používat jen ty nesmazané,
například vytvořením dvou kolekcí - persistentní (se všemi objekty) a nepersistentní (počítaná, filtruje jen nesmazané záznamy s persistentní kolekce.

V následujících ukázkách budeme pracovat s třídou `Child`, kterou lze příznakem označit za smazanou a s třídou `Master` mající kolekci `Children` objektů `Child`.

Pro implementaci potřebujeme zajistit:

* Použití kolekcí v modelu
* Mapování vlastností (kolekcí) v EF
* Kolekce filtrující smazané záznamy

**Filtrované kolekce není možné používat v queries (Where, OrderBy) ani v Include. V DataLoaderu je možné filtrované kolekce použít pro načtení záznamů.**

### Použití kolekcí v modelu

```csharp
public class Child
{
	public int Id { get; set; }
	public int MasterId { get; set; }
	public Master Master { get; set; }
	public DateTime? Deleted { get; set; }
}
 
public class Master
{
	public int Id { get; set; }

	public ICollection<Child> Children { get; } // nepersistentní
	public IList<Child> ChildrenIncludingDeleted { get; } = new List<Child>(); // persistentní

	public Master()
	{
		// kolekce children je počítanou kolekcí
		Children = new FilteringCollection<Child>(ChildrenIncludingDeleted, child => child.Deleted == null);
	}
}
```

### Mapování vlastností (kolekcí) v EF

```csharp
public class MasterConfiguration : IEntityTypeConfiguration<Master>
{
    public void Configure(EntityTypeBuilder<Master> builder)
    {
        builder.Ignore(c => c.Children);
        builder.HasMany(c => c.ChildrenIncludingDeleted);
    }
}
```

### Kolekce filtrující smazané záznamy

Viz `Havit.Model.Collections.Generic.FilteringCollection<T>` - [zdrojáky](https://havit.visualstudio.com/DEV/_git/002.HFW-HavitFramework?path=%2FHavit.Model%2FCollections%2FGeneric%2FFilteringCollection.cs&version=GBmaster). Kolekce je v nuget balíčku `Havit.Model`.

```csharp
public class FilteringCollection<T> : ICollection<T>
{
	private readonly ICollection<T> source;
	private readonly Func<T, bool> filter;

	public FilteringCollection(ICollection<T> source, Func<T, bool> filter)
	{
		this.source = source;
		this.filter = filter;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return source.Where(filter).GetEnumerator();
	}

    ...
}
```


# Entity

Definuje datový kontext, jeho vlastnosti a migrace.

## DbContext

Nevyžadujeme vytvářet vlastnosti typu `DbSet` pro každou evidovanou entitu.

### Obvyklá (a doporučená) struktura třídy DbContext

`ProjectNameDbContext` je připraven v `NewProjectTemplate`, nicméně kdyby bylo potřeba ručně:

Důležité je dědit z `Havit.Data.EntityFrameworkCore.DbContext`.

Obvykle se používají dva konstruktory:

* Konstruktor přijímající `DbContextOptions` pro běžné použití (produkční běh aplikace).
* Bezparametrický kontruktor pro použití v unit testech, proto je `internal`.

```csharp
public class NewProjectTemplateDbContext : Havit.Data.EntityFrameworkCore.DbContext
{
	internal NewProjectTemplateDbContext()
	{
		// NOOP
	}
	
	public NewProjectTemplateDbContext(DbContextOptions options) : base(options)
	{
		// NOOP
	}

	protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);

		modelBuilder.RegisterModelFromAssembly(typeof(Havit.NewProjectTemplate.Model.Localizations.Language).Assembly);
		modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
	}
}
```

### ConnectionString

Není žádné výchozí nastavení, jaký connection string bude použit. Vše je řešeno až při použití DbContextu, např. v konfiguraci AddDbContext(...), viz příklad v [Dependency Injection](#dependency-injection).
Není doporučeno použít OnConfiguring, neboť brání použití DbContext poolingu.

### Exception handling metod Save\[Async\]

V případě selhání uložení objektů je vyhozena výjimka `DbUpdateException`, ta je však "ošklivě formátovaná" a vyžaduje dohledávání, co se vlastně stalo v InnerException.  
Proto v případě výskytu `DbUpdateException` tuto zachytáváme a vyhazujeme novou instanci `DbUpdateException` s trochu lépe formátovanou zprávou (`Message`). Původní výjimku `DbUpdateException` použijeme jako `InnerException` námi vyhozené výjimky.

### DesignTimeDbContextFactory

Viz dokumentace [Design-time DbContext Creation](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation).

Využívá jej tooling migrací a code generátor. Pro účely toolingu migrací musí db context používat SqlServer (nebo jinou relační databázi, nelze použít in-memory provider).

## Registrace modelu a konfigurací

Abychom nemuseli registrovat entity ručně, je k dispozici extension metoda `RegisterModelFromAssembly`.
Zaregistruje všechny třídy z dané assembly, které nemají žádný z atributů: `[NotMapped]`, `[ComplexType]`, `[Owned]`.

Pro registraci konfigurací je k dispozici extension metoda `ApplyConfigurationsFromAssembly`.

## Conventions

### Výchozí konvence

- `ManyToManyEntityKeyDiscoveryConvention`  
  Konvence nastaví tabulkám, které reprezentují vazbu Many-To-Many složený primární klíč, pokud jej nemají nastaven.
  Index primárního klíče má sloupce v pořadí, v jakém byly definovány v kódu.
- `DataTypeAttributeConvention`
  Pokud je vlastnost třídy modelu označena atributem `[DataType]` s hodnotou `DataType.Date` pak se použije v databázi datový typ `Date`.
- `CascadeDeleteToRestictConvention`
  Všem cizím klíčům s nastaví DeleteBehavior na Restrict, čímž zamezí kaskádnímu delete.
- `CacheAttributeToAnnotationConvention`
  Hodnoty zadané v atributu `[Cache]` předá do anotations.

### Volitelné konvence

- `StringPropertiesDefaultValueConvention`
  Pro všechny stringové vlastnosti, pokud nemají výchozí hodnotu, se použije výchozí hodnota `String.Empty`.
  Nastavuje vlastnosti výchozí hodnotu a `ValueGenerated` na `Never`.

### Selektivní potlačení konvence

Pokud některá naše konvence nevyhovuje na určitém místě, lze ji potlačit. Dříve (EF Core 2.x) bylo možné je potlačit v konfiguraci entity, to již (EF Core 3.x) možné není, neboť v té době jsou již konvence aplikovány. Jediná šance je znečistit model informací, že se konvence nemá aplikovat.

Potlačení konvence lze vyjádřit umístěním `[SuppressConvention]` s uvedením konvence, kterou potlačujeme.
Identifikátory konvencí jsou ve třídě `ConventionIdentifiers`.
Atribut i třída s identifikátory jsou v nuget balíčku `Havit.Data.EntityFrameworkCore.Abstractions`.

Potlačit lze tyto konvence:

- `StringPropertiesDefaultValueConvention` (na modelové třídě pro všechny vlastnosti třídy, nebo jen na vlastnosti)
- `ManyToManyEntityKeyDiscoveryConvention` (na modelové třídě)

```csharp
[SuppressConvention(ConventionIdentifiers.ManyToManyEntityKeyDiscoveryConvention)]
public class SomeClass
{
...
}

public class OtherClass
{
...
	[SuppressConvention(ConventionIdentifiers.StringPropertiesDefaultValueConvention)]
	public string SomeString { get; set; }
}
```

## Konfigurace

Viz dobře napsaná [dokumentace EF Core](https://docs.microsoft.com/en-us/ef/core/modeling/).

### Vztah M:N

Entity Framework Core 5.x přináší podporu pro vazby typu M:N (viz [dokumentace](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many)), avšak HFW pro práci s kolekcemi nemá plnou podporu. Kolekce typu M:N je možné omezeně použít, nebude je umět dočíst `DbDataLoader` a nemají (a nejspíš mít později ani nebudou) řešenou podporu v entity validátorech a before commit processorech.

Příklad řešení v modelu a konfigurace je uveden v sekci [Model](#model).

## Migrations

Viz dokumentace [EF Core Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/).

### Spouštění Migrations

Spuštění migrations a seedů (viz další kapitoly) provádíme typicky při spuštění aplikace.
Samotné spuštění při startu aplikace zajišťuje hosted service `MigrationHostedService`, která migrace a seedy spustí prostřednictvím `MigrationService.UpgradeDatabaseSchemaAndDataAsync`.

# DataLayer

## Generátor kódu

Implementace tříd popsaných v následujících kapitolách je automaticky generována s umožněním vlastního rozšíření vygenerovaného kódu.

Kód je generován pomocí [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) `Havit.Data.EntityFrameworkCore.CodeGenerator.Tool` (musí být nainstalován), jenž spouští code generátor z NuGet balíčku `Havit.Data.EntityFrameworkCore.CodeGenerator`, který je zamýšlen pro použití v projektu `Entity`.

### Aktualizace dotnet toolu Havit.Data.EntityFrameworkCore.CodeGenerator.Tool

Aktualizace `Havit.Data.EntityFrameworkCore.CodeGenerator.Tool` se očekávají příležitostně, např. když se změní podporovaná verze `.NET`, atp. Změny (aktualizace) `Havit.Data.EntityFrameworkCore.CodeGenerator` jsou podle změn, které potřebujeme udělat do code generátoru.

> ℹ️ Tím, že `Havit.Data.EntityFrameworkCore.CodeGenerator.Tool` není "běžným nuget" balíčkem v projektu, ale jde o [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools), nezobrazují se jeho aktualizace v Package Manageru.
> Pro aktualizaci je třeba z příkazové řádky spustit (aktuální složka ve složce se solution):
>
> ```
> dotnet tool update Havit.Data.EntityFrameworkCore.CodeGenerator.Tool
> ```

### Spuštění generátoru

Generátor lze spustit powershell skriptem `Run-CodeGenerator.ps1` v rootu projektu `DataLayer`.

Pro spuštění přímo z Visual Studia si musíme otevřít jakoukoliv konzoli (Terminal, Developer Powershell, Nuget Package Console), přepnout se do složky `DataLayer` a spustit.

Běh generátoru je relativně rychlý, generátor je obvykle hotov během pár sekund.

### Princip generátoru (aneb kde bere generátor data)

Generátor získává data z modelu `DbContextu` ([DbContext.Model](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.model?view=efcore-2.1)). `DbContext` se hledá v assembly projektu `Entity`, získává se přes [DbContextActivator](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.design.dbcontextactivator?view=efcore-2.1), čímž získáme instanci přes [IDesignTimeDbContextFactory](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.design.idesigntimedbcontextfactory-1?view=efcore-2.1), pokud existuje.

Assembly pro `Entity` se hledá ve složce `Entity/bin` (a všech podsložkách), bere se poslední v čase, tj. nejaktuálnější. Tím řešíme případnou existenci více verzí assembly v případě existence lokálního buildu v `Debug` i v `Release` konfiguraci.

### Konfigurace generátoru kódu

V rootu projektu s generátorem kódu nebo ve složce se solution je možné mít soubor `efcore.codegenerator.json` s nastavením:

```json
{
	"ModelProjectPath": "...",
	"MetadataProjectPath": "...",
	"DataLayerProjectPath" : "...",
	"MetadataNamespace": "..."
}
```

* `ModelProjectPath` - název složky (musí obsahovat \*.csproj) nebo cesta k csproj, kam budou generována metadata z modelu, cesta je relativní vůči složce se solution. Výchozí hodnotou je Model\\Model.csproj. 
* `MetadataProjectPath` - název složky (musí obsahovat \*.csproj) nebo cesta k csproj, kam budou generována metadata z modelu, cesta je relativní vůči složce se solution. Výchozí hodnotou je Model\\Model.csproj (by default stejné ako `ModelProjectPath`).
* `DataLayerProjectPath` - název složky (musí obsahovat \*.csproj) nebo cesta k csproj, kam bude generován DataLayer kód, cesta je relativní vůči složce se solution. Výchozí hodnotou je DataLayer\\DataLayer.csproj.
* `MetadataNamespace`  namespace, do kterého se metadata generují, je možno použít strukturovaný namespace, např. `My.Customized.Metadata` (na disku budou metadata vygenerována do složky \_generated\\My\\Customized\\Metadata).
* `SuppressRemovingRelicRepositories` - umožní potlačit mazání repositories, které již nejsou v modelu (např. byly odstraněny). Výchozí hodnota je false (mazání probíhá).

### Co generuje

Viz dále uvedené:

* DbDataSource, FakeDataSource
* DbRepository
* DataEntries
* Metadata
* DataLayerServiceExtensions
    
### Generované soubory

V `DataLayer`u jsou generovány soubory:

* _generated\\DataEntries\\*Namespace*\\I*Entity*Entries.cs
* _generated\\DataEntries\\*Namespace*\\*Entity*Entries.cs
* _generated\\DataSources\\*Namespace*\\I*Entity*DataSource.cs
* _generated\\DataSources\\*Namespace*\\*EntityDb*DataSource.cs
* _generated\\DataSources\\*Namespace*\\Fakes\\Fake*Entity*DataSource.cs
* _generated\\Repositories\\*Namespace*\\I*Entity*Repository.cs
* _generated\\Repositories\\*Namespace*\\*Entity*DbRepository.cs
* _generated\\Repositories\\*Namespace*\\*Entity*DbRepositoryBase.cs
* _generated\\Repositories\\*Namespace*\\*Entity*DbRepositoryQueryProvider.cs
* _generated\\DataLayerServiceExtensions.cs

a dále jsou jednorázově vytvořeny soubory (tj. při opakovaném spuštění generátoru se nepřepisují, neaktualizují):

* Repositories\\*Namespace*\\I*Entity*Repository.cs
* Repositories\\*Namespace*\\*Entity*DbRepository.cs
    

V `Model`u (resp. dle nastavení `MetadataProjectPath`) jsou generovány soubory:
* _generated\\Metadata\\*Namespace*\\*Entity*Metadata.cs
    

### Poznámka ke složce `_generated`

V každém projektu (`Model`, `Entity`) je jen jedna (rootová) složka `_generated`. To umožňuje přehledné zobrazení pending changes (všechno generované lze snadno sbalit a přeskočit).
Soubory pro neexistující entity (odstraněné z modelu) se mažou. Mažou se i repositories, pro které neexistují entity (lze vypnout nastavením `SuppressRemovingRelicRepositories`).

### Poznámka k entitám pro vztah M:N

Pro entity reprezentující vztah M:N (entity mající jen složený primární klíč ze dvou sloupců a nic víc) se žádný kód negeneruje.
## DataSources

Zprostředkovává přístup k datům jako `IQueryable`. Umožňuje snadné podstrčení dat v testech.

### I*Entity*DataSource, IDataSource<*Entity*>

Poskytuje dvě vlastnosti: `Data` a `DataIncludingDeleted`. Pokud obsahuje třída příznak smazání (soft delete), pak vlastnost `Data` automaticky odfiltruje přínakem smazané záznamy.

Pro každou entitu vzniká jeden interface pojmenovaný `IEntityDataSource` (např. `ILanguageDataSource`).

### *Entity*DbDataSource

* Generované třídy implementující `IEntityDataSource`.
* Pro každou entitu vzniká jedna třída, třídy jsou pojmenované `EntityDbDataSource` (např. `LanguageDbDataSource`).
* K dotazům automaticky přidává [query tag](https://learn.microsoft.com/en-us/ef/core/querying/tags) `IEntityDataSource.Data[IncludingDeleted]` a je možno (doporučeno) při každém použití `DataSource` doplnit vlastní [query tag](https://learn.microsoft.com/en-us/ef/core/querying/tags).

Data jsou získávána z databáze (resp. z `IDbContextu` a jeho `DbSet`u).

### Fake*Entity*DataSource

* Jedná se rovněž o generované třídy implementující `IEntityDataSource` (rovněž je pro každou entity jedna třída `FakeEntityDataSource`, např. `FakeLaguageDataSource`), avšak nejsou napojeny na databázi.
* Třídy jsou dekorovány atributem `[Fake]` a jsou vnořeny do namespace `Fakes`.
* Data jsou čerpána z kolekce předané v konstruktoru. Určeno pro podstrčení dat v unit testech tam, kde je použita závislost `IEntityDataSource` (ev. službám ve frameworku se závislostí `IDataSource<Entity>`).
* Implementace využívá [MockQueryable.EntityFrameworkCore](https://www.nuget.org/packages/MockQueryable.EntityFrameworkCore), čímž zajistíme fungování i asynchronních operací (což nad prostým `IQueryable<Entity>` nefunguje).

#### Příklad použití Fake*Entity*DataSource v unit testu

```csharp
// Arrange

// připravíme data source obsahující zadané záznamy
FakeUserDataSource fakeUserDataSource = new FakeUserDataSource(
	new User { Id = 1, Username = "...", ... },
	new User { Id = 2, Username = "...", ... },
	new User { Id = 3, Username = "...", ... });

// použijeme data source jako závislost v testované třídě
ITestedService service = new TestedService(..., fakeUserDataSource, ...);

// Act
...
```
## Repositories

Repositories jsou třídy s jednoduchými a opakovaně použitelnými metodami pro přístup k datům.

Repositories (navzdory 95% implementací nalezitelných na internetu) neobsahují metody pro CRUD operace.

### I*Entity*Repository, IRepository<*Entity*>

Poskytuje metody:
* `GetObject[Async]`
* `GetObjects[Async]`
* `GetAll[Async]`

Pro každou entitu vzniká jeden interface pojmenovaný `IEntityRepository` (např. `ILanguageRepository`), který implementuje `IRepository<Entity>`.

### *Entity*DbRepository

Generované třídy implementují `IEntityRepository`.

Poskytuje veřejné metody (implementace `IRepository<Entity>`)

* `GetAll[Async]` - Vrací příznakem nesmazané záznamy, pokud je metoda nad jednou instancí volána opakovaně, nedochází k opakovaným dotazům do databáze.
* `GetObject[Async]` - Vrací objekt dle Id, pokud neexistuje záznam s takovým Id, je vyhozena výjimka.
* `GetObjects[Async]` - Vrací objekty dle kolekce Id, pokud neexistuje záznam pro alespoň jedno Id, je vyhozena výjimka. Při opakovaném volání metody jsou objekt vrácen z identity mapy (I)DbContextu.

a protected vlastnosti

* `Data` a `DataIncludingDeleted` - viz [Data Sources](http://havit-wiki.atlassian.net/#datasources "http://havit-wiki.atlassian.net#datasources"), implementačně používají hodnoty ze závislosti `IDataSource<TEntity>`, čímž je lze snadno napsat test s mockem dat pro tyto vlatnosti.

### Implementační instrukce

Není zvykem, aby se repository navzájem používaly jako závislosti v implementacích, protože by to mohlo vést až k nepřehlednému a neřešitelnému zauzlování repositories navzájem.

Pokud potřebuje jedna repository to samé, co jiná, což je samo o sobě nezvyklé, je doporučeno extrahovat kód do samostatné služby, např. jako Query.

### Načítání závislých objektů

Pokud chceme načíst referované objekty či kolekce, disponuje EF [třemi možnostmi načtení referovaných objektů](https://docs.microsoft.com/en-us/ef/core/querying/related-data "https://docs.microsoft.com/en-us/ef/core/querying/related-data"). My máme navíc implementovaný [DataLoader](#dataloader)

Repository disponuje možnostmi načíst závislé objekty.

#### GetLoadReferences

Metoda je určena k override a definuje, jaké závislosti mají být s objektem načteny. Syntaxe viz [DataLoader](#dataloader).

Příklad:

```csharp
protected  override  IEnumerable<Expression<Func<EmailTemplate,  object>>>  GetLoadReferences()
{
	yield  return x => x.Localizations;
}
```

Návratového typu `IEnumerable<Expression<Func<Entity*, object>>>` se není třeba bát 🙂):

* `Func<Entity, object>` říká, že použijeme lambda výraz, kterým určíme z *Entity*, nějakou vlastnost vracející cokoliv
* `Expression` rozšiřuje `Func` o to, že se lambda výraz přeloží jako [expression tree](https://msdn.microsoft.com/en-us/library/mt654263.aspx "https://msdn.microsoft.com/en-us/library/mt654263.aspx")
* `IEnumerable` říká, že můžeme vrátit více takových výrazů.
* Viz ukázka, je to jednoduché.
* Aktuálně není možné touto metodou zajistit načtení objektů z kolekce (tedy `x => x.PropertyA.PropertyB` lze použít jen tehdy, pokud `PropertyA` není kolekcí objektů).
	* použijte override `LoadReferences` + `LoadReferencesAsync`

#### LoadReferences[Async]

* Načte závislosti definované v `GetLoadReferences`.
* Automaticky použito v metodách `GetAll`, `GetObject[Async]` a `GetObjects[Async]`.
* Pokud repository obsahuje vlastní metody vracející entity, je potřeba před navrácením dat provést dočtení závislostí touto metodou!
* Načítání závislostí je provedeno pomocí [DataLoaderu](#dataloader), nikoliv pomocí `Include` (byť by to mohlo být někdy výhodnější). Možno overridovat (rozšířit) o další dočítání věcí, co nejsou přímo podporované skrze `GetLoadReferences` (např. prvky kolekcí).
* Používejte s rozvahou, každé navrácení objektu z repository s dočtením závislostí může znamenat další dotazy do databáze, zvětšení množství trackovaných entit i kdyby následně tyto závislosti nebyly použity, atp.

Příklad:

```csharp
public EmailTemplate GetByXy(string xy) // vymyšleno pro ukázku
{
	EmailTemplate template = Data.FirstOrDefault(item => item.XY == xy);
	LoadReferences(template);
	return template;
}
```
## DataEntries

DataEntries zpřístupňují systémové záznamy v databázi dle `Entries` v modelu.

#### Příklad vygenerovaného interface pro DataEntries
(viz Entries v Modelu)

```csharp
public interface IRoleEntries : IDataEntries
{
	Role Administrator { get; }			
	Role BookReader { get; }			
	Role CustomerAdministrator { get; }			
	Role PublisherAdministrator { get; }			
}
```

Implementace vyzvedává objekty z příslušné repository (`IRepository<Entity>`) pomocí metody `GetObject`.
V aplikaci to tak může být jedno z mála (jediné) synchronních načítání dat z databáze.

#### Příklady použití

```csharp
IRoleEntries entries;
...
// máme strong-type k dispozici objekt, který reprezentuje konkrétní záznam v databázi
bool userIsAdmin = userRoles.Contains(entries.Administrator);
```

```csharp
INastaveniEntries nastaveni;
...
// máme strong-type k dispozici objekt, který reprezentuje nastavení aplikace
string url = nastaveni.Current.ApplicationUrl
```

### Párování záznamů v databázi
* Pokud primární klíč cílové tabulky není autoincrement, páruje se `Id` záznamu s hodnotou enumu (`Role.Id == (int)Role.Entry.Administrator`).
* Pokud je primární klíč cílové tabulky autoincrement, páruje se pomocí stringového sloupce `Symbol`, který je v takovém případě povinný (`Role.Symbol == Role.Entry.Administrator.ToString()`). Párování (`Id`, `Symbol`) se pro každou tabulku načítá jen jednou a drží se v paměti.

## LookupServices

Jde o třídy, které mají zajistit možnost **rychlého vyhledávání entity podle definovaného klíče**. Na rozdíl od ostatních ([Repository](#repositories), [DataSources](#datasources)) nejsou generované - píšeme je ručně, pro jejich napsání je však připravena silná podpora.

Třída je určena k použití u neměnných či občasně měněných entit a u entit které se mění hromadně (naráz). Není garantována stoprocentní spolehlivost u entit, které se mění často (myšleno zejména paralelně) v různých transakcích - invalidace a aktualizace může proběhnout v jiném pořadí, než v jakém doběhly commity.

Rovněž z principu “out-of-the-box” nefunguje korektně invalidace při použití více instancí aplikace k aktualizaci dat aplikace (farma, web+webjoby, atp.), pro distribuovanou invalidaci  je udělána příprava.

### Implementace
Je potřeba dědit z třídy, viz tato ukázka kódu minimálního kódu.

```csharp
public class UzivatelLookupService : LookupServiceBase<string, Uzivatel>, IUzivatelLookupService
{
	public UzivatelLookupService(IEntityLookupDataStorage lookupStorage, IRepository<Uzivatel> repository, IDataSource<Uzivatel> dataSource, IEntityKeyAccessor entityKeyAccessor, ISoftDeleteManager softDeleteManager) : base(lookupStorage, repository, dataSource, entityKeyAccessor, softDeleteManager)
	{
	}

	public Uzivatel GetUzivatelByEmail(string email) => GetEntityByLookupKey(email);

	protected override Expression<Func<Uzivatel, string>> LookupKeyExpression => uzivatel => uzivatel.Email;

	protected override LookupServiceOptimizationHints OptimizationHints => LookupServiceOptimizationHints.None;
}
```

Implementována je zejména vlastnost - `LookupKeyExpression`, jejíž návratovou hodnototu je expression pro získání párovacího klíče. Zde tedy říkáme, že párujeme uživatele dle emailu. Druhou implementovanou vlastností je `OptimizationHints`, vysvětlení viz níže.

Metoda `GetUzivatelByEmail` je pak službou třídy samotné, kterou mohou její konzumenti používat. Pod pokličkou jen volá metody `GetEntityByLookupKey`.

#### IncludeDeleted
By default nejsou uvažovány (a vraceny) příznakem smazané záznamy. Pokud mají být použity, je třeba provést override vlastnosti `IncludeDeleted` a vrátit `true`.

#### Filter
Pokud nás zajímají jen nějaké instance třídy (neprázdný párovací klíč, objekty v určitém stavu, atp.), lze volitelně provést override vlastnosti `Filter` a vrátit podmínku, kterou musí objekty splňovat.

#### ThrowExceptionWhenNotFound
Pokud není podle klíče objekt nalezen, je vyhozena výjimka `ObjectNotFoundException`. Pokud nemá být vyhozena výjimka a má být vrácena hodnota `null`, lze provést override této vlastnosti tak, aby vracela `false`.

### OptimizationHints
Pro efektivnější fungování invalidací (viz níže) je možné zadat určité hinty, např., pokud je entita readonly a tedy nemůže být za běhu aplikace změněna, nemusí k žádné invalidaci docházet.

### Dependency Injection
Třídy je nutno do DI containaru instalovat nejen pod sebe sama, ale ještě pod servisní interface, který zajistí možnost invalidace dat při uložení nějaké entity (viz dále).

```csharp
	services.WithEntityPatternsInstaller()
		...
		.AddLookupService<IUserLookupService, UserLookupService>();
 ```

### Invalidace
Pokud dojde k uložení entity, je potřeba lookup data nějakým způsobem invalidovat. S objektem se může stát spousta věcí - změna vyhledávacího klíče, smazání příznakem, změna jiných vlastností tak, aby objekt již neodpovídal filtru, atp. Je třeba zajistit, aby lookup data držená službou, byla aktuální.

Zvolené řešení je efektivnější než prostá invalidace, data jsou rovnou aktualizována na nové hodnoty.

To lze omezit tam, kde jsou entity např. readonly, viz `OptimizationHints`.

### ClearLookupData
Pokud chceme ručně vynutit odstranění dat z paměti, je k dispozici metoda `ClearLookupData`.

Užitečné to může být pro situace:

* Jednorázově jsme použili lookup service a víme, že ji dlouho nebudeme potřebovat - pak zavolání metody uvolní paměť alokovanou pro lookup data.
* Došlo k úpravě dat mimo `UnitOfWork` (třeba stored procedurou) a potřebujeme dát lookup službě vědět, že lookup data již nejsou aktuální.

### Použití repository
Objekty jsou po nalezení klíče v lookup datech vyzvednuty z repository. Přínos tohoto chování je takový, že získaný objekt je trackovaný a mohl být získán z cache, bez dotazu do databáze.

Pozor na scénáře, kde se ptáme do lookup služby opakovaně pro necachované objekty (nebo cachované, které ještě v cache nejsou), každé volání pak může udělat dotaz do databáze právě pro získání instance z repository.

Pro tuto situaci je k dispozici metoda, která nevrátí instanci entity, ale jen její klíč - `GetEntityKeyByLookupKey`. Je pak možno implementačně získat klíče všech objektů, které můžeme ve vlastním kódu přehodit metodě `GetObjects` repository. Pokud máme problém poté objekty roztřídit znovu dle klíčů, můžeme uvažovat takto:

1. Nejprve získáme všechny klíče entit dle vyhledávané vlastnosti
2. Poté všechny entity načteme pomocí `repository.GetObjects(...)`, čímž dostaneme objekty do paměti (identity mapy, `DbContext`).
3. Nyní se můžeme do lookup služby (a metody `GetEntityByLookupKey`) ptát jeden objekt po druhým, vracení objektů z repository již nebude dělat dotazy do databáze, neboť jsou již načteny.

### Použití non-Int32 primárního klíče
K dispozici je bázová třída `LookupServiceBase<TLookupKey, TEntity, TEntityKey>`, kde jako `TEntityKey` lze zvolit skutečný typ primárního klíče.

#### Použití složeného klíče
Pro vyhledávání je možno použít složený klíč, klíč musí mít vlastní třídu, která musí zajistit fungování porovnání v `Dictionary`, tedy předefinovat porovnání. S úspěchem lze použít v implementaci anonymní třídu, byť se trochu zhorší kvalita kódu tím, že jako typ klíče musíme uvést typ object.

#### Použití více entit pod jedním klíčem
Není podporováno, je vyhozena výjimka.

### Časová složitost
Snažíme se, aby složitost vyhledání byla O(1).

Konstantní složitost samozřejmě neplatí pro první volání, které sestavuje vyhledávací slovník.

### Implementační detail
Sestavení lookup dat provede jediný dotaz do databáze pro všechny objekty (s ohledem na `IncludeDeleted` a `Filter`). Nenačítají se celé instance entit, ale jen jejich projekce, tj. vrací se netrackované objekty, tj. nenaplní se identity mapa (`DbContext`) instancemi entit, ve kterých je vyhledáváno.
## Lokalizace

Pokud máme model s lokalizacemi, pak službou `ILocalizationService` získáváme pro entitu z kolekce `Localizations` hodnotu pro zvolený jazyk.
Hodnotu můžeme získat pro “aktuální” nebo zvolený jazyk.

```csharp
ContryLocalization countryLocalization = localizationService.GetCurrentLocalization(country);
ContryLocalization countryLocalization = localizationService.GetLocalization(country, czechLanguage);
```

> ℹ️ Služba nezajišťuje načtení kolekce `Localizations` z databáze, zajišťuje jen výběr požadované hodnoty z této kolekce.

Logika hledání pro daný jazyk je postavena takto:

* Pokud existuje položka pro zadaný jazyk, je použita tato.
* Není-li nalezena, zkouší se hledat pro jazyk dle "[neutrálnější culture](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=net-5.0)" jazyka.
* Není-li nalezena, zkouší se hledat pro jazyk dle invariantní culture (prázdný `UICulture`).

Například pro uživatele pracující v češtině se hledá položka pro jazyky dle `UICulture` postupně pro "cs-cz", "cs", "".

### Registrace DI
Použití služby je podmíněno registrací do DI containeru, což můžeme udělat extension metodou `AddLocalizationServices`.

```csharp
services
  ...
  .AddLocalizationServices<Language>()
  ...;
```

## DataLoader

Explicitní loader - dočítá objekty, které dosud nebyly načteny.

### Činnost
* Explicitní loader - dočítá objekty, které dosud nebyly načteny.
* Objekty jedné vlastnosti jsou dočteny jedním dotazem.
* Při dotazování se nepoužívá join přes všechno načítané, vždy jde o dotaz do tabulky, ze které se načítá daná vlastnost (žádné joiny).
* Např. načtení `faktura => faktura.Dodavatel.Adresa.Zeme` spustí do databáze 3 dotazy - načtení dodavatelů, načtení adres a načtení zemí (pro všechny načítané faktury naráz). Nevadí, pokud je některá z vlastností po cestě null.
* Spoléhá se na change tracker, objekty, ke kterým jsou dočítány "závislosti", musí být trackované. Tato podmínka je testována a v případě nesplnění je vyhozena výjimka.
* **Objekty musí mít primární klíč typu `Int32`.**
* Není vyžadována dualita cizího klíče a navigační property (není tedy vyžadována existence obou sloupců `auto.Barva` a `auto.BarvaId`, stačí samotné `auto.Barva`).
* Neprovádí dotazy do databáze pro nově založené objekty (příklad: nově zakládanému a ještě neuloženému uživateli nemůžeme z databáze načítat role, když uživatel v databázi ještě není)
* Instance kolekcí inicializuje na prázdné (pro `IList<>` a `List<>`), pokud jsou `null`.

### Chování ohledně foreign keys
Při načítání referenci spoléhá na hodnoty cizích klíčů, potažmo jako [shadow properties](https://learn.microsoft.com/en-us/ef/core/modeling/shadow-properties). 

Mějme tedy příklad:

```sharp
Auto auto = autoRepository.GetObject(1); // načte auto s Id 1, Barva bude null, BarvaId řekněme např. 2.
auto.BarvaId = 5; // změníme BarvaId na jinou hodnotu
dataLoader.Load(auto, a => a.Barva); // pokusíme se dočíst vlastnost Barva
```

Pod pokličkou se provede:

```csharp
dbContext.Set<Barva>().Where(barva => barva.Id == 5).ToList();
```

Čímž se načte barva podle hodnoty cizího klíče objektu v paměti, nikoliv podle databáze (tam může být aktuálně třeba `BarvaId == 2`).
Jinými slovy, po načtení bude mít auto přiřazenu do vlastnosti `Barva` instanci s `Id` 5.

> ℹ️ Tímto chováním se DataLoader v EF Core liší od implementace DataLoader v EF 6.

### Metody

* `Load`, `LoadAsync` - přijímá jeden objekt, ke kterému jsou dočteny požadované nenačtené vlastnosti
* `LoadAll`, `LoadAllAsync` - přijímá kolekci objektů, kterým jsou dočteny požadované nenačtené vlastnosti
* Podporuje fluent API pro načítání dalších objektů (`dataLoader.Load(...).ThenLoad(...).ThenLoad(...)`), viz příklady.

### Příklady

```csharp
dataLoader.Load(jednoAuto, auto => auto.Vyrobce).ThenLoad(vyrobce => vyrobce.Kategorie);
dataLoader.Load(jednoAuto, auto => auto.Vyrobce.Kategorie); // funguje také se zřetězením
dataLoader.LoadAll(mnohoAut, auto => auto.Vyrobce.Kategorie); // mnohoAut = kolekce, pole, ... (IEnumerable<Auto>)
dataLoader.LoadAll(mnohoAut, auto => auto.NahradniDily).ThenLoad(nahradniDil => nahradniDil.Dodavatel); // načítání objektů v kolekci
```

### Kolekce M:N
> ⚠️ Entity Framework Core 5.x přináší podporu pro vazby typu M:N (viz dokumentace), avšak HFW pro práci s kolekcemi nemá podporu. DataLoader při pokusu o načtení M:N kolekce neřízeně spadne.

### Podpora kolekcí s filtrováním smazaných záznamů
* Kolekce s filtrováním smazaných záznamů jsou dataloaderem podporovány.
* Pokud model obsahuje kolekci `Xyz` (obvykle nepersistentní) a `XyzIncludingDeleted` (obvykle persistentní), pak je použití kolekce Xyz automaticky nahrazeno načtením kolekce `XyzIncludingDeleted`.
* Konvence je dána pojmenováním kolekcí (přípona `IncludingDeleted`), žádné další testy vůči konfiguraci EF nejsou prováděny.
* Pokud je dále použito `ThenLoad(...)`, načtou se hodnoty jen nesmazaným záznamům, mají-li se načíst hodnoty i ke smazaným záznamům, je třeba použít kolekci `XyzIncludingDeleted`, lepší pochopení dá následující příklad.

#### Příklad

```csharp
public class Master
{
    public int Id { get; set; }
    public ICollection<Child> Children { get; } // nepersistentní
    public IList<Child> ChildrenIncludingDeleted { get; } = new List<Child>();// persistentní
    ...
}
 
dataLoader.Load(master, m => m.Children); // pod pokličkou je transformováno na načtení ChildrenIncludingDeleted, načteny jsou proto všechny Child (vč. příznakem smazaných) k danému masteru
dataLoader.Load(master, m => m.Children).ThenLoad(c => c.Boss); // načteny jsou všechny Children daného masteru, z nich se vyberou jen nesmazané a k těm se načte vlastnost Boss
dataLoader.Load(master, m => m.ChildrenIncludingDeleted).ThenLoad(c => c.Boss); // vlastnost Boss je načtena i smazaným Childům
```

#### Omezení
Je požadováno, aby filtrovaná kolekce dokázala během práce data loaderu vrátit nesmazané objekty.
Pokud by filtrovaná kolekce vyhodnocovala příznak na dalším objektu (např. `me => me.Another.Deleted == null`), může se stát, že tento další objekt `Another` ještě není v proběhu práce dataloaderu načten a dojde tak k vyhození `NullReferenceException`.

### DataLoader jako závislost v unit testech
K dispozici je `FakeDataLoader`, který nic nedělá. Lze tak použít v unit testech, které pracují s daty v paměti a nemají co dočítat.

```csharp
// Arrange

// připravíme data source obsahující zadané záznamy
FakeUserDataSource userDataSource = new FakeUserDataSource(...); 

// připravíme fake data loaderu
FakeDataLoader dataLoader = new FakeDataLoader();

// použijeme data loader jako závislost v testované třídě
ITestedService service = new TestedService(..., fakeUserDataSource, fakeDataLoader, ...);

// Act
...
```

## Cachování

### Jak to funguje
Implementace cachování je realizována na úrovni `Repositories`, `DbDataLoader` a `UnitOfWork`:

* `XyDbRespository.GetObject[Async]` - pokud nemá objekt v identity mapě, pokusí se ho najít v cache, pokud není ani v cache, načítá jej z databáze, poté jej uloží do cache
* `XyDbRespository.GetObjects[Async]`- objekty, které nemá v identity mapě se pokusí najít v cache, objekty, které nejsou ani v cache, načítá z databáze a uloží je do cache
* `XyDbRepository.GetAll[Async]()` - hledá v cache identifikátory objektů
* `DbDataLooader.Load[Async]`, `DbDataLooader.LoadAll[Async]` - při načítání referencí i kolekcí se pokusí najít objekty v cache, objekty, které nejsou v cache, načítá z databáze a uloží je do cache
* `DbUnitOfWork.Commit[Async]` - invaliduje položky v cache
* `XyEntries.Item` - pod pokličkou volá `XyDbRepository.GetObject`

Implementaci cachování zajišťuje zejména `IEntityCacheManager` a jeho implementace.

Ve výchozí konfiguraci (viz dále) je použit `EntityCacheManager`, který realizuje cachování se závislostmi:

* `IEntityCacheSupportDecision` - rozhoduje, zda je daná entita cachovaná či nikoliv
* `IEntityCacheKeyGenerator` - definuje, pod jakým klíčem bude entita uložena do cache
* `IEntityCacheOptionsGenerator` - určuje další parametry položky v cache (priorita, sliding expirace)
* `IEntityCacheDependencyManager` - poskytuje klíč pro cache dependencies

> ⚠️ **Cachování kolekcí**
>
> Cachování kolekcí funguje spolehlivě pro objekty, které nepřecházejí mezi různými parenty. Tj. cachování funguje v obvyklých typických scénářích - objekt s lokalizacemi, faktura s řádky faktur, atp.
>
> **Cachování kolekcí však nefunguje tam, kde mohou prvky kolekce přecházet mezi různými parenty**, např. pokud budu přepínat zaměstnanci jeho nadřízeného zaměstnance a tento nadřízený zaměstnanec má kolekci svých podřízených, pak cachování této kolekce bude vykazovat chyby. Pokud má být v tomto scénáři nadřízený zaměstnanec cachován, nesmíme mu zapnout cachování kolekcí. Nesmíme ani použít "cachování všech entit se sliding expirací", jak je uvedeno níže.
>
> (Důvod: Invalidace cache se provádí po uložení změn. Po uložení změn vidíme jen nový, aktuální stav objektů. Nejsme schopni tedy invalidovat cache pro původního nadřízeného zaměstnance, neboť nevíme, kdo to byl.)

### Konfigurace

#### Výchozí konfigurace

Ve výchozí konfiguraci jsou:

* cachovány entity, které označeny atributem `Havit.Data.EntityFrameworkCore.Abstractions.Attributes.CacheAttribute` (zjednodušeně),
* v atributu lze nastavit prioritu položek v cache, sliding a absolute expiraci,
* v atributu lze zakázat cachování klíčů `GetAll`.
* kolekce jsou cachovány, pokud je cílový typ cachován (umožňuje cachovat entities)

> ℹ️ Je vyžadována registrace závislosti `ICacheService`, kterou knihovny k EFCore neřeší, je třeba ji zaregistrovat do DI containeru samostatně.

### Ukázková situace: Reference

```csharp
public class Auto
{
	public int Id { get; set; }
	public Barva Barva { get; set; }
	// ...
}

[Cache]
public class Barva
{
	public int Id { get; set; }		
	// ...
}
```

`Barva` je cachovaná, `Auto` nikoliv.

Z cache se proto mohou odbavovat např.:
* `BarvaRepository.GetObject(...)`
* `BarvaRepository.GetAll()`
* `BarvaEntries.Black`
* `DataLoader.Load(auto, a => a.Barva)`

### Ukázková situace: Kolekce 1:N

```csharp
[Cache]
public class Stav : ILocalized<StavLocalization>
{
	public int Id { get; set; }
	public List<StavLocalization> Localizations { get; } = new List<StavLocalization>();
}

[Cache]
public class StavLocalization : ILocalization<Stav>
{
	public int Id { get; set; }
	public Stav Parent { get; set; }
	public int ParentId { get; set; }
	public Language Language { get; set; }
	public int LanguageId { get; set; }
	// ...
}
```

Číselník stavů je cachovaný vč. svých lokalizací.

Attribut `[Cache]` je třeba uvést na obou třídách, žádný předpoklad, "když X je cachované, tak XLocalization také" není uplatňován.

Z cache se proto mohou odbavovat např.:

* `StavRepository.GetObject(...)`
* `StavRepository.GetAll()`
* `StavEntries.Aktivni`
* (obdobně `StavLocalizationRepository`, `StavLocalizationEntries`, avšak nemá valného významu takto použít)
* `DataLoader.LoadAll(stavy, s => s.Localizations)`

#### Ukázková situace: Dekomponovaný vztah M:N do asociační třídy s kolekcí 1:N

```sharp
public class LoginAccount
{
	public int Id { get; set; }
	public List<Membership> Memberships { get; } = new List<Membership>();
	// ...
}

[Cache]
public class Membership
{
	public LoginAccount LoginAccount { get; set; }
	public int LoginAccountId { get; set; }
	public Role Role { get; set; }
	public int RoleId { get; set; }
}

[Cache]
public class Role
{
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }
	// ...
}
```

`LoginAccount` není cachován, třídy `Membership` a `Role` jsou cachovány.

Z cache se proto mohou odbavovat např.:
* `RoleRepository.GetObject(...)`
* `RoleRepository.GetAll()`
* `RoleEntries.Administrator`
* `DataLoader.Load(loginAccount, la => la.Membership).ThenLoad(m => m.Role)`

Cachování `Membership` je pro daný scénář nutné, ale nemá jiného významu, neboť nemáme pro třídy reprezentující M:N vazbu nepoužíváme repository.

Pokud nebude `Membership` označen jako cachovaný, nebude se `LoginAccountu` cachovat kolekce `Memberships`.

### Cachování vypnuto
Pokud je nutné cachování vypnout (např. jednorázově běžící konzolovky, které jen sežerou paměť, ale data v cache nevyužijí), je možné toto řešit extension metodou:

```csharp
services
    ...
    .AddDataLayerServices(new ComponentRegistrationOptions().ConfigureNoCaching())
    ...;
```

Není pak potřeba ani registrovat závislost `ICacheService`.

### Cachování všech entit s použitím sliding expirace [Experimental]
Myšlenka: Na chvíli si do cache umístíme cokoliv, s čím pracujeme. Až s tím nebudeme pracovat, vypadne to z cache. Cachujeme tedy vše, ale zároveň všemu omezujeme dobu expirace.

Na rozdíl od výchozí konfigurace:
* se neohlíží na `[Cache]`, cachováno je vše,
* v atributu nastavená priorita položek v cache, sliding a absolute expirace se respektuje (použije), pokud není uvedena sliding expirace, použije se výchozí.

```csharp
services
    ...
    .AddDataLayerServices(new ComponentRegistrationOptions().ConfigureCacheAllEntitiesWithDefaultSlidingExpirationCaching(timeSpan))
    ...;
```

### Cache dependencies

Pokud potřebujeme do cache uložit objekt, který bychom chtěli invalidovat v případě změněny nějakého konkrétního objektu v databázi, případně změny jakéhokoliv objektu v databázi, můžeme objekt do `ICacheService` registrovat se závislostmi.

Klíče závislostí lze získat ze služby `IEntityCacheDependencyManager`:

* `GetSaveCacheDependencyKey` - závislosti jsou vyhozeny při uložení entity daného typu s daným `Id` (např. pro invalidaci nějaké vlastnosti subjektu, pokud se subjekt změní).
* `GetAllSaveCacheDependencyKey` - závislosti jsou vyhozeny při uložení (a založení a smazání) jakékoliv entity daného typu (např. pro invalidaci součtu částek všech faktur).

Předpokládáme úpravu tohoto interface na základě dalších požadavků.

### Invalidace

Invalidace provádí výhradně DbUnitOfWork v metodě `Commit[Async]`.

Invaliduje se při uložení entity:
* entita samotná
* klíče pro `GetAll` typu dle entity
* kolekce, jichž je entita členem
* po invalidaci entity se uložená entita opět uloží do cache (tj. omezí se nutné načtení entity po její změně)
* je myšleno na distribuovanou invalidaci v lokálních caches

### Nepodporované scénáře

Uložení/vyzvednutí z cache:

* Owned Types (cachování entity s owned types není a nebude, při použití Owned Entity Types je třeba úplně vypnout cachování - viz níže.)
* Vazba 1:1 (v případě potřeby prověříme možnost doimplementování)

Veškeré obejití `UnitOfWork`:
* např. Cascade Deletes (na úrovni databáze)

### Modely s Owned Entity Types
Problémy, které způsobuje použití Owned Entity Types:

* ChangeTracker sleduje změny na owned types samostatně, v pokud má `Person` vlastnost pro domácí adresu `HomeAddress` (owned) typu `Address`, pak při změně (např.) ulice `ChangeTracker` vidí změnu v owned entitě `Address`, nikoliv v `Person`. To ztěžuje invalidace. (Pozn: Ale ukládá to efektivně, takže musí jít nějak rozumně pospojovat entitu a jí použité owned typy).
* Současná implementace uložení `Person` do cache neukládá owned entity types, tj. při odbavení položky z cache nebudou hodnoty pro vlastnosti dobře nastaveny.


## Generovaná metadata

Na základě modelu jsou pro všechny stringové vlastnosti generována metadata s definicí jejich maximálních délek dle attributu `[MaxLength]` (viz [Model](#model)). Pro vlastnosti označované jako "maximální možná délka" se použije hodnota Int32.MaxValue, byť to není správně (nejde uložit tolik znaků, ale tolik byte). Jiná metadata negenerujeme.

Metadata jsou generována přímo do modelu a jsou určena pro definici maximálních délek např. ve view modelu. Změnou délky textu v modelu, se po přegenerování kódu změní vygenerované konstanty, které změní maximální velikosti viewmodelu...

#### Příklad

```csharp
public static class LanguageMetadata
{
    public const int CultureMaxLength = 10;
    public const int NameMaxLength = 200;
    public const int SymbolMaxLength = 50;
    public const int UiCultureMaxLength = 10;
}
```
## SoftDeleteManager

Implementeace `ISoftDeleteManager` rozhodují o tom, zda daná entita podporuje soft delete a pokud ano, poskytuje metody pro nastavení příznaku smazání (a odebrání příznaku smazání).

Výchozí implementace `SoftDeleteManager` říká, že soft-delete jsou ty entity, které mají vlastnost `Deleted` typu `Nullable<DateTime>`.

## UnitOfWork

`IUnitOfWork` poskytuje metody:

* `Add[Range]ForInsert`
* `Add[Range]ForInsertAsync` - určeno pro použití [HiLo strategie](https://davecallan.com/how-to-use-hilo-with-entity-framework/) generování Id
* `Add[Range]ForUpdate`
* `Add[Range]ForDelete`
* `Commit[Async]`
* `RegisterAfterCommitAction`
* `Clear` - Umožňuje vyčistit ChangeTracker podkladového DbContextu.

#### Add[Range]ForDelete

Entity, které podporují soft delete jsou metodou `Add[Range]ForDelete` označeny jako smazané příznakem, nedojde k jejich fyzickému smazání, ale k aktualizaci (`UPDATE`).

Fyzické smazání entity podporující soft delete není aktuálně možné (kdo bude potřebovat, nechť se ozve, doplníme metodu `Add[Range]ForDestroy`).

### RegisterAfterCommitAction
Umožňuje přidat zvenku nějakou akci k provedení po commitu (odeslání emailu, smazání cache, atp.)
Umožnuje přidat jak synchronní akci tak asynchronní akci.
Asynchronní akce funguje pouze v asynchronním commitu, v případě registrace asynchronní akce a spuštění synchronního commitu dojde k vyhození výjimky.

#### Příklad

```csharp
private void ProcessPayment(Payment payment)
{
	...
	// vůbec nevíme, kde je unitOfWork.Commit(), ale víme, že po jeho spuštění dojde k odeslání notifikace
	unitOfWork.RegisterAfterCommitAction(() => SendNotification(payment));
	...	
}
```

### Koncept BeforeCommitProcessorů
`DbUnitOfWork` obsahuje koncept, který umožní při volání commitu spustit služby pro každou změněnou entitu ještě před uložením objektu. Je možné tak "na poslední chvíli" provést v entitách nějaké změny.

Pro implementaci nějakého vlastního `BeforeCommitProcessoru` je vhodné dědit z `BeforeCommitProcessor<TEntity>`, což pomůže vypořádat se s dvojicí metod `Run` a `RunAsync` v interface `IBeforeCommitProcessor<TEntity>`.

Službu je potřeba si zaregistrovat službu do DI containeru pod interface `IBeforeCommitProcessor<TEntity>`.

Metoda vrací hodnotu výčtu `ChangeTrackerImpact` a má pomoci `UnitOfWorku` s výkonovou optimalizací. Hodnota říká, zda změna provedená before commit processorem může ovlivnit changetracker tak, že je nutné jej spustit znovu (což je potřeba typicky jen při přidání nové entity).

#### Příklad

Viz např. implementace [`SetCreatedToInsertingEntitiesBeforeCommitProcessor`](https://dev.azure.com/havit/DEV/_git/002.HFW-HavitFramework?path=%2FHavit.Data.EntityFrameworkCore.Patterns%2FUnitOfWorks%2FBeforeCommitProcessors%2FSetCreatedToInsertingEntitiesBeforeCommitProcessor.cs&_a=contents&version=GBmaster).

```csharp
public class MyEntityBeforeCommitProcessor : BeforeCommitProcessor<MyEntity>
{
    public ChangeTrackerImpact Run(ChangeType changeType, MyEntity changingEntity)
    {
		if (changeType == ChangeType.Insert)
		{
			// do something
		}
		return ChangeTrackerImpact.NoImpact;
    }
}
```

#### SetCreatedToInsertingEntitiesBeforeCommitProcessor
Pro nově založené objekty, které mají vlastnost `Created` typu `DateTime` a v této vlastnosti je hodnota `default(DateTime)` nastaví aktuální čas (z `ITimeService`). Tj. automaticky nastavuje hodnotu `Created` entitám, které ji nastavenou nemají.

Je použit automaticky (díky registraci do DI containeru).

### Koncept EntityValidatorů

Před uložením objektů (a po spuštění `BeforeCommitProcessorů`) se spustí validátory entit, které umožňují kontrolovat jejich stav. Pokud je zjištěna nějaká validační chyba, je vyhozena výjimka typu `ValidationFailedException`.

Pro implementaci nějakého vlastního `EntityValidatoru` je třeba implementovat interface `IEntityValidator<TEntity>`. K implementaci je jediná metoda `Validate`, jež má na výstupu kolekci `IEnumerable<string>` - zjištěných chyb při validaci.

Dále je třeba službu zaregistrovat do DI containeru.

#### Příklad

```csharp
public class MyEntityEntityValidator : IEntityValidator<MyEntity>
{
	IEnumerable<string> Validate(ChangeType changeType, MyEntity changingEntity)
	{
		if (changingEntity.StartDate >= changingEntity.EndDate)
		{
			yield return "Počáteční datum musí předcházet koncovému datu.";
		}
	}
}
```

### IValidatableObject.Validate()
Jednou ze specifických možností implementace `EntityValidatoru` je `IValidatableObject.Validate()` přímo entitě.


```csharp 
public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
{
	if ((this.Parent == null) && (this.Id != (int)Project.Entry.Root))
	{
		yield return new ValidationResult($"Property {nameof(Parent)} is allowed to be null only for Root project.");
	}
	if ((this.Depth == 0) && (this.Id != (int)Project.Entry.Root))
	{
		yield return new ValidationResult($"Value 0 of {nameof(Depth)} property is allowed only for Root project.");
	}
}
```
Tyto validace lze pak do commit-sekvence zapojit zaregistrováním služby `ValidatableObjectEntityValidator` do DI containeru:

```csharp
services.AddSingleton<IEntityValidator<object>, ValidatableObjectEntityValidator>();
```

> ℹ️ ValidatableObjectEntityValidator nezajišťuje validace dle DataAnnotations atributů, jako jsou např. [Required], [MaxLength] apod.

### Pořadí akcí v commitu

Během commitu dochází postupně k těmto akcím:
* zavolání metody BeforeCommit
* spuštění BeforeCommitProcessorů
* spuštění EntityValidátorů
* uložení změn do databáze (DbContext.SaveChanges[Async]).
* zavolání metody AfterCommit (zajišťuje volání akcí registrovaných metodou RegisterAfterCommitAction)

# Seedování dat
Seedování dat je automatické založení dat v databázi.

### Definice dat k seedování
Seedování dat provádí třídy implementujíící interface `IDataSeed`. S jednoduchostí lze vytvořit třídu dědící ze třídy `DataSeed<>`, která tento interface poskytuje, je třeba jen implementovat template metody `SeedData` a `SeedDataAsync`.

Vytvořením instancí dat, metodou `For` a provedené konfigurace nad jejím výsledkem, se připraví data, která mají být v databázi. Připravená data se předhodí metodě `Seed` nebo `SeedAsync`.
(Poznámka: Metoda `For` vychází z otevřenosti pro další rozšíření, kdy se mohou data získávat z jiných zdrojů, např. `ForCsv`, `ForExcel`, `ForResource`. To však není implementováno a budeme řešit, až bude potřeba.)

### Párování pomocí sloupce Id (>99% případů)
Pokud jde o systémový číselník, do kterého nejsou **vkládány** hodnoty uživatelsky, pak můžeme na sloupci vypnout autoincrement a tím použít vlastní hodnoty pro Id.

Pokud jde o číselník, do kterého jsou vkládány hodnoty uživatelsky, můžeme namísto autoincrementu použít sekvenci, což nám umožní, abychom stále mohli použít vlastní hodnoty pro Id.

Seedovaná data s daty v databázi jsou pak párována pomocí Id.

#### Příklad bez autoincrementu

```csharp
public class Role
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)] // nepoužijeme autoincrement, čímž umožníme vkládat vlastní hodnoty do sloupce Id
    public int Id { get; set; }
 
    ...
 
    public enum Entry
    {
        Writer = -3,
        Reader = -2,
        Administrator = -1
    }
}


public class RoleSeed : DataSeed<CoreProfile>
{
    public override void SeedData()
    {
        Role[] roles = new[]
        {
            new Role
            {
                Id = (int)Role.Entry.Administrator, // nastavíme hodnotu pro sloupec Id
                Name = "Administrátor"
            },
            ... // Weader, Writer
        };
        Seed(For(roles).PairBy(item => item.Id)); // řekneme, že se má párovat dle sloupce Id
    }
}
```

#### Příklad se sekvencí

```csharp
public class User
{
    public int Id { get; set; }
    ...
 
    public enum Entry
    {
        SystemUser = -1
    }
}
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.Id).HasDefaultValueSql("NEXT VALUE FOR UserSequence");
    }
}
 
// dále je třeba na modelu (v DbContextu) zajistit existenci sekvence
// modelBuilder.HasSequence<int>("UserSequence");
public class UserSeed : DataSeed<CoreProfile>
{
    public override void SeedData()
    {
        User[] users = new[]
        {
            new User
            {
                Id = (int)User.Entry.SystemUser, // nastavíme hodnotu pro sloupec Id
                Name = "(Systémový uživatel)"
            }
        };
        Seed(For(users).PairBy(item => item.Id)); // řekneme, že se má párovat dle sloupce Id
    }
}
```

#### Párování pomocí sloupce Symbol (<1% případů)

> ℹ️ Toto řešení jsme implementovali a používali jako první verzi v Entity Framework 6. V Entity Framework Core nemá valného použití díky možnosti využití sekvence a řešení dle předchozího odstavce.

Pokud z nějakého důvodu potřebujeme autoincrement na číselníku, do nějž potřebujeme seedovat data, např. pokud nemůžeme použít sekvenci, pak musíme párovat data v databázi s párovanými daty podle jiného sloupce než podle Id. V takovém případě používáme párování dle sloupce Symbol, který je (bohužel díky zpětné kompatibilitě) výchozím párováním, pokud sloupec existuje.

```csharp
public class LanguageSeed : DataSeed<DefaultDataSeedProfile>
{
    public override void SeedData()
    {
        Language czechLanguage = new Language
        {
            Name = "Česky",
            Culture = "cs-CZ",
            UiCulture = "",
            Symbol = Language.Entry.Czech.ToString()
        };
 
        Seed(For(czechLanguage)); // PairBy(item => item.Symbol) je default, který není třeba uvádět
    }
}
```

### Konfigurace
Poskytuje fluidní API, následující metody lze řetězit.

#### Párování seedovaných dat s daty v databázi
Metodami `PairBy` a `AndBy` lze určit sloupce, pomocí kterých budou seedovaná data párována s daty v databázi. Pro reference je nutno použít cizí klíče, nikoliv navigation property (jinými slovy: je nutno použít `LanguageId`, nikoliv `Language`).

```csharp
Seed(For(...).PairBy(item => item.LanguageId).AndBy(item => item.ParentId));
Seed(For(...).PairBy(item => item.LanguageId, item => item.ParentId));
```

#### Aktualizace záznamů
Neexistující záznamy jsou standardně založeny. Existující záznamy aktualizovány.

Aktualizace existujících záznamů lze potlačit metodou `WithoutUpdate`:

```csharp
Seed(For(...).WithoutUpdate());
```

Potlačení **aktualizace** pouze vybraných **vlastností** (sloupců) lze metodou `ExcludeUpdate`:

```csharp
Seed(For(...).ExcludeUpdate(item => item.UserRank)); // sloupec UserRank nebude aktualizován
```

#### Závislé záznamy / Kolekce
Po uložení "parent" záznamů je možné zajistit uložení i jejich referencí či kolekcí (například lokalizace číselníků). Jaké hodnoty budou ukládány je určeno metodami `AndFor` nebo `AndForAll`. Tyto metody dále umožňuje provést nastavení seedování těchto záznamů.

Pro řešení "jak získat Id aktuálně uloženého záznamu" lze použít metodu `AfterSave`.

Dobrou ukázkou je níže ukázka výchozí konfigurace pro lokalizované tabulky.

### Výchozí konfigurace

#### Symbol
Pokud třída obsahuje sloupec `Symbol`, je podle něj automaticky párováno (není-li určeno jinak; zpětná kompatibilita, sorry):

```csharp
Seed(For(...).PairBy(item => item.Symbol));
```

#### Lokalizované tabulky
Lokalizovaným třídám zajistí uložení lokalizací, lokalizacím zajistí párování dle `ParentId` a `LanguageId` (aktuálně hardcodováno v HFW). Že jde o lokalizované a lokalizační třídy se poznává podle implementace `ILocalization<,>` a `ILocalized<,>`.

```csharp
// pseudokód popisující implementaci
Seed(For(...)
     // po uložení každého lokalizovaného záznamu nastavíme jeho lokalizacím ParentId
    .AfterSave(item => item.SeedEntity.Localization.ForEach(localization => localization.ParentId = item.PersistedEntity.Id))
     // po seedu lokalizovaných dat budeme seedovat lokalizace, které budeme párovat pomocí ParentId a LanguageId
    .AndForAll(item => item.Localization, configuration =>
         {
            configuration.PairBy(item => item.ParentId, item => item.LanguageId);
         }));
```

Ve skutečnosti je tato výchozí hodnota implementována mnohem komplikovaněji, efekt je takovýto.

### Závislost na jiných seedovaných datech
Pokud chceme seedovat data, potřebujeme závislosti (například pro lokalizovaná data potřebujeme mít provedeno seedování jazyků).

Metodou `GetPrerequisiteDataSeeds` lze říct, na jakých seedech je tento závislý. V ukázce musí nejprve proběhnout `EmailTemplateSeed` a `LanguageSeed` než je spuštěn `EmailTemplateLocalizationSeed` (HFW řeší i detekci cyklů, atp.).

Návratovou hodnotou metody je `IEnumerable` typů, nikoliv instancí, implementace je tak náchylná na chybu - např. použití `typeof(Language)` namísto `typeof(LanguageSeed)`. Taková chyba je však v runtime detekována a je vyhozena výjimka.

```csharp
public class EmailTemplateLocalizationSeed : DataSeed<DefaultDataSeedProfile>
{
    public override void SeedData()
    {
        ...
    }
 
    public override IEnumerable<Type> GetPrerequisiteDataSeeds()
    {
        yield return typeof(EmailTemplateSeed);
        yield return typeof(LanguageSeed);
    }
}
```

### Profily

Data je možné seedovat pro různé účely - produkční data, testovací data pro testování funkcionality A, testovací data pro testování funkcionality B, atp.
Pro tento scénář máme k dispozici profily pro seedování dat, které určují, které seedy se mají v jakém profilu spustit.
Profil je třída implementující `IDataSeedProfile`, např. děděním z abstraktní třídy `DataSeedProfile`.

Jaká data patří do jakého profilu je určeno generickým parametrem u třídy `DataSeed`. Profily mohou mít závislosti na jiných profilech, jsou definovány pomocí metody `GetPrerequisiteProfiles()`.

```csharp
public class TestDataProfile : DataSeedProfile
{
    public override IEnumerable<Type> GetPrerequisiteProfiles()
    {
        yield return typeof(DefaultDataSeedProfile);
    }
}
```

Jak se spustí jednotlivé seedování dat jednotlivých profilů je uvedeno dále.

### Spuštění seedování
Spuštění seedování zajišťuje třída `DataSeedRunner`.
Ta dostává závislosti: Kolekci data seedů, které se mají provést, persister seedovaných dat a strategii rozhodující, zda je třeba seedování pustit.
Profil, který se má spustit je určen generickým parametrem v metodě `SeedData`, viz ukázka.

#### Obvyklé spuštění je při startu aplikace (global.asax.cs, apod.):

Typicky je řešeno v projektu ve třídě `MigrationService`.

```csharp
var dataSeedRunner = serviceScope.ServiceProvider.GetService<IDataSeedRunner>();
await dataSeedRunner.SeedDataAsync<CoreProfile>(false, cancellationToken);
```

### Izolace jednotlivých seedů
Počet objektů sledovaných ChangeTrackerem postupně při volání jednotlivých seedů nenarůstá, což při seedování většího objemu dat znamená dopad na výkon.
Pro izolaci jednotlich seedů se na začátku a konci metody `Seed[Async]` zajistí vyčištění changetrackeru.
(Při ladění jednoho z projektů se dostáváme na pětinásobné zrychlení).
Přes tuto izolaci jednotlivé seedy sdílejí databázovou transakci.

### Omezení spuštění seedování
Aby se nespouštělo seedování dat při každém startu aplikace, pamatují si seedy v databázové tabulce `__SeedData` (zjednodušeně) verzi dataseedů, která byla spuštěna.
V tabulce jsou záznamy pro jednotlivé profily, název profilu je primárním klíčem.

Pokud zjistíme, že daná verze již byla spuštěna, nebude se seedování spouštět.

Verze dataseedů se určí z názvu assembly, z file version a z data (datumu) posledního zápisu assembly. Díky datu poslední zápisu assembly nám funguje seedování i při vývoji, kde se nám jinak název a verze assembly nemění a bez data posledního zápisu assembly bychom při vývoji spustili seedování jen jedenkrát.

Toto je implementováno v `OncePerVersionDataSeedRunDecision`, která je zaregistrována do DI containeru pod `IDataSeedRunDecision`.

K dispozici je ještě strategie `AlwaysRunDecision`, která nic nekontroluje ale zajistí spuštění seedování vždy.




# Dependency Injection

Registrace do DI containeru je podporována pro `IServiceCollection`.
Pro registrace služeb se generuje extension metoda `DataLayerServiceExtensions.AddDataLayerServices`.

```csharp
services
	.AddDbContext<IDbContext, GoranG3DbContext>(optionsBuilder =>
	{
		if (configuration.UseInMemoryDb)
		{
			optionsBuilder.UseInMemoryDatabase(nameof(GoranG3DbContext));
		}
		else
		{
			optionsBuilder.UseSqlServer(configuration.DatabaseConnectionString, c =>
			{
				c.MaxBatchSize(30);
				c.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
			});
		}
		optionsBuilder.UseDefaultHavitConventions();
	})
	.AddLocalizationServices<Language>() // volitelné
	.AddDataLayerServices()
	.AddDataSeeds(typeof(CoreProfile).Assembly)
	.AddLookupService<ICountryByIsoCodeLookupService, CountryByIsoCodeLookupService>();

services.AddSingleton<IEntityValidator<object>, ValidatableObjectEntityValidator>(); // pokud je požadována validace entit pomocí IValidatableObject

services.AddSingleton<ITimeService, ApplicationTimeService>();
services.AddSingleton<ICacheService, MemoryCacheService>();
services.AddSingleton(new MemoryCacheServiceOptions { UseCacheDependenciesSupport = false });
```

