using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.CodeGenerator.Entity;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model
{
	public class DbDataSourceModelSource : IModelSource<DbDataSourceModel>
	{
		private readonly RegisteredEntityEnumerator registeredEntityEnumerator;
		private readonly Project modelProject;
		private readonly Project dataLayerProject;

		public DbDataSourceModelSource(RegisteredEntityEnumerator registeredEntityEnumerator, Project modelProject, Project dataLayerProject)
		{
			this.registeredEntityEnumerator = registeredEntityEnumerator;
			this.modelProject = modelProject;
			this.dataLayerProject = dataLayerProject;
		}

		public IEnumerable<DbDataSourceModel> GetModels()
		{
			return (from registeredEntity in registeredEntityEnumerator.GetRegisteredEntities()
				select new DbDataSourceModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.NamespaceName),
					InterfaceDataSourceFullName = "I" + registeredEntity.ClassName + "DataSource",
					DbDataSourceClassName = registeredEntity.ClassName + "DbDataSource",
					ModelClassFullName = registeredEntity.FullName
				}).ToList();
		}

		// TODO: Opakující se kód (obdobný)
		private string GetNamespaceName(string namespaceName)
		{
			string modelProjectNamespace = modelProject.GetProjectRootNamespace();
			if (namespaceName.StartsWith(modelProjectNamespace))
			{
				return dataLayerProject.GetProjectRootNamespace() + ".DataSources" + namespaceName.Substring(modelProjectNamespace.Length);
			}
			else
			{
				return namespaceName + ".DataSources";
			}
		}

	}
}
