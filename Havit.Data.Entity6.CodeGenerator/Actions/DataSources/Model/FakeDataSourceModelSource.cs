using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.CodeGenerator.Entity;
using Havit.Data.Entity.CodeGenerator.Services;
using Havit.Data.Entity.Mapping.Internal;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model
{
	public class FakeDataSourceModelSource : IModelSource<FakeDataSourceModel>
	{
		private readonly DbContext dbContext;
		private readonly Project modelProject;
		private readonly Project dataLayerProject;

		public FakeDataSourceModelSource(DbContext dbContext, Project modelProject, Project dataLayerProject)
		{
			this.dbContext = dbContext;
			this.modelProject = modelProject;
			this.dataLayerProject = dataLayerProject;
		}

		public IEnumerable<FakeDataSourceModel> GetModels()
		{
			return (from registeredEntity in dbContext.GetRegisteredEntities()
				select new FakeDataSourceModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.NamespaceName, true),
					InterfaceDataSourceFullName = GetNamespaceName(registeredEntity.NamespaceName, false) + ".I" + registeredEntity.ClassName + "DataSource",
					FakeDataSourceClassName = "Fake" + registeredEntity.ClassName + "DataSource",
					ModelClassFullName = registeredEntity.FullName
				}).ToList();
		}

		private string GetNamespaceName(string namespaceName, bool addFakes)
		{
			string modelProjectNamespace = modelProject.GetProjectRootNamespace();
			string fakesString = addFakes ? ".Fakes" : "";

			if (namespaceName.StartsWith(modelProjectNamespace))
			{
				return dataLayerProject.GetProjectRootNamespace() + ".DataSources" + namespaceName.Substring(modelProjectNamespace.Length) + fakesString;
			}
			else
			{
				return namespaceName + ".DataSources" + fakesString;
			}
		}

	}
}
