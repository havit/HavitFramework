# Entity Framework Core – Model

## Úvod
Konvence datového modelu a výchozí chování jsou velmi dobře popsány v oficiální dokumentaci EF Core, proto zde není smysluplné dokumentaci opakovat.
Viz https://docs.microsoft.com/en-us/ef/core/modeling/

Pojmenování tříd a modelu je v angličtině ev. v primárním jazyce projektu.

## Primární klíč

Používáme primární klíč typu int pojmenovaný Id.
Primátní klíč může být i jiného typu (celočíselný `SByte`, `Int16`, `Int64`, `Byte`, `UInt16`, `UInt32`, `UInt64`, dále `string` nebo `Guid`),
podpora těchto typů zatím není kompletní (chybí minimálně podpora `IDataLoader`).

```csharp
public int Id { get; set; }
```

Přítomnost a pojmenování primárního klíče je kontrolována unit testem.

```csharp
public int Id { get; set; }
```

## Délky stringů

U všech vlastností typu `string` je nutno uvést jejich maximální délku pomocí attributu `[MaxLength]`.
Pokud nemá být délka omezená, atributu nezadáváme hodnotu nebo použijeme hodnotu `Int32.MaxValue`.

Ze zadaných hodnot jsou vygenerována metadata, např. pro snadné omezení maximální délky textu v UI.

```csharp
[MaxLength(128)]
public string PasswordHash { get; set; }

[MaxLength(8)]
public string PasswordSalt { get; set; }

...

[MaxLength] // pro maximální možnou délku
public string Note { get; set; }
```

## Výchozí hodnoty vlastností

Výchozí hodnoty vlastností definujeme přímo v kódu:

```csharp
public bool IsActive { get; set; } = true;
```

## Reference / cizí klíče

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

## Kolekce One-To-Many (1:N)

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

## Kolekce Many-To-Many (M:N)
Entity Framework Core 5.x přináší podporu pro vazby typu M:N (viz dokumentace), avšak HFW pro práci s kolekcemi nemá podporu.

Vazby M:N doporučujeme **dekomponovat na dvě vazby 1:N** (postup známý z EF Core 2.x a 3.x).
EF Core Ve výchozím chování EF Core je třeba této entitě nakonfigurovat složený primární klíč (pomocí data anotations nelze definovat složený primární klíč), nám se klíč nastaví sám (pokud není ručně nastaven) konvencí. Pokud je to třeba, nastavíme pouze název databázové tabulky, do které je entita mapována.

### Příklad
Pokud má mít `User` kolekci `Roles`, musíme zavést entity `Membership` se dvěma vlastnostmi. `User` pak bude mít kolekci nikoliv rolí, ale těchto `Membershipů`.

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

## Kolekce s filtrováním smazaných záznamů

Viz Entity Framework Core – Kolekce s filtrováním smazaných záznamů

## Mazání příznakem (Soft Delete)

Podpora mazání příznakem je na objektech, které obsahují vlastnost `Deleted` typu `Nullable<DateTime>`. Podpora není implementovatelná na dočítání kolekcí modelových objektů, tj. **při načítání kolekcí objektů jsou načítány i smazané objekty**.

```csharp
public DateTime? Deleted { get; set; }
```

## Lokalizace

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

## Entries / systémové záznamy (EnumClass)
Pokud má třída sloužit jako systémový číselník se známými hodnotami, použijeme vnořený veřejný enum `Entry` s hodnotami.
Pokud mají mít záznamy v databázi stejné Id, což je obvyklé, je třeba uvést položkám hodnotu.

Na základě tohoto enumu pak generátor zakládá DataEntries.


### Příklad

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
