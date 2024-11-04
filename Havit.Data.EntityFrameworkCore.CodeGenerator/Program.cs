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

namespace Havit.Data.EntityFrameworkCore.CodeGenerator;

public static class Program
{
	// This is not a true entry point - it is not a console, but a class library (netstandard 2.x).
	// Method is used in CodeGenerator.Tool via reflection!
	// Environment is preconfigured by CodeGenerator.Tool (mainly AppDomain.CurrentDomain.AssemblyResolve).
	public static void Main(string[] args)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();

		string solutionDirectory = args[0];
		string entityAssemblyName = args[1];


		Console.WriteLine($"Reading configuration...");
		CodeGeneratorConfiguration configuration = GetConfiguration(new DirectoryInfo(solutionDirectory));

		IProject modelProject = new ProjectFactory().Create(Path.Combine(solutionDirectory, configuration.ModelProjectPath));
		IProject metadataProject = new ProjectFactory().Create(Path.Combine(solutionDirectory, configuration.MetadataProjectPath));
		IProject dataLayerProject = new ProjectFactory().Create(Path.Combine(solutionDirectory, "DataLayer", "DataLayer.csproj"));

		Console.WriteLine($"Initializing DbContext...");
		if (!TryGetDbContext(entityAssemblyName, out DbContext dbContext))
		{
			return;
		}

		Console.WriteLine($"Generating code...");
		CammelCaseNamingStrategy cammelCaseNamingStrategy = new CammelCaseNamingStrategy();
		var dataEntriesModelSource = new DataEntriesModelSource(dbContext, modelProject, dataLayerProject, cammelCaseNamingStrategy);

		Parallel.Invoke(
			() => GenerateMetadata(metadataProject, modelProject, dbContext, configuration),
			() => GenerateDataSources(dataLayerProject, modelProject, dbContext),
			() => GenerateDataEntries(dataLayerProject, modelProject, dbContext, dataEntriesModelSource),
			() => GenerateRepositories(dataLayerProject, dbContext, modelProject, dataEntriesModelSource),
			() => GenerateDataLayerServiceExtensions(modelProject, dataLayerProject, dbContext)
		);

		string[] unusedDataLayerFiles = null;
		string[] unusedModelFiles = null;

		Parallel.Invoke(
			() =>
			{
				unusedModelFiles = modelProject.GetUnusedGeneratedFiles();
				modelProject.RemoveUnusedGeneratedFiles();
				modelProject.SaveChanges();
			},
			() =>
			{
				unusedDataLayerFiles = dataLayerProject.GetUnusedGeneratedFiles();
				dataLayerProject.RemoveUnusedGeneratedFiles();
				dataLayerProject.SaveChanges();
			});

		unusedModelFiles.Concat(unusedDataLayerFiles).ToList().ForEach(item =>
		{
			try
			{
				File.SetAttributes(item, FileAttributes.Normal);
				File.Delete(item);
			}
			catch
			{
				// NOOP
			}
		});

