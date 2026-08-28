HAVIT .NET Framework Extensions - Entity Framework Core Data Patterns - Analyzers

## Účel nuget balíčku
* Balíček obsahuje Roslyn analyzery, které v compile time hlásí chybná použití API knihovny
  `Havit.Data.EntityFrameworkCore.Patterns` (resp. `Havit.Data.Patterns`).
* Jde o vývojovou závislost (`DevelopmentDependency`), do runtime se nic nepřenáší.

## Jak balíček použít
* Zaregistrujte nuget balíček `Havit.Data.EntityFrameworkCore.Patterns.Analyzers` do projektů,
  ve kterých se pracuje s `IUnitOfWork`, s modelem a s dotazy do databáze
  (typicky `Model`, `DataLayer`, `Services`, `Facades`).
* Jednotlivá pravidla lze standardně konfigurovat v `.editorconfig`
  (např. `dotnet_diagnostic.HFW1004.severity = error`).

## Pravidla

| ID | Pravidlo |
| --- | --- |
| HFW1002 | `IEnumerable<T>` předaný do `IUnitOfWork.AddFor*` metody, která očekává jednu entitu. |
| HFW1003 | Vnořená kolekce (`IEnumerable<IEnumerable<T>>`) předaná do `IUnitOfWork.AddRangeFor*` metody. |
| HFW1004 | `FilteringCollection<T>` použitá v expression tree, tedy v dotazu do databáze. Kolekce je in-memory wrapper nad namapovanou kolekcí, EF Core ji nepřeloží do SQL: dotaz buď spadne za běhu, nebo (ve finální projekci) tiše nevrátí žádná data. Řešením je použít namapovanou kolekci `XIncludingDeleted` s explicitním filtrem. Pokrývá method syntax i query syntax. Načítání přes `IDataLoader`/`IFluentDataLoader` hlášeno není - data loader `FilteringCollection<T>` podporuje. |
