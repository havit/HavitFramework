using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;

public class RepositoryModelSource : IModelSource<RepositoryModel>, IModelSourceErrorsProvider
{
	private readonly DbContext _dbContext;
	private readonly IModelProject _modelProject;
	private readonly IDataLayerProject _dataLayerProject;

	// Tento model source je DI singleton sdílený mezi generátory, které běží paralelně (viz DataLayerGeneratorRunner.Parallel.ForEachAsync).
	// Lazy (výchozí ExecutionAndPublication) zajistí, že se modely vyhodnotí právě jednou i při souběžném přístupu z více generátorů.
	private readonly Lazy<List<RepositoryModel>> _models;

	public RepositoryModelSource(DbContext dbContext, IModelProject modelProject, IDataLayerProject dataLayerProject)
	{
		_dbContext = dbContext;
		_modelProject = modelProject;
		_dataLayerProject = dataLayerProject;
		_models = new Lazy<List<RepositoryModel>>(GetModelsCore);
	}

	public List<RepositoryModel> GetModels() => _models.Value;

	private List<RepositoryModel> GetModelsCore()
	{
		return (
			from registeredEntity in _dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
			where registeredEntity.FindPrimaryKey()?.Properties.Count == 1
			select new RepositoryModel
			{
				NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
				DbRepositoryName = registeredEntity.ClrType.Name + "DbRepository",
				DbRepositoryBaseName = registeredEntity.ClrType.Name + "DbRepositoryBase",
				InterfaceRepositoryName = "I" + registeredEntity.ClrType.Name + "Repository",
				RepositoryQueryProviderClassName = registeredEntity.ClrType.Name + "DbRepositoryQueryProvider",
				ModelClassNamespace = registeredEntity.ClrType.Namespace,
				ModelClassFullName = registeredEntity.ClrType.FullName,
				ModelClassPrimaryKeyTypeName = registeredEntity.FindPrimaryKey().Properties.Single().ClrType.FullName,
				ModelClassPrimaryKeyPropertyName = registeredEntity.FindPrimaryKey().Properties.Single().Name,
			}).ToList();
	}

	public IEnumerable<string> GetModelErrors()
	{
		return from registeredEntity in _dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
			   where registeredEntity.FindPrimaryKey()?.Properties.Count != 1
			   select $"Entity {registeredEntity.ClrType.FullName} does not have exactly one primary key property.";
	}

	private string GetNamespaceName(string namespaceName, string typeNamespace = "Repositories")
	{
		string modelProjectNamespace = _modelProject.GetProjectRootNamespace();
		if (NamespaceHelper.TryGetRelativeNamespace(namespaceName, modelProjectNamespace, out string relativeNamespace))
		{
			return _dataLayerProject.GetProjectRootNamespace() + "." + typeNamespace + relativeNamespace;
		}
		else
		{
			return namespaceName + "." + typeNamespace;
		}
	}
}
