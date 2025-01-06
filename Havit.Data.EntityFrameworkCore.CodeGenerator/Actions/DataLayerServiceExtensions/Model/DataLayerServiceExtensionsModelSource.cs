using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Model;

public class DataLayerServiceExtensionsModelSource : IModelSource<DataLayerServiceExtensionsModel>
{
	private readonly IProject _dataLayerProject;
	private readonly DataEntriesModelSource _dataEntriesModelSource;
	private readonly DbDataSourceModelSource _dbDataSourceModelSource;
	private readonly RepositoryModelSource _repositoryModelSource;

	public DataLayerServiceExtensionsModelSource(
		IProject dataLayerProject,
		DataEntriesModelSource dataEntriesModelSource,
		DbDataSourceModelSource dbDataSourceModelSource,
		RepositoryModelSource repositoryModelSource)
	{
		_dataLayerProject = dataLayerProject;
		_dataEntriesModelSource = dataEntriesModelSource;
		_dbDataSourceModelSource = dbDataSourceModelSource;
		_repositoryModelSource = repositoryModelSource;
	}
	public IEnumerable<DataLayerServiceExtensionsModel> GetModels()
	{
		yield return new DataLayerServiceExtensionsModel
		{
			NamespaceName = _dataLayerProject.GetProjectRootNamespace(),
			DataEntries = _dataEntriesModelSource.GetModels().ToList(),
			DataSources = _dbDataSourceModelSource.GetModels().ToList(),
			Repositories = _repositoryModelSource.GetModels().ToList()
		};
	}
}
