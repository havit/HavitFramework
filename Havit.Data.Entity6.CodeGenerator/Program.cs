using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Havit.Data.Entity.CodeGenerator.Actions.DataEntries;
using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Template;
using Havit.Data.Entity.CodeGenerator.Actions.DataSources;
using Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.Entity.CodeGenerator.Actions.DataSources.Template;
using Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses;
using Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses.Model;
using Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses.Template;
using Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions;
using Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Model;
using Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Template;
using Havit.Data.Entity.CodeGenerator.Actions.Repositories;
using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Templates;
using Havit.Data.Entity.CodeGenerator.Entity;
using Havit.Data.Entity.CodeGenerator.Services;
using Havit.Data.Entity.Patterns;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Services.TimeServices;

namespace Havit.Data.Entity.CodeGenerator
{
	internal static class Program
	{
		internal static void Main(string[] args)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Project modelProject = null;
			Project dataLayerProject = null;
			DbContext dbContext = null;
			SourceControlClient sourceControlClient = null;

			DirectoryInfo solutionDirectory = new DirectoryInfo(Environment.CurrentDirectory);// @"D:\Dev\002.HFW-NewProjectTemplate";
			
			while (System.IO.Directory.GetFiles(solutionDirectory.FullName, "*.sln", SearchOption.TopDirectoryOnly).Length == 0)
			{
				if (solutionDirectory.Root.FullName == solutionDirectory.FullName)
				{
					throw new InvalidOperationException("Solution file not found.");
				}
				solutionDirectory = solutionDirectory.Parent;
			}

			string[] files = System.IO.Directory.GetFiles(Path.Combine(solutionDirectory.FullName, @"Entity\bin"), "*.Entity.dll", SearchOption.AllDirectories);
			if (files.Length == 0)
			{
				Console.WriteLine("Nebyla nalezena assembly pro Entity.");
				return;
			}

			string file = files.Where(item => !item.EndsWith("Havit.Entity.dll")).OrderByDescending(item => System.IO.File.GetLastAccessTime(item)).First();
			Console.WriteLine(file);

			Assembly assembly = Assembly.LoadFrom(file);
			Type dbContextType = assembly.GetTypes().FirstOrDefault(type => !type.IsAbstract && type.GetInterfaces().Contains(typeof(Havit.Data.Entity.IDbContext)));
			if (dbContextType == null)
			{
				Console.WriteLine("Nebyla nalezena třída implementující IDbContext.");
				return;
			}

			Parallel.Invoke(
				() => modelProject = new Project(Path.Combine(solutionDirectory.FullName, @"Model\Model.csproj")),
				() => dataLayerProject = new Project(Path.Combine(solutionDirectory.FullName, @"DataLayer\DataLayer.csproj")),
				() => sourceControlClient = SourceControlClient.GetByFolder(solutionDirectory.FullName),
				() =>
				{
					dbContext = (DbContext)Activator.CreateInstance(dbContextType);
					
					// doNothingInitializer = new DoNothingInitializer<MyDbContext>();
					Type doNothingInitializerType = typeof(DoNothingInitializer<>).MakeGenericType(dbContextType);
					object doNothingInitializer = Activator.CreateInstance(doNothingInitializerType);

					// System.Data.Entity.Database.SetInitializer<MyDbContext>(doNothingInitializer);
					MethodInfo setInitializerMethod = typeof(System.Data.Entity.Database).GetMethod("SetInitializer", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(dbContextType);
					setInitializerMethod.Invoke(null, new object[] { doNothingInitializer });
				});

			ObjectContext objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
			RegisteredEntityEnumerator registeredEntityEnumerator = new RegisteredEntityEnumerator(objectContext);
            var dataEntriesModelSource = new DataEntriesModelSource(registeredEntityEnumerator, modelProject, dataLayerProject);

		    Parallel.Invoke(
		        () => GenerateMetadata(modelProject, registeredEntityEnumerator, sourceControlClient),
		        () => GenerateDataSources(dataLayerProject, sourceControlClient, objectContext, modelProject, registeredEntityEnumerator),
		        () => GenerateDataEntries(dataLayerProject, sourceControlClient, objectContext, modelProject, registeredEntityEnumerator, dataEntriesModelSource),
		        () => GenerateRepositories(dataLayerProject, sourceControlClient, registeredEntityEnumerator, modelProject, dataEntriesModelSource)/*,
				() => GenerateQueryExtensions(dataLayerProject, sourceControlClient, registeredEntityEnumerator)*/
			);

		    Parallel.Invoke(
		        () =>
		        {
		            modelProject.RemoveUnusedGeneratedFiles();
		            modelProject.SaveChanges();
		        },
		        () =>
		        {
		            dataLayerProject.RemoveUnusedGeneratedFiles();
		            dataLayerProject.SaveChanges();
		        });

			stopwatch.Stop();
			Console.WriteLine("Completed in {0} ms.", (int)stopwatch.Elapsed.TotalMilliseconds);
		}

