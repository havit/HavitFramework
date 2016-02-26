using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.CodeGenerator.Entity;
using Havit.Data.Entity.CodeGenerator.Services;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model
{
	public class InterfaceDataSourceModelSource : IModelSource<InterfaceDataSourceModel>
	{
		private readonly RegisteredEntityEnumerator registeredEntityEnumerator;
		private readonly Project modelProject;
		private readonly Project dataLayerProject;
		private readonly ISoftDeleteManager softDeleteManager;

		public InterfaceDataSourceModelSource(RegisteredEntityEnumerator registeredEntityEnumerator, Project modelProject, Project dataLayerProject, ISoftDeleteManager softDeleteManager)
		{
			this.registeredEntityEnumerator = registeredEntityEnumerator;
			this.modelProject = modelProject;
			this.dataLayerProject = dataLayerProject;
			this.softDeleteManager = softDeleteManager;
		}

		public IEnumerable<InterfaceDataSourceModel> GetModels()
		{
			return (from registeredEntity in registeredEntityEnumerator.GetRegisteredEntities()
				select new InterfaceDataSourceModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.NamespaceName),
					InterfaceDataSourceName = "I" + registeredEntity.ClassName + "DataSource",
					ModelClassFullName = registeredEntity.FullName,
					RenderDataSourceSoftDelete = softDeleteManager.IsSoftDeleteSupported(registeredEntity.Type)
				}).ToList();
		}

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
