using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services.SourceControl;
using Microsoft.EntityFrameworkCore.Design;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator
{
	public static class Program
	{
		// This is not a true entry point - it is not a console, but a class library (netstandard 2.x).
		// Method is used in CodeGenerator.Tool via reflection!
		// Environment is preconfigured by CodeGenerator.Tool (mainly AppDomain.CurrentDomain.AssemblyResolve).
		public static void Main(string[] args)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			IProject modelProject = null;
			IProject dataLayerProject = null;
			DbContext dbContext = null;
			ISourceControlClient sourceControlClient = null;

			string solutionDirectory = args[0];
			string entityAssemblyName = args[1];

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
				return;
			}

			Type dbContextType = assemblyTypes.SingleOrDefault(type => !type.IsAbstract && type.GetInterfaces().Contains(typeof(IDbContext)));
			if (dbContextType == null)
			{
				Console.WriteLine("No IDbContext implementation was found.");
				return;
			}

			Parallel.Invoke(
				() => sourceControlClient = new NullSourceControlClient(), //new TfsSourceControlClientFactory().Create(solutionDirectory.FullName),
				() =>
				{
					dbContext = (DbContext)DbContextActivator.CreateInstance(dbContextType);
				},
				() => modelProject = new ProjectFactory().Create(Path.Combine(solutionDirectory, "Model", "Model.csproj")),
				() => dataLayerProject = new ProjectFactory().Create(Path.Combine(solutionDirectory, "DataLayer", "DataLayer.csproj"))
			);

			Console.WriteLine($"Initializing DbContext...");
			CammelCaseNamingStrategy cammelCaseNamingStrategy = new CammelCaseNamingStrategy();

			Console.WriteLine($"Generating code...");
			
			var dataEntriesModelSource = new DataEntriesModelSource(dbContext, modelProject, dataLayerProject, cammelCaseNamingStrategy);

			Parallel.Invoke(
				() => GenerateMetadata(modelProject, dbContext, sourceControlClient),
				() => GenerateDataSources(dataLayerProject, sourceControlClient, modelProject, dbContext),
				() => GenerateDataEntries(dataLayerProject, sourceControlClient, modelProject, dbContext, dataEntriesModelSource),
				() => GenerateRepositories(dataLayerProject, sourceControlClient, dbContext, modelProject, dataEntriesModelSource)/*,
				() => GenerateQueryExtensions(dataLayerProject, sourceControlClient, registeredEntityEnumerator)*/
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

			sourceControlClient.Delete(unusedModelFiles.Concat(unusedDataLayerFiles).ToArray());
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

		private static void GenerateMetadata(IProject modelProject, DbContext dbContext, ISourceControlClient sourceControlClient)
		{
			CodeWriter codeWriter = new CodeWriter(modelProject, sourceControlClient);
			MetadataClassFileNamingService fileNamingService = new MetadataClassFileNamingService(modelProject);
			MetadataClassTemplateFactory factory = new MetadataClassTemplateFactory();
			MetadataClassModelSource modelSource = new MetadataClassModelSource(dbContext, modelProject);
			var metadataGenerator = new GenericGenerator<MetadataClass>(modelSource, factory, fileNamingService, codeWriter);
			metadataGenerator.Generate();
		}

		private static void GenerateDataSources(IProject dataLayerProject, ISourceControlClient sourceControlClient, IProject modelProject, DbContext dbContext)
		{
			CodeWriter codeWriter = new CodeWriter(dataLayerProject, sourceControlClient);
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

		private static void GenerateDataEntries(IProject dataLayerProject, ISourceControlClient sourceControlClient, IProject modelProject, DbContext dbContext, DataEntriesModelSource dataEntriesModelSource)
		{
			CodeWriter codeWriter = new CodeWriter(dataLayerProject, sourceControlClient);
			var interfaceDataEntriesGenerator = new GenericGenerator<DataEntriesModel>(dataEntriesModelSource, new InterfaceDataEntriesTemplateFactory(), new InterfaceDataEntriesFileNamingService(dataLayerProject), codeWriter);
			var dbDataEntriesGenerator = new GenericGenerator<DataEntriesModel>(dataEntriesModelSource, new DbDataEntriesTemplateFactory(), new DbDataEntriesFileNamingService(dataLayerProject), codeWriter);
			interfaceDataEntriesGenerator.Generate();
			dbDataEntriesGenerator.Generate();
		}

		private static void GenerateRepositories(IProject dataLayerProject, ISourceControlClient sourceControlClient, DbContext dbContext, IProject modelProject, DataEntriesModelSource dataEntriesModelSource)
		{
			CodeWriter codeWriter = new CodeWriter(dataLayerProject, sourceControlClient);
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
	}
}
