# Konfigurace konvencí přes UseDefaultHavitConventions — záměr a plán implementace

## Záměr

Konfigurace konvencí (`ConventionsOptions`) se dnes provádí overridem virtuální metody `DbContext.GetConventionOptions()`.
Extension metoda `UseDefaultHavitConventions` na `DbContextOptionsBuilder` je prázdný placeholder, který konzumenti už volají.

Cílový stav:

- `ConventionsOptions` se konfigurují v místě registrace DbContextu přes `UseDefaultHavitConventions(conventionsOptions)`.
- Virtuální metoda `GetConventionOptions()` se z `DbContext` **odstraňuje** (breaking change, jsme v preview řadě).
  Dědičný kontrakt convention options (kompozice `base.GetConventionOptions() with { ... }`) se nezachovává.
- `BusinessLayerDbContext` si své odlišné výchozí hodnoty zařídí sám přes `BusinessLayerDbContextSettings` + `OnConfiguring`.

**Pooling:** návrh pooling neomezuje, naopak je s ním přirozenější. `ConfigureConventions` běží jen při sestavení modelu
(jednou per interní service provider + typ contextu), ne per zápůjčka z poolu; options jsou u poolu zmrazené a sdílené.
Pooled scénář s `UseDefaultHavitConventions` pokrývá test v Havit.Data.EntityFrameworkCore.Patterns.Tests
(`ServiceCollectionExternsionsTests`).

## Plán implementace

### 1. Nový `HavitConventionsOptionsExtension` (internal)

Nový soubor `Infrastructure/HavitConventionsOptionsExtension.cs`:

- `internal class HavitConventionsOptionsExtension : IDbContextOptionsExtension` nesoucí `ConventionsOptions ConventionsOptions`
  (nikdy null — konstruktor normalizuje null na `new ConventionsOptions()`).
- `ApplyServices` = NOOP, `Validate` = NOOP.
- Nested `ExtensionInfo : DbContextOptionsExtensionInfo` — šablona:
  `Havit.Data.EntityFrameworkCore.BusinessLayer/Metadata/Conventions/Infrastructure/ConventionSetPluginServiceInstallerExtension.cs`:
  - `GetServiceProviderHashCode()` → `ConventionsOptions.GetHashCode()` (record = value semantics).
    Různé options → oddělené interní service providery → oddělené model cache
    (řeší registraci téhož typu contextu se dvěma různými konfiguracemi).
  - `ShouldUseSameServiceProvider(other)` → **hodnotové** porovnání `ConventionsOptions` (record `==`).
    Kritické: s `AddDbContext` (optionsLifetime = Scoped) vzniká nová instance extension per DI scope;
    referenční porovnání by po 20 scopech skončilo chybou `ManyServiceProvidersCreatedWarning` (od EF Core 3 error).
  - `IsDatabaseProvider` = false, `LogFragment` + `PopulateDebugInfo` (per-flag).

### 2. `UseDefaultHavitConventions` — dva overloady

V `DbContextOptionsExtensions.cs`:

- Stávající `UseDefaultHavitConventions(this DbContextOptionsBuilder)` zachovat (binární kompatibilita) —
  deleguje na nový overload s `new ConventionsOptions()`.
- Nový `UseDefaultHavitConventions(this DbContextOptionsBuilder, ConventionsOptions conventionsOptions)` —
  `((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new HavitConventionsOptionsExtension(conventionsOptions))`.
- XML doc: na DbContextu neodvozeném od Havit DbContextu metoda nemá efekt;
  u compiled models / design-time migrací platí hodnoty z doby sestavení modelu.

### 3. `DbContext` — odstranění `GetConventionOptions`, čtení z merged options

- **Smazat** virtuální metodu `GetConventionOptions()`.
- `ConfigureConventions` získá options takto:

  ```csharp
  ConventionsOptions conventionsOptions =
      this.GetService<IDbContextOptions>().FindExtension<HavitConventionsOptionsExtension>()?.ConventionsOptions
      ?? new ConventionsOptions();
  ```

  Čte se přes `GetService<IDbContextOptions>()` (nikoliv z konstruktoru), aby byly vidět i extensions přidané
  v `OnConfiguring` — touto cestou dodá své hodnoty `BusinessLayerDbContext`. `IDbContextOptions` je v okamžiku
  volání `ConfigureConventions` (model building) už resolvovatelná služba interního service provideru.
- Zbytek `ConfigureConventions` (podmíněné Add/Remove konvencí) beze změny.