		private static void GenerateMetadata(Project modelProject, RegisteredEntityEnumerator registeredEntityEnumerator, SourceControlClient sourceControlClient)
		{
			 CodeWriter codeWriter = new CodeWriter(modelProject, sourceControlClient);
			MetadataClassFileNamingService fileNamingService = new MetadataClassFileNamingService(modelProject);
			MetadataClassTemplateFactory factory = new MetadataClassTemplateFactory();
			MetadataClassModelSource modelSource = new MetadataClassModelSource(registeredEntityEnumerator, modelProject);
			var metadataGenerator = new GenericGenerator<MetadataClass>(modelSource, factory, fileNamingService, codeWriter);
			metadataGenerator.Generate();
		}

		private static void GenerateDataSources(Project dataLayerProject, SourceControlClient sourceControlClient, ObjectContext objectContext, Project modelProject, RegisteredEntityEnumerator registeredEntityEnumerator)
		{
			CodeWriter codeWriter = new CodeWriter(dataLayerProject, sourceControlClient);
			//IFileNamingService<InterfaceDataSourceModel> namingService = new InterfaceDataSourceFileNamingService(dataLayerProject);
			SoftDeleteManager softDeleteManager = new SoftDeleteManager(new ServerTimeService());
			IModelSource<InterfaceDataSourceModel> interfaceDataSourceModelSource = new InterfaceDataSourceModelSource(registeredEntityEnumerator, modelProject, dataLayerProject, softDeleteManager);
			IModelSource<DbDataSourceModel> dbDataSourceModelSource = new DbDataSourceModelSource(registeredEntityEnumerator, modelProject, dataLayerProject);
			IModelSource<FakeDataSourceModel> fakeDataSourceModelSource = new FakeDataSourceModelSource(registeredEntityEnumerator, modelProject, dataLayerProject);
			var interfaceDataSourceGenerator = new GenericGenerator<InterfaceDataSourceModel>(interfaceDataSourceModelSource, new InterfaceDataSourceTemplateFactory(), new InterfaceDataSourceFileNamingService(dataLayerProject), codeWriter);
			var dbDataSourceGenerator = new GenericGenerator<DbDataSourceModel>(dbDataSourceModelSource, new DbDataSourceTemplateFactory(), new DbDataSourceFileNamingService(dataLayerProject), codeWriter);
			var fakeDataSourceGenerator = new GenericGenerator<FakeDataSourceModel>(fakeDataSourceModelSource, new FakeDataSourceTemplateFactory(), new FakeDataSourceFileNamingService(dataLayerProject), codeWriter);
			interfaceDataSourceGenerator.Generate();
			dbDataSourceGenerator.Generate();
			fakeDataSourceGenerator.Generate();
		}

		private static void GenerateDataEntries(Project dataLayerProject, SourceControlClient sourceControlClient, ObjectContext objectContext, Project modelProject, RegisteredEntityEnumerator registeredEntityEnumerator, DataEntriesModelSource dataEntriesModelSource)
		{
			CodeWriter codeWriter = new CodeWriter(dataLayerProject, sourceControlClient);
			var interfaceDataEntriesGenerator = new GenericGenerator<DataEntriesModel>(dataEntriesModelSource, new InterfaceDataEntriesTemplateFactory(), new InterfaceDataEntriesFileNamingService(dataLayerProject), codeWriter);
			var dbDataEntriesGenerator = new GenericGenerator<DataEntriesModel>(dataEntriesModelSource, new DbDataEntriesTemplateFactory(), new DbDataEntriesFileNamingService(dataLayerProject), codeWriter);
			interfaceDataEntriesGenerator.Generate();
			dbDataEntriesGenerator.Generate();
		}

		private static void GenerateRepositories(Project dataLayerProject, SourceControlClient sourceControlClient, RegisteredEntityEnumerator registeredEntityEnumerator, Project modelProject, DataEntriesModelSource dataEntriesModelSource)
		{
			CodeWriter codeWriter = new CodeWriter(dataLayerProject, sourceControlClient);
			var dbRepositoryModelSource = new RepositoryModelSource(registeredEntityEnumerator, modelProject, dataLayerProject, dataEntriesModelSource);
			var interfaceRepositoryGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new InterfaceRepositoryTemplateFactory(), new InterfaceRepositoryFileNamingService(dataLayerProject), codeWriter);
			var dbRepositoryGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryTemplateFactory(), new DbRepositoryFileNamingService(dataLayerProject), codeWriter);
			//var dbRepositoryBaseGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryBaseTemplateFactory(), new DbRepositoryBaseFileNamingService(dataLayerProject), codeWriter);
			interfaceRepositoryGenerator.Generate();
			dbRepositoryGenerator.Generate();
			//dbRepositoryBaseGenerator.Generate();
        }

		private static void GenerateQueryExtensions(Project dataLayerProject, SourceControlClient sourceControlClient, RegisteredEntityEnumerator registeredEntityEnumerator)
		{
			CodeWriter codeWriter = new CodeWriter(dataLayerProject, sourceControlClient);
			var queryableExtensionsModelSource = new QueryableExtensionsModelSource(registeredEntityEnumerator, dataLayerProject);

			var dbRepositoryGenerator = new GenericGenerator<QueryableExtensionsModel>(queryableExtensionsModelSource, new QueryableExtensionsTemplateFactory(), new QueryableExtensionsFileNamingService(dataLayerProject), codeWriter);
			dbRepositoryGenerator.Generate();
		}
	}
}
