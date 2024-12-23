using System.Diagnostics;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Templates;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Microsoft.EntityFrameworkCore.Design;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Configuration;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Model;
using Microsoft.Extensions.Hosting;
using System.CodeDom.Compiler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator;

public static class Program
{
	// This is not a true entry point - it is not a console, but a class library (net x.0).
	// Method is used in CodeGenerator.Tool via reflection!
	// Environment is preconfigured by CodeGenerator.Tool (mainly AppDomain.CurrentDomain.AssemblyResolve).
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
	public static Task Main(string[] args)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
	{
		Stopwatch stopwatch = Stopwatch.StartNew();

		string solutionDirectory = args[0];
		string entityAssemblyName = args[1];

		var hostBuilder = Host.CreateApplicationBuilder();
		hostBuilder.Configuration.AddConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build());
		hostBuilder.Services.AddSingleton<DbContext>(sp => GetDbContext(entityAssemblyName));
		hostBuilder.Services.AddSingleton<CodeGeneratorConfiguration>(sp => GetConfiguration(new DirectoryInfo(solutionDirectory)));
		hostBuilder.Services.AddSingleton<IProjectFactory, ProjectFactory>();
		hostBuilder.Services.AddSingleton<CammelCaseNamingStrategy>();

		hostBuilder.Services.AddKeyedSingleton<IProject>(Project.ModelProjectKey, (sp, serviceKey) => sp.GetRequiredService<IProjectFactory>().Create(Path.Combine(solutionDirectory, sp.GetRequiredService<CodeGeneratorConfiguration>().ModelProjectPath)));
		hostBuilder.Services.AddKeyedSingleton<IProject>(Project.MetadataProjectKey, (sp, serviceKey) => sp.GetRequiredService<IProjectFactory>().Create(Path.Combine(solutionDirectory, sp.GetRequiredService<CodeGeneratorConfiguration>().MetadataProjectPath)));
		hostBuilder.Services.AddKeyedSingleton<IProject>(Project.DataLayerProjectKey, (sp, serviceKey) => sp.GetRequiredService<IProjectFactory>().Create(Path.Combine(solutionDirectory, "DataLayer", "DataLayer.csproj")));

		hostBuilder.Services.AddSingleton<IDataLayerGeneratorRunner, DataLayerGeneratorRunner>();
		hostBuilder.Services.AddSingleton<IDataLayerGenerator, RepositoriesGenerator>();

		var host = hostBuilder.Build();
		//await host.Services.GetRequiredService<IDataLayerGeneratorRunner>().RunAsync();

		Console.WriteLine($"Generating code...");
		return Task.CompletedTask;
		//var dataEntriesModelSource = new DataEntriesModelSource(dbContext, modelProject, dataLayerProject, cammelCaseNamingStrategy);

		//Parallel.Invoke(
		//	() => GenerateMetadata(metadataProject, modelProject, dbContext, configuration),
		//	() => GenerateDataSources(dataLayerProject, modelProject, dbContext),
		//	() => GenerateDataEntries(dataLayerProject, modelProject, dbContext, dataEntriesModelSource),
		//	() => GenerateRepositories(dataLayerProject, dbContext, modelProject, dataEntriesModelSource),
		//	() => GenerateDataLayerServiceExtensions(modelProject, dataLayerProject, dbContext)
		//);

		//string[] unusedDataLayerFiles = null;
		//string[] unusedModelFiles = null;

		//Parallel.Invoke(
		//	() =>
		//	{
		//		unusedModelFiles = modelProject.GetUnusedGeneratedFiles();
		//		modelProject.RemoveUnusedGeneratedFiles();
		//		modelProject.SaveChanges();
		//	},
		//	() =>
		//	{
		//		unusedDataLayerFiles = dataLayerProject.GetUnusedGeneratedFiles();
		//		dataLayerProject.RemoveUnusedGeneratedFiles();
		//		dataLayerProject.SaveChanges();
		//	});

		//unusedModelFiles.Concat(unusedDataLayerFiles).ToList().ForEach(item =>
		//{
		//	try
		//	{
		//		File.SetAttributes(item, FileAttributes.Normal);
		//		File.Delete(item);
		//	}
		//	catch
		//	{
		//		// NOOP
		//	}
		//});

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
		Console.WriteLine($"Reading configuration...");

