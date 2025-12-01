HAVIT .NET Framework Extensions - Entity Framework Core Extensions

## EF Core 9 to EF Core 10 Migration Guide

* Žádné zvláštní kroky (mimo oficiální guidelines migrace na .NET 10 a EF Core 10) nejsou potřeba.

## EF Core 8 to EF Core 9 Migration Guide

* Aktualizovat nuget HFW balíčky a Microsoft balíčky na EF Core 9.
* Aktualizovat dotnet tool Havit.Data.EntityFrameworkCore.CodeGenerator.Tool (`dotnet tool update Havit.Data.EntityFrameworkCore.CodeGenerator.Tool`).
* Zkompilovat projekt Entity (build celé solution selže).
* Spustit generátor kódu.
* Upravit Before Commit Processory:
    * V implementacích nahradit IBeforeCommitProcessor<TEntity> za bázovou třídu BeforeCommitProcessor<TEntity>.
	* Použít nové návratové hodnoty dle chování before commit processoru.
	* Neměnit registraci do DI.
* Upravit přetížení metod PerformAddForInsert/Update/Delete pro vlastní Unit Of Work, pokud je potřeba.
* Upravit registraci služeb do dependency injection:
    * odstranit volání WithEntityPatterns,
	* odstranit volání AddEntityPatterns,
	* k volání AddDbContext přidat generický parametr IDbContext,
	* k volání AddDbContext přidat do optionsBuilderu volání UseDefaultHavitConvetions(),
	* nahradit volání AddDataLayer metodou AddDataLayerServices (odstranit argument s assembly),
	* doplnit volání AddDataSeeds, jsou-li použity data seedy.
* Metody AddLocalizationServices, AddLookupServices zůstávají beze změny. 

```csharp
services
	.AddDbContext<IDbContext, MyShopDbContext>(optionsBuilder =>
	{
		string databaseConnectionString = configuration.Configuration.GetConnectionString("Database");
		optionsBuilder.UseSqlServer(databaseConnectionString, c => c.MaxBatchSize(30));
		optionsBuilder.UseDefaultHavitConventions();
	})
	.AddDataLayerServices()
	.AddDataSeeds(typeof(CoreProfile).Assembly)
	.AddLocalizationServices<Language>();
`

