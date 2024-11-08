using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Diagnostics.Contracts;
using Havit.EFCoreTests.DataLayer;
using Havit.EFCoreTests.DataLayer.DataSources;
using Havit.EFCoreTests.DataLayer.Lookups;
using Havit.EFCoreTests.DataLayer.Repositories;
using Havit.EFCoreTests.DataLayer.Seeds.Persons;
using Havit.EFCoreTests.Model;
using Havit.Linq.Expressions;
using Havit.Services.Caching;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp1;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var host = Host.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration(configurationBuilder =>
				configurationBuilder.AddJsonFile("appsettings.ConsoleApp.json", optional: false)
			)
			.ConfigureLogging((hostingContext, logging) => logging
			.AddSimpleConsole(config => config.TimestampFormat = "[hh:MM:ss.ffff] "))
			.ConfigureServices((hostingContext, services) => ConfigureServices(hostingContext, services))
			.Build();

		//await UpdateDatabaseAsync(host.Services, CancellationToken.None);
		//await SeedDatabaseAsync(host.Services, CancellationToken.None);
		await DebugAsync(host.Services);
	}

	private static void ConfigureServices(HostBuilderContext hostingContext, IServiceCollection services)
	{
		services.AddDbContext<IDbContext, Havit.EFCoreTests.Entity.ApplicationDbContext>(optionsBuilder =>
				optionsBuilder
					.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreTests;Application Name=EFCoreTests-Entity;ConnectRetryCount=0")
					.UseDefaultHavitConventions()
					.EnableSensitiveDataLogging(true));

		services
			.AddDataLayerServices()
			.AddLookupService<IUserLookupService, UserLookupService>()
			.AddDataSeeds(typeof(Havit.EFCoreTests.DataLayer.Seeds.Persons.PersonsProfile).Assembly);

		services.AddSingleton<ITimeService, ServerTimeService>();
		services.AddSingleton<ICacheService, MemoryCacheService>();
		services.AddSingleton<IOptions<MemoryCacheOptions>, OptionsManager<MemoryCacheOptions>>();
		services.AddSingleton(typeof(IOptionsFactory<MemoryCacheOptions>), new OptionsFactory<MemoryCacheOptions>(Enumerable.Empty<IConfigureOptions<MemoryCacheOptions>>(), Enumerable.Empty<IPostConfigureOptions<MemoryCacheOptions>>()));
		services.AddSingleton<IMemoryCache, MemoryCache>();
	}

	private static async Task UpdateDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
	{
		using var scope = serviceProvider.CreateScope();
		await scope.ServiceProvider.GetRequiredService<IDbContext>().Database.EnsureDeletedAsync(cancellationToken);
		await scope.ServiceProvider.GetRequiredService<IDbContext>().Database.MigrateAsync(cancellationToken);
	}

	private static async Task SeedDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
	{
		await serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IDataSeedRunner>().SeedDataAsync<PersonsProfile>(forceRun: true, cancellationToken: cancellationToken);
		//serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IDataSeedRunner>().SeedData<PersonsProfile>(forceRun: true);

		//for (int i = 0; i < 50000; i++)
		//{
		//	using var scope = serviceProvider.CreateScope();
		//	var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
		//	Person person = new Person();
		//	person.Subordinates.AddRange(Enumerable.Range(0, 2).Select(i => new Person()));
		//	uow.AddForInsert(person);
		//	await uow.CommitAsync(cancellationToken);
		//}
	}


	private static async Task DebugAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
	{
		using var scope = serviceProvider.CreateScope();
		var personRepository = scope.ServiceProvider.GetRequiredService<IPersonRepository>();
		var personDataSource = scope.ServiceProvider.GetRequiredService<IPersonDataSource>();
		var dataLoader = scope.ServiceProvider.GetRequiredService<IDataLoader>();

		Person person1 = personRepository.GetObject(1);
		dataLoader.Load(person1, p => p.Subordinates).ThenLoad(p => p.Subordinates);

		Person person2 = personRepository.GetObject(2);
		await dataLoader.LoadAsync(person2, p => p.Subordinates, cancellationToken).ThenLoadAsync(p => p.Subordinates, cancellationToken);

		//	var parameter = Expression.Parameter(typeof(Person), "item");
		//	Expression<Func<Person, IComparable>> expression = item => (IComparable)item.Name;
		//	var expression2 = Expression.Lambda<Func<Person, IComparable>>(expression.Body.RemoveConvert(), expression.Parameters[0]);

		//	// scénář 1: načítání kolekcí
		//	Person person1 = personRepository.GetObject(1);
		//	Person person2 = await personRepository.GetObjectAsync(4, cancellationToken);
		//	Contract.Assert(person1.BossId == null);
		//	Contract.Assert(person2.BossId == null);

		//	dataLoader.Load(person1, p => p.Subordinates).ThenLoad(p => p.Subordinates);
		//	await dataLoader.LoadAsync(person2, p => p.Subordinates, cancellationToken).ThenLoadAsync(p => p.Subordinates, cancellationToken);

		//	// scénář 2: načítání referencí
		//	List<Person> persons1 = personRepository.GetObjects(Enumerable.Range(1, 50000).ToArray());
		//	List<Person> persons2 = await personRepository.GetObjectsAsync(Enumerable.Range(50000, 100000).Where(int.IsEvenInteger).ToArray(), cancellationToken);
		//	dataLoader.LoadAll(persons1, p => p.Boss);
		//	await dataLoader.LoadAllAsync(persons2, p => p.Boss, cancellationToken);

		//	// scénář 3: XyRepository.GetObjects()
		//	personRepository.GetObjects(3, 4);
		//	await personRepository.GetObjectsAsync([5, 6], cancellationToken);
	}

}