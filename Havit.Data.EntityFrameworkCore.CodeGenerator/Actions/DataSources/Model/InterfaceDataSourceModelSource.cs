using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;

public class InterfaceDataSourceModelSource : IModelSource<InterfaceDataSourceModel>
{
	private readonly DbContext _dbContext;
	private readonly IModelProject _modelProject;
	private readonly IDataLayerProject _dataLayerProject;

	public InterfaceDataSourceModelSource(DbContext dbContext, IModelProject modelProject, IDataLayerProject dataLayerProject)
	{
		_dbContext = dbContext;
		_modelProject = modelProject;
		_dataLayerProject = dataLayerProject;
	}

	public IEnumerable<InterfaceDataSourceModel> GetModels()
	{
		return (from registeredEntity in _dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
				select new InterfaceDataSourceModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
					InterfaceDataSourceName = "I" + registeredEntity.ClrType.Name + "DataSource",
					ModelClassFullName = registeredEntity.ClrType.FullName
				}).ToList();
	}

	private string GetNamespaceName(string namespaceName)
	{
		string modelProjectNamespace = _modelProject.GetProjectRootNamespace();
		if (namespaceName.StartsWith(modelProjectNamespace))
		{
			return _dataLayerProject.GetProjectRootNamespace() + ".DataSources" + namespaceName.Substring(modelProjectNamespace.Length);
		}
		else
		{
			return namespaceName + ".DataSources";
		}
	}

}
