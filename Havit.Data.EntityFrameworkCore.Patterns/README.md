HAVIT .NET Framework Extensions - Entity Framework Core Extensions

# EF Core 8 to EF Core 9 Migration Guide

* Aktualizovat nuget HFW balíčky a Microsoft balíčky na EF Core 9.
* Aktualizovat dotnet tool Havit.Data.EntityFrameworkCore.CodeGenerator.Tool (`dotnet tool update Havit.Data.EntityFrameworkCore.CodeGenerator.Tool`).
* Zkompilovat projekt Entity (build celé solution selže).
* Spustit generátor kódu.
* Upravit Before Commit Processory (doplnění nové návratové hodnoty), jsou-li.
* Upravit přetížení metod PerformAddForInsert/Update/Delete pro vlastní Unit Of Work, pokud je potřeba.
* Upravit registraci služeb do dependency injection.
    * Odstranit volání WithEntityPatterns
	* Odstranit volání AddEntityPatterns
	* K volání AddDbContext přidat generický parametr IDbContext
	* K volání AddDbContext přidat do optionsBuilderu volání UseDefaultHavitConvetions()
	* Nahradit volání AddDataLayer metodou AddDataLayerServices (odstranit argument s assembly)
	* Doplnit volání data seedů, jsou-li použity.
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

