## Cachování

### Jak to funguje
Implementace cachování je realizována na úrovni `Repositories`, `DbDataLoader` a `UnitOfWork`:

* `XyDbRespository.GetObject[Async]` - pokud nemá objekt v identity mapě, pokusí se ho najít v cache, pokud není ani v cache, načítá jej z databáze, poté jej uloží do cache
* `XyDbRespository.GetObjects[Async]`- objekty, které nemá v identity mapě se pokusí najít v cache, objekty, které nejsou ani v cache, načítá z databáze a uloží je do cache
* `XyDbRepository.GetAll[Async]()` - hledá v cache identifikátory objektů
* `DbDataLooader.Load[Async]`, `DbDataLooader.LoadAll[Async]` - při načítání referencí i kolekcí se pokusí najít objekty v cache, objekty, které nejsou v cache, načítá z databáze a uloží je do cache
* `DbUnitOfWork.Commit[Async]` - invaliduje položky v cache
* `XyEntries.Item` - pod pokličkou volá `XyDbRepository.GetObject()`

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
> *Cachování kolekcí však nefunguje tam, kde mohou prvky kolekce přecházet mezi různými parenty*, např. pokud budu přepínat zaměstnanci jeho nadřízeného zaměstnance a tento nadřízený zaměstnanec má kolekci svých podřízených, pak cachování této kolekce bude vykazovat chyby. Pokud má být v tomto scénáři nadřízený zaměstnanec cachován, nesmíme mu zapnout cachování kolekcí. Nesmíme ani použít "cachování všech entit se sliding expirací", jak je uvedeno níže.
>
> (Důvod: Invalidace cache se provádí po uložení změn. Po uložení změn vidíme jen nový, aktuální stav objektů. Nejsme schopni tedy invalidovat cache pro původního nadřízeného zaměstnance, neboť nevíme, kdo to byl.)

### Konfigurace
#### Výchozí konfigurace

Ve výchozí konfiguraci jsou:

* cachovány entity, které označeny atributem Havit.Data.EntityFrameworkCore.Abstractions.Attributes.CacheAttribute (zjednodušeně),
* v atributu lze nastavit prioritu položek v cache, sliding a absolute expiraci,
* v atributu lze zakázat cachování klíčů GetAll.
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

Barva je cachovaná, Auto nikoliv.

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

LoginAccount není cachován, třídy Membership a Role jsou cachovány.

Z cache se proto mohou odbavovat např.:
* `RoleRepository.GetObject(...)`
* `RoleRepository.GetAll()`
* `RoleEntries.Administrator`
* `DataLoader.Load(loginAccount, la => la.Membership).ThenLoad(m => m.Role)`

Cachování Membership je pro daný scénář nutné, ale nemá jiného významu, neboť nemáme pro třídy reprezentující M:N vazbu nepoužíváme repository.

Pokud nebude Membership označen jako cachovaný, nebude se LoginAccountu cachovat kolekce Memberships.

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

Veškeré obejití UnitOfWork:
* např. Cascade Deletes

### Modely s Owned Entity Types
Problémy, které způsobuje použití Owned Entity Types:

* ChangeTracker sleduje změny na owned types samostatně, v pokud má `Person` vlastnost pro domácí adresu `HomeAddress` (owned) typu `Address`, pak při změně (např.) ulice `ChangeTracker` vidí změnu v owned entitě `Address`, nikoliv v `Person`. To ztěžuje invalidace. (Pozn: Ale ukládá to efektivně, takže musí jít nějak rozumně pospojovat entitu a jí použité owned typy).
* Současná implementace uložení `Person` do cache neukládá owned entity types, tj. při odbavení položky z cache nebudou hodnoty pro vlastnosti dobře nastaveny.

