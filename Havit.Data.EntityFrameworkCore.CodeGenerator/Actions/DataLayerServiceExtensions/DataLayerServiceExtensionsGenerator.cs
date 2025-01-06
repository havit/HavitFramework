using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions;

public class DataLayerServiceExtensionsGenerator(
	IModelProject _modelProject,
	IDataLayerProject _dataLayerProject,
	DbContext _dbContext,
	ICodeWriter _codeWriter) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		string targetFilename = Path.Combine(_dataLayerProject.GetProjectRootPath(), "_generated\\DataLayerServiceExtensions.cs");

		// TODO: Lépe pomocí DI? Nebo místo sources rovnou řešit modely?
		DataEntriesModelSource dataEntriesModelSource = new DataEntriesModelSource(_dbContext, _modelProject, _dataLayerProject);
		DbDataSourceModelSource dbDataSourceModelSource = new DbDataSourceModelSource(_dbContext, _modelProject, _dataLayerProject);
		RepositoryModelSource repositoryModelSource = new RepositoryModelSource(_dbContext, _modelProject, _dataLayerProject);

		DataLayerServiceExtensionsModelSource modelSource = new DataLayerServiceExtensionsModelSource(_dataLayerProject, dataEntriesModelSource, dbDataSourceModelSource, repositoryModelSource);
		DataLayerServiceExtensionsTemplate template = new DataLayerServiceExtensionsTemplate(modelSource.GetModels().Single());
		await _codeWriter.SaveAsync(targetFilename, template.TransformText(), OverwriteBahavior.OverwriteWhenFileAlreadyExists, cancellationToken);
	}
}
