## LookupServices

Jde o třídy, které mají zajistit možnost rychlého vyhledávání entity podle klíče. Na rozdíl od ostatních (Repository, DataSources) nejsou generované - píšeme je ručně, pro jejich napsání je však připravena silná podpora.

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

Metoda `GetUzivatelByEmail` je pak službou třídy samotné, kterou mohou její konzumenti používat. Pod pokličkou jen volá metody GetEntityByLookupKey.

#### IncludeDeleted
By default nejsou uvažovány (a vraceny) příznakem smazané záznamy. Pokud mají být použity, je třeba provést override vlastosti IncludeDeleted a vrátit true.

#### Filter
Pokud nás zajímají jen nějaké instance třídy (neprázdný párovací klíč, objekty v určitém stavu, atp.), lze volitelně provést override vlastnosti `Filter` a vrátit podmínku, kterou musí objekty splňovat.

#### ThrowExceptionWhenNotFound
Pokud# není podle klíče objekt nalezen, je vyhozena výjimka `ObjectNotFoundException`. Pokud nemá být vyhozena výjimka a má být vrácena hodnota null, lze provést override této vlastnosti, aby vracela `false`.

### OptimizationHints
Pro efektivnější fungování invalidací (viz níže) je možné zadat určité hinty, např., pokud je entita readonly a tedy nemůže být za běhu aplikace změněna, nemusí k žádné invalidaci docházet.

### Dependency Injection
Třídy je nutno do DI containaru instalovat nejen pod sebe sama, ale ještě pod servisní interface, který zajistí možnost invalidace dat při uložení nějaké entity (viz dále).

Není tak možné pro lookup service použít automatickou registraci pomocí attributu [Service].

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
* Došlo k úpravě dat mimo UnitOfWork (třeba stored procedurou) a potřebujeme dát lookup službě vědět, že lookup data již nejsou aktuální.

### Použití repository
Objekty jsou po nalezení klíče v lookup datech vyzvednuty z repository. Přínos tohoto chování je takový, že získaný objekt je trackovaný a mohl být získán z cache, bez dotazu do databáze.

Pozor na scénáře, kde se ptáme do lookup služby opakovaně pro necachované objekty (nebo cachované, které ještě v cache nejsou), každé volání pak může udělat dotaz do databáze právě pro získání instance z repository.

Pro tuto situaci je k dispozici metoda, která nevrátí instanci entity, ale jen její klíč - GetEntityKeyByLookupKey. Je pak možno implementačně získat klíče všech objektů, které můžeme ve vlastním kódu přehodit metodě GetObjects repository. Pokud máme problém poté objekty roztřídit znovu dle klíčů, můžeme uvažovat takto:

1. Nejprve získáme všechny klíče entit dle vyhledávané vlastnosti
2. Poté všechny entity načteme pomocí repository.GetObjects(…), čímž dostaneme objekty do paměti (identity mapy, DbContext).
3. Nyní se můžeme do lookup služby (a metody GetEntityByLookupKey) ptát jeden objekt po druhým, vracení objektů z repository již nebude dělat dotazy do databáze, neboť jsou již načteny.

### Použití non-Int32 primárního klíče
K dispozici je bázová třída LookupServiceBase<TLookupKey, TEntity, TEntityKey>, kde jako TEntityKey je třeba zvolit skutečný typ primárního klíče.

#### Použití složeného klíče
Pro vyhledávání je možno použít složený klíč, klíč musí mít vlastní třídu, která musí zajistit fungování porovnání v Dictionary, tedy předefinovat porovnání. S úspěchem lze použít v implementaci anonymní třídu, byť se trochu zhorší kvalita kódu tím, že jako typ klíče musíme uvést typ object.

#### Použití více entit pod jedním klíčem
Není podporováno, je vyhozena výjimka.

### Časová složitost
Snažíme se, aby složitost vyhledání byla O(1).

Konstantní složitost samozřejmě neplatí pro první volání, které sestavuje vyhledávací slovník.

### Implementační detail
Sestavení lookup dat provede jediný dotaz do databáze pro všechny objekty (s ohledem na `IncludeDeleted` a `Filter`). Nenačítají se celé instance entit, ale jen jejich projekce, tj. vrací se netrackované objekty, tj. nenaplní se identity mapa (`DbContext`) instancemi entit, ve kterých je vyhledáváno.