using System.Collections.Generic;
using System.Linq;
using Havit.CodeGenerator.Entity;
using Havit.CodeGenerator.Services;

namespace Havit.CodeGenerator.Actions.Repositories.Model
{
	public class InterfaceRepositoryModelSource : IModelSource<InterfaceRepositoryModel>
	{
		private readonly RegisteredEntityEnumerator registeredEntityEnumerator;
		private readonly Project modelProject;
		private readonly Project dataLayerProject;

		public InterfaceRepositoryModelSource(RegisteredEntityEnumerator registeredEntityEnumerator, Project modelProject, Project dataLayerProject)
		{
			this.registeredEntityEnumerator = registeredEntityEnumerator;
			this.modelProject = modelProject;
			this.dataLayerProject = dataLayerProject;
		}

		public IEnumerable<InterfaceRepositoryModel> GetModels()
		{
			return (from registeredEntity in registeredEntityEnumerator.GetRegisteredEntities()
				select new InterfaceRepositoryModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.NamespaceName),
					InterfaceRepositoryName = "I" + registeredEntity.ClassName + "Repository",
					ModelClassFullName = registeredEntity.FullName,					
				}).ToList();
		}

		private string GetNamespaceName(string namespaceName)
		{
			string modelProjectNamespace = modelProject.GetProjectRootNamespace();
			if (namespaceName.StartsWith(modelProjectNamespace))
			{
				return dataLayerProject.GetProjectRootNamespace() + ".Repositories" + namespaceName.Substring(modelProjectNamespace.Length);
			}
			else
			{
				return namespaceName + ".Repositories";
			}
		}

	}
}
