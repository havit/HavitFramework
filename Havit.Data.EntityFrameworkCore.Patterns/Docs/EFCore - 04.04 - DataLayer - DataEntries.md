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
