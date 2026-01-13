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

Není žádné výchozí nastavení, jaký connection string bude použit. Vše je řešeno až při použití DbContextu, např. v konfiguraci AddDbContext(...).
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
  Pokud je vlastnost třídy modelu označena atributem DataTypeAttribute s hodnotou DataType.Date pak se použije v databázi datový typ Date.
- `CascadeDeleteToRestictConvention`
  Všem cizím klíčům s nastaví DeleteBehavior na Restrict, čímž zamezí kaskádnímu delete.
- `CacheAttributeToAnnotationConvention`
  Hodnoty zadané v atributu `[Cache]` předá do anotations.

### Volitelné konvence

- `StringPropertiesDefaultValueConvention`
  Pro všechny stringové vlastnosti, pokud nemají výchozí hodnotu, se použije výchozí hodnota `String.Empty`.
  Nastavuje vlastnosti výchozí hodnotu a `ValueGenerated` na `Never` dle [Entity Framework Core - 10 - Know-How - Výchozí hodnoty v databázi vs. uložené hodnoty](https://havit.atlassian.net/wiki/spaces/DEV/pages/556138497/Entity+Framework+Core+-+10+-+Know-How+-+V+choz+hodnoty+v+datab+zi+vs.+ulo+en+hodnoty).

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

Viz dobře napsaná [dokumentace EF Core](https://docs.microsoft.com/cs-cz/ef/core/modeling/).

### Vztah M:N

Entity Framework Core 5.x přináší podporu pro vazby typu M:N (viz [dokumentace](https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#many-to-many)), avšak HFW pro práci s kolekcemi nemá plnou podporu. Kolekce typu M:N je možné omezeně použít, nebude je umět dočíst DbDataLoader a nemají (a nejspíš mít později ani nebudou) řešenou podporu v entity validátorech a before commit processorech.

Příklad řešení v modelu a konfigurace je uveden v sekci [Entity Framework Core - 02 - Model](https://havit.atlassian.net/wiki/spaces/DEV/pages/507281437/Entity+Framework+Core+-+02+-+Model).

## Migrations

Viz dokumentace [migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/).

### Spouštění Migrations

Spuštění migrations a seedů (viz další kapitoly) provádíme typicky při spuštění aplikace.
Samotné spuštění při startu aplikace zajišťuje hosted service `MigrationHostedService`, která migrace a seedy spustí prostřednictvím `MigrationService.UpgradeDatabaseSchemaAndDataAsync`.