		stopwatch.Stop();
		Console.WriteLine("Completed in {0} ms.", (int)stopwatch.Elapsed.TotalMilliseconds);
	}

	private static bool TryGetDbContext(string entityAssemblyName, out DbContext dbContext)
	{
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
			dbContext = null;
			return false;
		}

		Type dbContextType = assemblyTypes.SingleOrDefault(type => !type.IsAbstract && type.GetInterfaces().Contains(typeof(IDbContext)));
		if (dbContextType == null)
		{
			Console.WriteLine("No IDbContext implementation was found.");
			dbContext = null;
			return false;
		}

		// Pokud nejde zkompilovat následující řádek, velmi pravděpodobně se do CSPROJ při aktualizaci nuget balíčku Microsoft.Data.EntityFrameworkCore.Design dostalo
		// nastavení privateAssets, které je třeba odebrat. Nastavení se tam dostává, neboť .Data.EntityFrameworkCore.Design je zejména toolingový balíček.
		// Přestože to zní příšerně, to doporučeno, abychom si po každé aktualizaci csproj ručně upravili.
		dbContext = (DbContext)DbContextActivator.CreateInstance(dbContextType);
		return true;
	}

	private static CodeGeneratorConfiguration GetConfiguration(DirectoryInfo solutionPath, DirectoryInfo currentPath = null)
	{
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
		metadataGenerator.Generate();
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
		interfaceDataSourceGenerator.Generate();
		dbDataSourceGenerator.Generate();
		fakeDataSourceGenerator.Generate();
	}

	private static void GenerateDataEntries(IProject dataLayerProject, IProject modelProject, DbContext dbContext, DataEntriesModelSource dataEntriesModelSource)
	{
		CodeWriter codeWriter = new CodeWriter(dataLayerProject);
		var interfaceDataEntriesGenerator = new GenericGenerator<DataEntriesModel>(dataEntriesModelSource, new InterfaceDataEntriesTemplateFactory(), new InterfaceDataEntriesFileNamingService(dataLayerProject), codeWriter);
		var dbDataEntriesGenerator = new GenericGenerator<DataEntriesModel>(dataEntriesModelSource, new DbDataEntriesTemplateFactory(), new DbDataEntriesFileNamingService(dataLayerProject), codeWriter);
		interfaceDataEntriesGenerator.Generate();
		dbDataEntriesGenerator.Generate();
	}

	private static void GenerateRepositories(IProject dataLayerProject, DbContext dbContext, IProject modelProject, DataEntriesModelSource dataEntriesModelSource)
	{
		CodeWriter codeWriter = new CodeWriter(dataLayerProject);
		var dbRepositoryModelSource = new RepositoryModelSource(dbContext, modelProject, dataLayerProject, dataEntriesModelSource);
		var dbRepositoryBaseGeneratedGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryBaseGeneratedTemplateFactory(), new DbRepositoryBaseGeneratedFileNamingService(dataLayerProject), codeWriter);
		var interfaceRepositoryGeneratedGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new InterfaceRepositoryGeneratedTemplateFactory(), new InterfaceRepositoryGeneratedFileNamingService(dataLayerProject), codeWriter);
		var dbRepositoryGeneratedGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryGeneratedTemplateFactory(), new DbRepositoryGeneratedFileNamingService(dataLayerProject), codeWriter);
		var interfaceRepositoryGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new InterfaceRepositoryTemplateFactory(), new InterfaceRepositoryFileNamingService(dataLayerProject), codeWriter, false);
		var dbRepositoryGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryTemplateFactory(), new DbRepositoryFileNamingService(dataLayerProject), codeWriter, false);
		interfaceRepositoryGeneratedGenerator.Generate();
		interfaceRepositoryGenerator.Generate();
		dbRepositoryBaseGeneratedGenerator.Generate();
		dbRepositoryGeneratedGenerator.Generate();
		dbRepositoryGenerator.Generate();
	}

	private static void GenerateDataLayerServiceExtensions(IProject modelProject, IProject dataLayerProject, DbContext dbContext)
	{
		CodeWriter codeWriter = new CodeWriter(dataLayerProject);
		string targetFilename = Path.Combine(dataLayerProject.GetProjectRootPath(), "_generated\\DataLayerServiceExtensions.cs");

		// TODO: model factory
		DbDataSourceModelSource dbDataSourceModelSource = new DbDataSourceModelSource(dbContext, modelProject, dataLayerProject);
		var model = new DataLayerServiceExtensionsModel
		{
			NamespaceName = dataLayerProject.GetProjectRootNamespace(),
			DataSourceModels = dbDataSourceModelSource.GetModels().ToList()
		};
		DataLayerServiceExtensionsTemplate template = new DataLayerServiceExtensionsTemplate(model);
		codeWriter.Save(targetFilename, template.TransformText(), true);
		dataLayerProject.SaveChanges();
	}
}