### 4. `BusinessLayerDbContext` — vlastní hodnoty přes settings + OnConfiguring

- **Smazat** override `GetConventionOptions()`.
- Do `BusinessLayerDbContextSettings` přidat vlastnost `ConventionsOptions ConventionsOptions` s BL defaultem:

  ```csharp
  new ConventionsOptions
  {
      StringPropertiesDefaultValueConventionEnabled = true,
      LocalizationTableIndexConventionEnabled = false
  }
  ```

- V `OnConfiguring` (kde už BL registruje své convention set pluginy) doplnit:
  `optionsBuilder.UseDefaultHavitConventions(settings.ConventionsOptions);`
  BL hodnoty tím přepíšou případnou registraci z místa registrace DbContextu — konzumenti BL contextu konfigurují
  přes `CreateDbContextSettings()` (stávající BL vzor), zdokumentovat v XML doc.
  Pooling BL contextů se nemění (OnConfiguring přidává extensions už dnes, BL pooling nepředpokládá).

### 5. `EndToEndTestDbContext` (BusinessLayer.Tests)

Override `GetConventionOptions()` přesunout do override `CreateDbContextSettings()` (nastavení `settings.ConventionsOptions`).

### 6. `ConventionsOptions` — ochranný komentář

Record musí zůstat plně value-equatable (žádné delegáty/kolekce) — hodnotová rovnost je nosná pro cachování
interního service provideru (viz bod 1).

### 7. Testy

Do Havit.Data.EntityFrameworkCore.Tests:

- `ShouldUseSameServiceProvider` vrací true pro dvě nezávisle vytvořené extension se shodnými `ConventionsOptions`,
  false pro odlišné.
- `UseDefaultHavitConventions(options)` ovlivní model (např. `StringPropertiesDefaultValueConventionEnabled = true`
  → string property má default value `''`; bez extension → nemá).
- Extension přidaná v `OnConfiguring` se uplatní (cesta pro BusinessLayer).
- Stávající testy (vč. pooled scénáře v Patterns.Tests a BusinessLayer.Tests) musí projít.

## Breaking changes

- Odstranění `protected virtual ConventionsOptions GetConventionOptions()` z `DbContext` — kdo ji overriduje,
  dostane compile error; náhradou je `UseDefaultHavitConventions(conventionsOptions)` při registraci,
  u BL contextů `CreateDbContextSettings()`. Uvést v release notes.

## Zamítnuté alternativy

- **`IConventionSetPlugin`** (vzor BL pluginů) pro 6 core konvencí: běží v jiné fázi sestavení convention setu
  (riziko změny pořadí konvencí, viz komentáře o pořadí v `BusinessLayerDbContext.OnConfiguring`)
  a vyžaduje víc infrastruktury. Hodnotu z options extension lze číst přímo v `ConfigureConventions`.
- **Zachování `GetConventionOptions` jako fallback/kompozice** — není potřeba; odstranění dává jediný způsob konfigurace.
- **Jediná metoda s optional parametrem** místo dvou overloadů: binárně nekompatibilní (`MissingMethodException`
  u assembly zkompilovaných proti staré signatuře).

## Známá omezení (zdokumentovat, neřešit)

- `UseInternalServiceProvider` + dvě registrace téhož typu contextu s různými options → kolize model cache
  (default `IModelCacheKeyFactory` klíčuje typem contextu + designTime).
- U pooled contextu nelze `UseDefaultHavitConventions` volat z `OnConfiguring` (zmrazené options) —
  konfigurovat při registraci. Pro core DbContext se nic nemění; BL contexty pooling nepodporují už dnes.
- Compiled models (`dotnet ef dbcontext optimize`): konvence běží jen při kompilaci modelu,
  runtime hodnoty `ConventionsOptions` se neuplatní.

## Ověření

1. Build: Havit.Data.EntityFrameworkCore, BusinessLayer, Patterns (závislosti na smazané metodě).
2. `dotnet test Havit.Data.EntityFrameworkCore.Tests` — nové + stávající testy.
3. `dotnet test Havit.Data.EntityFrameworkCore.Patterns.Tests` — zejména `ServiceCollectionExternsionsTests`
   (pooled + non-pooled s `UseDefaultHavitConventions`).
4. `dotnet test Havit.Data.EntityFrameworkCore.BusinessLayer.Tests` — EndToEndTestDbContext po přesunu konfigurace do settings.
5. Grep celého repa na `GetConventionOptions` — nesmí zůstat žádný odkaz.
