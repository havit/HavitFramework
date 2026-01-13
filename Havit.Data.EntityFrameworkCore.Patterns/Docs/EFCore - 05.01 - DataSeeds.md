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



