using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions;

public class DataLayerServiceExtensionsGenerator(
	IDataLayerProject _dataLayerProject,
	DataEntriesModelSource _dataEntriesModelSource,
	DbDataSourceModelSource _dbDataSourceModelSource,
	RepositoryModelSource _repositoryModelSource,
	ICodeWriter _codeWriter) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		string targetFilename = Path.Combine(_dataLayerProject.GetProjectRootPath(), "_generated", "DataLayerServiceExtensions.cs");

		var dataLayerServiceExtensionsModel = new DataLayerServiceExtensionsModel
		{
			NamespaceName = _dataLayerProject.GetProjectRootNamespace(),
			DataEntries = _dataEntriesModelSource.GetModels(),
			DataSources = _dbDataSourceModelSource.GetModels(),
			Repositories = _repositoryModelSource.GetModels()
		};

		DataLayerServiceExtensionsTemplate template = new DataLayerServiceExtensionsTemplate(dataLayerServiceExtensionsModel);
		await _codeWriter.SaveAsync(targetFilename, template.TransformText(), OverwriteBehavior.OverwriteWhenFileAlreadyExists, cancellationToken);
	}
}
