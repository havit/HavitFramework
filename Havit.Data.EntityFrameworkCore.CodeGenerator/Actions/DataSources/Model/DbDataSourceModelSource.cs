using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;

public class DbDataSourceModelSource : IModelSource<DbDataSourceModel>
{
	private readonly DbContext _dbContext;
	private readonly IModelProject _modelProject;
	private readonly IDataLayerProject _dataLayerProject;

	// Tento model source je DI singleton sdílený mezi generátory, které běží paralelně (viz DataLayerGeneratorRunner.Parallel.ForEachAsync).
	// Lazy (výchozí ExecutionAndPublication) zajistí, že se modely vyhodnotí právě jednou i při souběžném přístupu z více generátorů.
	private readonly Lazy<List<DbDataSourceModel>> _models;

	public DbDataSourceModelSource(DbContext dbContext, IModelProject modelProject, IDataLayerProject dataLayerProject)
	{
		_dbContext = dbContext;
		_modelProject = modelProject;
		_dataLayerProject = dataLayerProject;
		_models = new Lazy<List<DbDataSourceModel>>(GetModelsCore);
	}

	public List<DbDataSourceModel> GetModels() => _models.Value;

	private List<DbDataSourceModel> GetModelsCore()
	{
		return (from registeredEntity in _dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
				select new DbDataSourceModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
					InterfaceDataSourceFullName = "I" + registeredEntity.ClrType.Name + "DataSource",
					DbDataSourceClassName = registeredEntity.ClrType.Name + "DbDataSource",
					ModelClassFullName = registeredEntity.ClrType.FullName
				}).ToList();
	}

	// TODO: Opakující se kód (obdobný)
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
