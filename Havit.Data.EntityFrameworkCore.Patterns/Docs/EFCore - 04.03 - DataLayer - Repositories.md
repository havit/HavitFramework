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