		if (currentPath == null)
		{
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

	private static void GenerateMetadata(IProject metadataProject, IProject modelProject, DbContext dbContext, CodeGeneratorConfiguration configuration)
	{
		CodeWriter codeWriter = new CodeWriter(metadataProject);
		MetadataClassFileNamingService fileNamingService = new MetadataClassFileNamingService(metadataProject);
		MetadataClassTemplateFactory factory = new MetadataClassTemplateFactory();
		MetadataClassModelSource modelSource = new MetadataClassModelSource(dbContext, metadataProject, modelProject, configuration);
		var metadataGenerator = new GenericGenerator<MetadataClass>(modelSource, factory, fileNamingService, codeWriter);
		//metadataGenerator.Generate();
	}

	private static void GenerateDataSources(IProject dataLayerProject, IProject modelProject, DbContext dbContext)
	{
		CodeWriter codeWriter = new CodeWriter(dataLayerProject);
		IModelSource<InterfaceDataSourceModel> interfaceDataSourceModelSource = new InterfaceDataSourceModelSource(dbContext, modelProject, dataLayerProject);
		IModelSource<DbDataSourceModel> dbDataSourceModelSource = new DbDataSourceModelSource(dbContext, modelProject, dataLayerProject);
		IModelSource<FakeDataSourceModel> fakeDataSourceModelSource = new FakeDataSourceModelSource(dbContext, modelProject, dataLayerProject);
		var interfaceDataSourceGenerator = new GenericGenerator<InterfaceDataSourceModel>(interfaceDataSourceModelSource, new InterfaceDataSourceTemplateFactory(), new InterfaceDataSourceFileNamingService(dataLayerProject), codeWriter);
		var dbDataSourceGenerator = new GenericGenerator<DbDataSourceModel>(dbDataSourceModelSource, new DbDataSourceTemplateFactory(), new DbDataSourceFileNamingService(dataLayerProject), codeWriter);
		var fakeDataSourceGenerator = new GenericGenerator<FakeDataSourceModel>(fakeDataSourceModelSource, new FakeDataSourceTemplateFactory(), new FakeDataSourceFileNamingService(dataLayerProject), codeWriter);
		//interfaceDataSourceGenerator.Generate();
		//dbDataSourceGenerator.Generate();
		//fakeDataSourceGenerator.Generate();
	}

	private static void GenerateDataEntries(IProject dataLayerProject, IProject modelProject, DbContext dbContext, DataEntriesModelSource dataEntriesModelSource)
	{
		CodeWriter codeWriter = new CodeWriter(dataLayerProject);
		var interfaceDataEntriesGenerator = new GenericGenerator<DataEntriesModel>(dataEntriesModelSource, new InterfaceDataEntriesTemplateFactory(), new InterfaceDataEntriesFileNamingService(dataLayerProject), codeWriter);
		var dbDataEntriesGenerator = new GenericGenerator<DataEntriesModel>(dataEntriesModelSource, new DbDataEntriesTemplateFactory(), new DbDataEntriesFileNamingService(dataLayerProject), codeWriter);
		//interfaceDataEntriesGenerator.Generate();
		//dbDataEntriesGenerator.Generate();
	}


	private static void GenerateDataLayerServiceExtensions(IProject modelProject, IProject dataLayerProject, DbContext dbContext)
	{
		CodeWriter codeWriter = new CodeWriter(dataLayerProject);
		string targetFilename = Path.Combine(dataLayerProject.GetProjectRootPath(), "_generated\\DataLayerServiceExtensions.cs");

		// TODO: Lépe pomocí DI? Nebo místo sources rovnou řešit modely?
		DataEntriesModelSource dataEntriesModelSource = new DataEntriesModelSource(dbContext, modelProject, dataLayerProject, new CammelCaseNamingStrategy());
		DbDataSourceModelSource dbDataSourceModelSource = new DbDataSourceModelSource(dbContext, modelProject, dataLayerProject);
		RepositoryModelSource repositoryModelSource = new RepositoryModelSource(dbContext, modelProject, dataLayerProject, dataEntriesModelSource);

		DataLayerServiceExtensionsModelSource modelSource = new DataLayerServiceExtensionsModelSource(dataLayerProject, dataEntriesModelSource, dbDataSourceModelSource, repositoryModelSource);
		DataLayerServiceExtensionsTemplate template = new DataLayerServiceExtensionsTemplate(modelSource.GetModels().Single());
		codeWriter.Save(targetFilename, template.TransformText(), true);
		//dataLayerProject.SaveChanges();
	}
}
