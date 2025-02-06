HAVIT .NET Framework Extensions - Dependency Injection Source Generators

## Účel nuget balíčku
* Knihovna má usnadnit registraci služeb označených attributem `[Service]` do DI containeru.
* Registrace je provedena bez použití reflexe v runtime. Použití reflexe je nahrazeno spuštěním
  kódu vygenerovaného source generátorem v compile time.

## Jak balíček použít

* Do všech projektů v solution, které používají `[Service]` atribut k označení služeb, které mají
 být zaregistrovány do DI containeru, zaregistrujte tento nuget balíček
 `Havit.Extensions.DependencyInjection.SourceGenerators`.
* Source generator vygeneruje extension metodu `Add[ProjectName]ByServiceAttribute`.
* V rámci registrace služeb do DI containeru použijte tuto vygenerovanou metodu (`services.Add[ProjectName]ByServiceAttribute(profileName)`)

## Migrace z předchozích řešení
* Tam, kde používáme předchozí řešení spočívající ve volání extension metody `AddByServiceAttribute`, musíme odebrat nuget balíček `Havit.Extensions.DependencyInjection` ze všech projektů v solution
  (vč. případného pozůstatku v `Directory.Packages.props`). Typicky jde o jeden projekt v solution.
* Odstranit volání extension metody `AddByServiceAttribute(...)`, která používá reflexi, resp. nahradit toto volání postupem popsaným výše.
