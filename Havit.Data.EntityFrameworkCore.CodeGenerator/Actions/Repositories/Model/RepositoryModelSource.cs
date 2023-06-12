using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Microsoft.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Metadata;
using System;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model
{
	public class RepositoryModelSource : IModelSource<RepositoryModel>
	{
		private readonly DbContext dbContext;
		private readonly IProject modelProject;
		private readonly IProject dataLayerProject;
		private readonly DataEntriesModelSource dataEntriesModelSource;

		public RepositoryModelSource(DbContext dbContext, IProject modelProject, IProject dataLayerProject, DataEntriesModelSource dataEntriesModelSource)
		{
			this.dbContext = dbContext;
			this.modelProject = modelProject;
			this.dataLayerProject = dataLayerProject;
			this.dataEntriesModelSource = dataEntriesModelSource;
		}

		public IEnumerable<RepositoryModel> GetModels()
		{
			IEnumerable<DataEntriesModel> dataEntriesModels = dataEntriesModelSource.GetModels();

			return (from registeredEntity in dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
					select new RepositoryModel
					{
						NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
						DbRepositoryName = registeredEntity.ClrType.Name + "DbRepository",
						DbRepositoryBaseName = registeredEntity.ClrType.Name + "DbRepositoryBase",
						InterfaceRepositoryName = "I" + registeredEntity.ClrType.Name + "Repository",
						ModelClassNamespace = registeredEntity.ClrType.Namespace,
						ModelClassFullName = registeredEntity.ClrType.FullName,
						//GenerateGetObjectByEntryEnumMethod = !registeredEntity.HasDatabaseGeneratedIdentity && registeredEntity.HasEntryEnum,
						//DataSourceDependencyFullName = GetNamespaceName(registeredEntity.ClrType.Namespace, "DataSources") + ".I" + registeredEntity.ClrType.Name + "DataSource"
					}).ToList();
		}

		private string GetNamespaceName(string namespaceName, string typeNamespace = "Repositories")
		{
			string modelProjectNamespace = modelProject.GetProjectRootNamespace();
			if (namespaceName.StartsWith(modelProjectNamespace))
			{
				return dataLayerProject.GetProjectRootNamespace() + "." + typeNamespace + namespaceName.Substring(modelProjectNamespace.Length);
			}
			else
			{
				return namespaceName + "." + typeNamespace;
			}
		}
	}
}
