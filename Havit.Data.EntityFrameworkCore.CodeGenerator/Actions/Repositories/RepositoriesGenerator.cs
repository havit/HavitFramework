using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Templates;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;

public class RepositoriesGenerator(
	DbContext _dbContext,
	IDataLayerProject _dataLayerProject,
	IModelProject _modelProject,
	IGenericGenerator _genericGenerator) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		var dbRepositoryModelSource = new RepositoryModelSource(_dbContext, _modelProject, _dataLayerProject);

		// interface repository (generated/...)
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, repositoryModel => new InterfaceRepositoryGeneratedTemplate(repositoryModel), new InterfaceRepositoryGeneratedFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// interface repository
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, repositoryModel => new InterfaceRepositoryTemplate(repositoryModel), new InterfaceRepositoryFileNamingService(_dataLayerProject), OverwriteBahavior.SkipWhenAlreadyExists, cancellationToken);

		// db repository base (generated/...)
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, repositoryModel => new DbRepositoryBaseGeneratedTemplate(repositoryModel), new DbRepositoryBaseGeneratedFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db repository (generated/...)
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, repositoryModel => new DbRepositoryGeneratedTemplate(repositoryModel), new DbRepositoryGeneratedFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db repository
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, repositoryModel => new DbRepositoryTemplate(repositoryModel), new DbRepositoryFileNamingService(_dataLayerProject), OverwriteBahavior.SkipWhenAlreadyExists, cancellationToken);

		// repository query provider (generated/...)
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, repositoryModel => new RepositoryQueryProviderTemplate(repositoryModel), new DbRepositoryQueryProviderFileGeneratedNamingService(_dataLayerProject), cancellationToken: cancellationToken);
	}
}
