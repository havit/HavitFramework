using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;

public class FakeDataSourceModelSource : IModelSource<FakeDataSourceModel>
{
	private readonly DbContext _dbContext;
	private readonly IModelProject _modelProject;
	private readonly IDataLayerProject _dataLayerProject;

	// Tento model source je DI singleton sdílený mezi generátory, které běží paralelně (viz DataLayerGeneratorRunner.Parallel.ForEachAsync).
	// Lazy (výchozí ExecutionAndPublication) zajistí, že se modely vyhodnotí právě jednou i při souběžném přístupu z více generátorů.
	private readonly Lazy<List<FakeDataSourceModel>> _models;

	public FakeDataSourceModelSource(DbContext dbContext, IModelProject modelProject, IDataLayerProject dataLayerProject)
	{
		_dbContext = dbContext;
		_modelProject = modelProject;
		_dataLayerProject = dataLayerProject;
		_models = new Lazy<List<FakeDataSourceModel>>(GetModelsCore);
	}

	public List<FakeDataSourceModel> GetModels() => _models.Value;

	private List<FakeDataSourceModel> GetModelsCore()
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

		if (NamespaceHelper.TryGetRelativeNamespace(namespaceName, modelProjectNamespace, out string relativeNamespace))
		{
			return _dataLayerProject.GetProjectRootNamespace() + ".DataSources" + relativeNamespace + fakesString;
		}
		else
		{
			return namespaceName + ".DataSources" + fakesString;
		}
	}

}
