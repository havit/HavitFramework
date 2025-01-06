﻿using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
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
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, new InterfaceRepositoryGeneratedTemplateFactory(), new InterfaceRepositoryGeneratedFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// interface repository
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, new InterfaceRepositoryTemplateFactory(), new InterfaceRepositoryFileNamingService(_dataLayerProject), OverwriteBahavior.SkipWhenAlreadyExists, cancellationToken);

		// db repository base (generated/...)
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, new DbRepositoryBaseGeneratedTemplateFactory(), new DbRepositoryBaseGeneratedFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db repository (generated/...)
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, new DbRepositoryGeneratedTemplateFactory(), new DbRepositoryGeneratedFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db repository
		await _genericGenerator.GenerateAsync(dbRepositoryModelSource, new DbRepositoryTemplateFactory(), new DbRepositoryFileNamingService(_dataLayerProject), OverwriteBahavior.SkipWhenAlreadyExists, cancellationToken);
	}
}
