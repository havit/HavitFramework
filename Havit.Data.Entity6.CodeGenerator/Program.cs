﻿using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Reflection;
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
using Havit.Data.Entity.CodeGenerator.Services.SourceControl;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Services.TimeServices;

namespace Havit.Data.Entity.CodeGenerator;

internal static class Program
{
	internal static void Main(string[] args)
	{
		IProject modelProject = null;
		IProject dataLayerProject = null;
		DbContext dbContext = null;
		ISourceControlClient sourceControlClient = new NullSourceControlClient();

		DirectoryInfo solutionDirectory = new DirectoryInfo(Environment.CurrentDirectory);// @"D:\Dev\002.HFW-NewProjectTemplate";

		while (System.IO.Directory.GetFiles(solutionDirectory.FullName, "*.sln", SearchOption.TopDirectoryOnly).Length == 0)
		{
			if (solutionDirectory.Root.FullName == solutionDirectory.FullName)
			{
				throw new InvalidOperationException("Solution file was not found.");
			}
			solutionDirectory = solutionDirectory.Parent;
		}

		string[] files = System.IO.Directory.GetFiles(Path.Combine(solutionDirectory.FullName, @"Entity\bin"), "*.Entity.dll", SearchOption.AllDirectories);
		if (files.Length == 0)
		{
			Console.WriteLine("Assembly *.Entity.dll was not found.");
			return;
		}

		string file = files.Where(item => !item.EndsWith("Havit.Entity.dll")).OrderByDescending(item => System.IO.File.GetLastAccessTime(item)).First();
		Console.WriteLine($"Using metadata from assembly {file}.");

		Type dbContextType;

		Stopwatch dbContextTypeInitializationStopwatch = Stopwatch.StartNew();
		Assembly assembly = Assembly.LoadFrom(file);
#if DEBUG
		DisplayTargetFramework(assembly);
#endif

		try
		{
			dbContextType = assembly.GetTypes().SingleOrDefault(type => !type.IsAbstract && type.GetInterfaces().Contains(typeof(Havit.Data.Entity.IDbContext)));
		}
		catch (ReflectionTypeLoadException exception)
		{
			foreach (var message in exception.LoaderExceptions.Select(item => item.ToString()).Distinct())
			{
				Console.WriteLine(message);
			}
			return;
		}

		if (dbContextType == null)
		{
			Console.WriteLine("No IDbContext implementation was found.");
			return;
		}

		Parallel.Invoke(
			() =>
			{
				dbContext = new DbContextActivator().Activate(dbContextType);

				Type nullDatabaseInitializerType = typeof(NullDatabaseInitializer<>).MakeGenericType(dbContextType);
				object nullDatabaseInitializer = Activator.CreateInstance(nullDatabaseInitializerType);

				// System.Data.Entity.Database.SetInitializer<MyDbContext>(doNothingInitializer);
				MethodInfo setInitializerMethod = typeof(System.Data.Entity.Database).GetMethod("SetInitializer", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(dbContextType);
				setInitializerMethod.Invoke(null, new object[] { nullDatabaseInitializer });
			},
			() => modelProject = new ProjectFactory().Create(Path.Combine(solutionDirectory.FullName, @"Model\Model.csproj")),
			() => dataLayerProject = new ProjectFactory().Create(Path.Combine(solutionDirectory.FullName, @"DataLayer\DataLayer.csproj"))
		);

		Console.WriteLine($"Initializing DbContext...");
		ObjectContext objectContext = ((IObjectContextAdapter)dbContext).ObjectContext; // z nějakého důvodu je třeba, viz Bug 39328: CodeGenerator: Na 129.ECO nejde přegenerovat Data Layer
		CammelCaseNamingStrategy cammelCaseNamingStrategy = new CammelCaseNamingStrategy();

		dbContextTypeInitializationStopwatch.Stop();
		Console.WriteLine("DbContext initialization completed in {0} ms.", (int)dbContextTypeInitializationStopwatch.Elapsed.TotalMilliseconds);

		Stopwatch codeGenerationStopwath = Stopwatch.StartNew();
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

		codeGenerationStopwath.Stop();
		Console.WriteLine("Code generation completed in {0} ms.", (int)codeGenerationStopwath.Elapsed.TotalMilliseconds);
	}

#if DEBUG
	private static void DisplayTargetFramework(Assembly assembly)
	{
		try
		{
			if (assembly.CustomAttributes != null)
			{
				CustomAttributeData attribute = assembly.CustomAttributes.Where(customDataAttribute => typeof(System.Runtime.Versioning.TargetFrameworkAttribute).IsAssignableFrom(customDataAttribute.AttributeType)).SingleOrDefault();
				CustomAttributeNamedArgument? namedArgument = attribute?.NamedArguments?.Where(na => na.MemberName == nameof(System.Runtime.Versioning.TargetFrameworkAttribute.FrameworkDisplayName)).SingleOrDefault();
				if (namedArgument != null)
				{
					Console.WriteLine("Target framework: " + namedArgument.Value.TypedValue.ToString().Trim('\"'));
					return;
				}
			}
			Console.WriteLine("Target framework not identified.");
			return;
		}
		catch
		{
			Console.WriteLine("An error has occured during target framework detection.");
			// ignore exception, do not throw
		}
	}
#endif

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
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(new ServerTimeService());
		IModelSource<InterfaceDataSourceModel> interfaceDataSourceModelSource = new InterfaceDataSourceModelSource(dbContext, modelProject, dataLayerProject, softDeleteManager);
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

	private static void GenerateQueryExtensions(IProject dataLayerProject, ISourceControlClient sourceControlClient, DbContext dbContext)
	{
		CodeWriter codeWriter = new CodeWriter(dataLayerProject, sourceControlClient);
		var queryableExtensionsModelSource = new QueryableExtensionsModelSource(dbContext, dataLayerProject);

		var dbRepositoryGenerator = new GenericGenerator<QueryableExtensionsModel>(queryableExtensionsModelSource, new QueryableExtensionsTemplateFactory(), new QueryableExtensionsFileNamingService(dataLayerProject), codeWriter);
		dbRepositoryGenerator.Generate();
	}
}
