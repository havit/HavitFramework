using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Diagnostics.Contracts;
using Havit.EFCoreTests.DataLayer.DataSources;
using Havit.EFCoreTests.DataLayer.Lookups;
using Havit.EFCoreTests.DataLayer.Repositories;
using Havit.EFCoreTests.Model;
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
		services.WithEntityPatternsInstaller()
			.AddDataLayer(typeof(IPersonRepository).Assembly)
			.AddDbContext<Havit.EFCoreTests.Entity.ApplicationDbContext>(optionsBuilder =>
				optionsBuilder
					.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=EFCoreTests;Application Name=EFCoreTests-Entity;ConnectRetryCount=0")
					.EnableSensitiveDataLogging(true))
			//.UseInMemoryDatabase("ConsoleApp")
			.AddEntityPatterns()
			.AddLookupService<IUserLookupService, UserLookupService>();

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
		//await scope.ServiceProvider.GetRequiredService<IDataSeedRunner>().SeedDataAsync<PersonsProfile>(forceRun: true, cancellationToken);

		for (int i = 0; i < 50000; i++)
		{
			using var scope = serviceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
			Person person = new Person();
			person.Subordinates.AddRange(Enumerable.Range(0, 2).Select(i => new Person()));
			uow.AddForInsert(person);
			await uow.CommitAsync(cancellationToken);
		}
	}


	private static async Task DebugAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
	{
		using var scope = serviceProvider.CreateScope();
		var personRepository = scope.ServiceProvider.GetRequiredService<IPersonRepository>();
		var personDataSource = scope.ServiceProvider.GetRequiredService<IPersonDataSource>();
		var dataLoader = scope.ServiceProvider.GetRequiredService<IDataLoader>();

		// scénář 1: načítání kolekcí
		//Person person = await personRepository.GetObjectAsync(1, cancellationToken);
		//Contract.Assert(person.BossId == null);

		//Stopwatch sw = Stopwatch.StartNew();
		//dataLoader.Load(person, p => p.Subordinates).ThenLoad(p => p.Subordinates);
		//await dataLoader.LoadAsync(person, p => p.Subordinates, cancellationToken).ThenLoadAsync(p => p.Subordinates, cancellationToken);
		//sw.Stop();

		// scénář 2: načítání referencí
		//List<Person> persons = personDataSource.DataIncludingDeleted.Where(p => (p.BossId != null) && (p.BossId != 1)).ToList();
		//Stopwatch sw = Stopwatch.StartNew();
		////dataLoader.LoadAll(persons, p => p.Boss);
		//await dataLoader.LoadAllAsync(persons, p => p.Boss, cancellationToken);
		//sw.Stop();

		// scénář 3: XyRepository.GetObjects()
		//await personRepository.GetObjectsAsync(new int[] { 1, 3 }, cancellationToken);

		//Stopwatch sw = Stopwatch.StartNew();
		////List<Person> persons = personRepository.GetObjects(Enumerable.Range(1, 50000).ToArray());
		//List<Person> persons = await personRepository.GetObjectsAsync(Enumerable.Range(1, 100000).Where(int.IsEvenInteger).ToArray(), cancellationToken);
		//sw.Stop();

		var person = personRepository.GetObject(1);
		await dataLoader.LoadAsync(person, p => p.Subordinates, cancellationToken);

		//Console.WriteLine("  " + sw.ElapsedMilliseconds + " ms");
	}

}
