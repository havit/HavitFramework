using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Entity;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model
{
	public class RepositoryModelSource : IModelSource<RepositoryModel>
	{
		private readonly RegisteredEntityEnumerator registeredEntityEnumerator;
		private readonly Project modelProject;
		private readonly Project dataLayerProject;
	    private readonly DataEntriesModelSource dataEntriesModelSource;

	    public RepositoryModelSource(RegisteredEntityEnumerator registeredEntityEnumerator, Project modelProject, Project dataLayerProject, DataEntriesModelSource dataEntriesModelSource)
		{
			this.registeredEntityEnumerator = registeredEntityEnumerator;
			this.modelProject = modelProject;
			this.dataLayerProject = dataLayerProject;
	        this.dataEntriesModelSource = dataEntriesModelSource;
		}

		public IEnumerable<RepositoryModel> GetModels()
		{
		    IEnumerable<DataEntriesModel> dataEntriesModels = dataEntriesModelSource.GetModels();

			return (from registeredEntity in registeredEntityEnumerator.GetRegisteredEntities()
					select new RepositoryModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.NamespaceName),
					DbRepositoryName = registeredEntity.ClassName + "DbRepository",
					DbRepositoryBaseName = registeredEntity.ClassName + "DbRepositoryBase",
					InterfaceRepositoryName = "I" + registeredEntity.ClassName + "Repository",
					ModelClassNamespace = registeredEntity.NamespaceName,
					ModelClassFullName = registeredEntity.FullName,
					GenerateGetObjectByEntryEnumMethod = !registeredEntity.HasDatabaseGeneratedIdentity && registeredEntity.HasEntryEnum,
					DataSourceDependencyFullName = GetNamespaceName(registeredEntity.NamespaceName, "DataSources") + ".I" + registeredEntity.ClassName + "DataSource"
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
