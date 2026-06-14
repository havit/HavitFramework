using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;

public class InterfaceDataSourceModelSource : IModelSource<InterfaceDataSourceModel>
{
	private readonly DbContext _dbContext;
	private readonly IModelProject _modelProject;
	private readonly IDataLayerProject _dataLayerProject;

	// Tento model source je DI singleton sdílený mezi generátory, které běží paralelně (viz DataLayerGeneratorRunner.Parallel.ForEachAsync).
	// Lazy (výchozí ExecutionAndPublication) zajistí, že se modely vyhodnotí právě jednou i při souběžném přístupu z více generátorů.
	private readonly Lazy<List<InterfaceDataSourceModel>> _models;

	public InterfaceDataSourceModelSource(DbContext dbContext, IModelProject modelProject, IDataLayerProject dataLayerProject)
	{
		_dbContext = dbContext;
		_modelProject = modelProject;
		_dataLayerProject = dataLayerProject;
		_models = new Lazy<List<InterfaceDataSourceModel>>(GetModelsCore);
	}

	public List<InterfaceDataSourceModel> GetModels() => _models.Value;

	private List<InterfaceDataSourceModel> GetModelsCore()
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
		if (NamespaceHelper.TryGetRelativeNamespace(namespaceName, modelProjectNamespace, out string relativeNamespace))
		{
			return _dataLayerProject.GetProjectRootNamespace() + ".DataSources" + relativeNamespace;
		}
		else
		{
			return namespaceName + ".DataSources";
		}
	}

}
