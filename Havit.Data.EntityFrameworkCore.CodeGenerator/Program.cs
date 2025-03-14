﻿using System.Diagnostics;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Microsoft.EntityFrameworkCore.Design;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator;

public static class Program
{
	// This is not a true entry point - it is not a console, but a class library (net x.0).
	// Method is used in CodeGenerator.Tool via reflection!
	// Environment is preconfigured by CodeGenerator.Tool (mainly AppDomain.CurrentDomain.AssemblyResolve).
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
	public static async Task Main(string[] args)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
	{
		Stopwatch stopwatch = Stopwatch.StartNew();

		string solutionDirectory = args[0];
		string entityAssemblyName = args[1];

		IServiceCollection services = new ServiceCollection();

		services.AddSingleton<DbContext>(GetDbContext(entityAssemblyName));
		services.AddSingleton<CodeGeneratorConfiguration>(GetConfiguration(new DirectoryInfo(solutionDirectory)));

		services.AddSingleton<IProjectFactory, ProjectFactory>();
		services.AddSingleton<IModelProject>(sp => sp.GetRequiredService<IProjectFactory>().Create<ModelProject>(Path.Combine(solutionDirectory, sp.GetRequiredService<CodeGeneratorConfiguration>().ModelProjectPath)));
		services.AddSingleton<IMetadataProject>(sp => sp.GetRequiredService<IProjectFactory>().Create<MetadataProject>(Path.Combine(solutionDirectory, sp.GetRequiredService<CodeGeneratorConfiguration>().MetadataProjectPath)));
		services.AddSingleton<IDataLayerProject>(sp => sp.GetRequiredService<IProjectFactory>().Create<DataLayerProject>(Path.Combine(solutionDirectory, sp.GetRequiredService<CodeGeneratorConfiguration>().DataLayerProjectPath)));

		services.AddSingleton<ICodeWriter, CodeWriter>();
		services.AddSingleton<ICodeWriteReporter, CodeWriteReporter>();
		services.AddSingleton<IGenericGenerator, GenericGenerator>();

		services.AddSingleton<IDataLayerGeneratorRunner, DataLayerGeneratorRunner>();
		services.AddSingleton<IDataLayerGenerator, MetadataGenerator>();
		services.AddSingleton<IDataLayerGenerator, DataEntriesGenerator>();
		services.AddSingleton<IDataLayerGenerator, DataSourcesGenerator>();
		services.AddSingleton<IDataLayerGenerator, RepositoriesGenerator>();
		services.AddSingleton<IDataLayerGenerator, DataLayerServiceExtensionsGenerator>();

		services.AddSingleton<IModelErrorsProvider, ModelErrorsProvider>();
		services.AddSingleton<IModelSourceErrorsProvider, RepositoryModelSource>();

		services.AddSingleton<IRelicsCleaner, RelicsCleaner>();

		var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

		var dataLayerGeneratorRunner = serviceProvider.GetRequiredService<IDataLayerGeneratorRunner>();
		var relicsCleaner = serviceProvider.GetRequiredService<IRelicsCleaner>();

		await dataLayerGeneratorRunner.RunAsync(CancellationToken.None);

		await relicsCleaner.CleanRelicsAsync(CancellationToken.None);

		IModelErrorsProvider modelErrorsProvider = serviceProvider.GetRequiredService<IModelErrorsProvider>();
		List<string> errors = modelErrorsProvider.GetErrors();
		if (errors.Count > 0)
		{
			errors.ForEach(Console.Error.WriteLine);
		}

		stopwatch.Stop();
		Console.WriteLine("Completed in {0} ms.", (int)stopwatch.Elapsed.TotalMilliseconds);
	}

	private static DbContext GetDbContext(string entityAssemblyName)
	{
		Console.WriteLine("Initializing DbContext...");
		Assembly assembly = Assembly.Load(new AssemblyName { Name = entityAssemblyName });

		Type[] assemblyTypes = null;
		try
		{
			assemblyTypes = assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException reflectionTypeLoadException)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"There was an error during loading types from {entityAssemblyName}.");
			if (reflectionTypeLoadException.LoaderExceptions.All(exception => exception is FileLoadException))
			{
				reflectionTypeLoadException.LoaderExceptions
					.Cast<FileLoadException>()
					.Select(exception => exception.FileName)
					.Distinct()
					.OrderBy(message => message)
					.ToList()
					.ForEach(message => Console.WriteLine("Cannot load " + message));
			}
			else
			{
				reflectionTypeLoadException.LoaderExceptions
					.Select(exception => exception.Message)
					.Distinct()
					.OrderBy(message => message)
					.ToList()
					.ForEach(message => Console.WriteLine(message));
			}
			Console.ResetColor();
			throw new InvalidOperationException();
		}

		Type dbContextType = assemblyTypes.SingleOrDefault(type => !type.IsAbstract && type.GetInterfaces().Contains(typeof(IDbContext)));
		if (dbContextType == null)
		{
			Console.WriteLine("No IDbContext implementation was found.");
			throw new InvalidOperationException();
		}

		// Pokud nejde zkompilovat následující řádek, velmi pravděpodobně se do CSPROJ při aktualizaci nuget balíčku Microsoft.Data.EntityFrameworkCore.Design dostalo
		// nastavení privateAssets, které je třeba odebrat. Nastavení se tam dostává, neboť .Data.EntityFrameworkCore.Design je zejména toolingový balíček.
		// Přestože to zní příšerně, to doporučeno, abychom si po každé aktualizaci csproj ručně upravili.
		return (DbContext)DbContextActivator.CreateInstance(dbContextType);
	}

	private static CodeGeneratorConfiguration GetConfiguration(DirectoryInfo solutionPath, DirectoryInfo currentPath = null)
	{
		if (currentPath == null)
		{
			Console.WriteLine($"Reading configuration...");
			currentPath = new DirectoryInfo(Environment.CurrentDirectory);
		}

		string configurationFileName = Path.Combine(currentPath.FullName, "efcore.codegenerator.json");
		if (File.Exists(configurationFileName))
		{
			return CodeGeneratorConfiguration.ReadFromFile(configurationFileName);
		}

		if ((currentPath.Parent == null) || (solutionPath.FullName == currentPath.FullName))
		{
			return CodeGeneratorConfiguration.Defaults;
		}

		return GetConfiguration(solutionPath, currentPath.Parent);
	}
}
