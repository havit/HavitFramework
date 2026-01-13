## Data Sources

Zprostředkovává přístup k datům jako `IQueryable`. Umožňuje snadné podstrčení dat v testech.

### I*Entity*DataSource, IDataSource<*Entity*>

Poskytuje dvě vlastnosti: `Data` a `DataIncludingDeleted`. Pokud obsahuje třída příznak smazání (soft delete), pak vlastnost `Data` automaticky odfiltruje přínakem smazané záznamy.

Pro každou entitu vzniká jeden interface pojmenovaný `IEntityDataSource` (např. `ILanguageDataSource`).

### *Entity*DbDataSource

* Generované třídy implementující `IEntityDataSource`.
* Pro každou entitu vzniká jedna třída, třídy jsou pojmenované `EntityDbDataSource` (např. `LanguageDbDataSource`).
* K dotazům automaticky přidává [query tag](https://learn.microsoft.com/en-us/ef/core/querying/tags) `IEntityDataSource.Data[IncludingDeleted]`.

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