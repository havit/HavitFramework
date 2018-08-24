using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Entity;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model
{
	public class FakeDataSourceModelSource : IModelSource<FakeDataSourceModel>
	{
		private readonly DbContext dbContext;
		private readonly IProject modelProject;
		private readonly IProject dataLayerProject;

		public FakeDataSourceModelSource(DbContext dbContext, IProject modelProject, IProject dataLayerProject)
		{
			this.dbContext = dbContext;
			this.modelProject = modelProject;
			this.dataLayerProject = dataLayerProject;
		}

		public IEnumerable<FakeDataSourceModel> GetModels()
		{
			return (from registeredEntity in dbContext.Model.GetApplicationEntityTypes()
				select new FakeDataSourceModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace, true),
					InterfaceDataSourceFullName = GetNamespaceName(registeredEntity.ClrType.Namespace, false) + ".I" + registeredEntity.ClrType.Name + "DataSource",
					FakeDataSourceClassName = "Fake" + registeredEntity.ClrType.Name + "DataSource",
					ModelClassFullName = registeredEntity.ClrType.FullName
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
