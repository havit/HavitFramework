# Dependency Injection

Registrace do DI containeru je podporována pro `IServiceCollection`.
Pro registrace služeb se generuje extension metoda `DataLayerServiceExtensions.AddDataLayerServices`.

```csharp
services
			.AddDbContext<IDbContext, GoranG3DbContext>(optionsBuilder =>
			{
				if (configuration.UseInMemoryDb)
				{
					optionsBuilder.UseInMemoryDatabase(nameof(GoranG3DbContext));
				}
				else
				{
					optionsBuilder.UseSqlServer(configuration.DatabaseConnectionString, c =>
					{
						c.MaxBatchSize(30);
						c.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
					});
				}
				optionsBuilder.UseDefaultHavitConventions();
			})
			.AddLocalizationServices<Language>() // volitelné
			.AddDataLayerServices()
			.AddDataSeeds(typeof(CoreProfile).Assembly)
			.AddLookupService<ICountryByIsoCodeLookupService, CountryByIsoCodeLookupService>();

		services.AddSingleton<IEntityValidator<object>, ValidatableObjectEntityValidator>(); // pokud je požadována validace entit pomocí IValidatableObject

		services.AddSingleton<ITimeService, ApplicationTimeService>();
		services.AddSingleton<TimeProvider, ApplicationTimeProvider>();
		services.AddSingleton<ICacheService, MemoryCacheService>();
		services.AddSingleton(new MemoryCacheServiceOptions { UseCacheDependenciesSupport = false });

```
