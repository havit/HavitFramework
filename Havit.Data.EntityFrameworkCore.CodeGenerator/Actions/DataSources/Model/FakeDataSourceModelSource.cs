using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;

public class FakeDataSourceModelSource : IModelSource<FakeDataSourceModel>
{
	private readonly DbContext _dbContext;
	private readonly IModelProject _modelProject;
	private readonly IDataLayerProject _dataLayerProject;

	public FakeDataSourceModelSource(DbContext dbContext, IModelProject modelProject, IDataLayerProject dataLayerProject)
	{
		_dbContext = dbContext;
		_modelProject = modelProject;
		_dataLayerProject = dataLayerProject;
	}

	public List<FakeDataSourceModel> GetModels()
	{
		return (from registeredEntity in _dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
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
		string modelProjectNamespace = _modelProject.GetProjectRootNamespace();
		string fakesString = addFakes ? ".Fakes" : "";

		if (namespaceName.StartsWith(modelProjectNamespace))
		{
			return _dataLayerProject.GetProjectRootNamespace() + ".DataSources" + namespaceName.Substring(modelProjectNamespace.Length) + fakesString;
		}
		else
		{
			return namespaceName + ".DataSources" + fakesString;
		}
	}

}
