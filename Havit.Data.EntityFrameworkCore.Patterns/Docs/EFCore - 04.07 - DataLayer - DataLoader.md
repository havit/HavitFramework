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
