using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Templates;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;

public class RepositoriesGenerator(
	IDataLayerProject _dataLayerProject,
	RepositoryModelSource _dbRepositoryModelSource,
	IGenericGenerator _genericGenerator) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		// interface repository (generated/...)
		await _genericGenerator.GenerateAsync(_dbRepositoryModelSource, repositoryModel => new InterfaceRepositoryGeneratedTemplate(repositoryModel), new InterfaceRepositoryGeneratedFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// interface repository
		await _genericGenerator.GenerateAsync(_dbRepositoryModelSource, repositoryModel => new InterfaceRepositoryTemplate(repositoryModel), new InterfaceRepositoryFileNamingService(_dataLayerProject), OverwriteBehavior.SkipWhenAlreadyExists, cancellationToken);

		// db repository base (generated/...)
		await _genericGenerator.GenerateAsync(_dbRepositoryModelSource, repositoryModel => new DbRepositoryBaseGeneratedTemplate(repositoryModel), new DbRepositoryBaseGeneratedFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db repository (generated/...)
		await _genericGenerator.GenerateAsync(_dbRepositoryModelSource, repositoryModel => new DbRepositoryGeneratedTemplate(repositoryModel), new DbRepositoryGeneratedFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db repository
		await _genericGenerator.GenerateAsync(_dbRepositoryModelSource, repositoryModel => new DbRepositoryTemplate(repositoryModel), new DbRepositoryFileNamingService(_dataLayerProject), OverwriteBehavior.SkipWhenAlreadyExists, cancellationToken);

		// repository query provider (generated/...)
		await _genericGenerator.GenerateAsync(_dbRepositoryModelSource, repositoryModel => new RepositoryQueryProviderTemplate(repositoryModel), new DbRepositoryQueryProviderFileGeneratedNamingService(_dataLayerProject), cancellationToken: cancellationToken);
	}
}
