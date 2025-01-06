using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;

public class RepositoryModelSource : IModelSource<RepositoryModel>
{
	private readonly DbContext _dbContext;
	private readonly IModelProject _modelProject;
	private readonly IDataLayerProject _dataLayerProject;
	private readonly DataEntriesModelSource _dataEntriesModelSource;

	public RepositoryModelSource(DbContext dbContext, IModelProject modelProject, IDataLayerProject dataLayerProject, DataEntriesModelSource dataEntriesModelSource)
	{
		_dbContext = dbContext;
		_modelProject = modelProject;
		_dataLayerProject = dataLayerProject;
		_dataEntriesModelSource = dataEntriesModelSource;
	}

	public IEnumerable<RepositoryModel> GetModels()
	{
		IEnumerable<DataEntriesModel> dataEntriesModels = _dataEntriesModelSource.GetModels();

		return (from registeredEntity in _dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
				select new RepositoryModel
				{
					NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
					DbRepositoryName = registeredEntity.ClrType.Name + "DbRepository",
					DbRepositoryBaseName = registeredEntity.ClrType.Name + "DbRepositoryBase",
					InterfaceRepositoryName = "I" + registeredEntity.ClrType.Name + "Repository",
					ModelClassNamespace = registeredEntity.ClrType.Namespace,
					ModelClassFullName = registeredEntity.ClrType.FullName,
					//GenerateGetObjectByEntryEnumMethod = !registeredEntity.HasDatabaseGeneratedIdentity && registeredEntity.HasEntryEnum,
					//DataSourceDependencyFullName = GetNamespaceName(registeredEntity.ClrType.Namespace, "DataSources") + ".I" + registeredEntity.ClrType.Name + "DataSource"
				}).ToList();
	}

	private string GetNamespaceName(string namespaceName, string typeNamespace = "Repositories")
	{
		string modelProjectNamespace = _modelProject.GetProjectRootNamespace();
		if (namespaceName.StartsWith(modelProjectNamespace))
		{
			return _dataLayerProject.GetProjectRootNamespace() + "." + typeNamespace + namespaceName.Substring(modelProjectNamespace.Length);
		}
		else
		{
			return namespaceName + "." + typeNamespace;
		}
	}
}
