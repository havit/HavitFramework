using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Templates;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;

public class RepositoriesGenerator(
	DbContext _dbContext,
	[FromKeyedServices(Project.DataLayerProjectKey)] IProject _dataLayerProject,
	[FromKeyedServices(Project.ModelProjectKey)] IProject _modelProject,
	ICodeWriter _codeWriter) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		var dataEntriesModelSource = new DataEntriesModelSource(_dbContext, _modelProject, _dataLayerProject);

		var dbRepositoryModelSource = new RepositoryModelSource(_dbContext, _modelProject, _dataLayerProject, dataEntriesModelSource);
		var dbRepositoryBaseGeneratedGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryBaseGeneratedTemplateFactory(), new DbRepositoryBaseGeneratedFileNamingService(_dataLayerProject), _codeWriter);
		var interfaceRepositoryGeneratedGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new InterfaceRepositoryGeneratedTemplateFactory(), new InterfaceRepositoryGeneratedFileNamingService(_dataLayerProject), _codeWriter);
		var dbRepositoryGeneratedGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryGeneratedTemplateFactory(), new DbRepositoryGeneratedFileNamingService(_dataLayerProject), _codeWriter);
		var interfaceRepositoryGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new InterfaceRepositoryTemplateFactory(), new InterfaceRepositoryFileNamingService(_dataLayerProject), _codeWriter, OverwriteBahavior.SkipWhenAlreadyExists);
		var dbRepositoryGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryTemplateFactory(), new DbRepositoryFileNamingService(_dataLayerProject), _codeWriter, OverwriteBahavior.SkipWhenAlreadyExists);
		await interfaceRepositoryGeneratedGenerator.GenerateAsync(cancellationToken);
		await interfaceRepositoryGenerator.GenerateAsync(cancellationToken);
		await dbRepositoryBaseGeneratedGenerator.GenerateAsync(cancellationToken);
		await dbRepositoryGeneratedGenerator.GenerateAsync(cancellationToken);
		await dbRepositoryGenerator.GenerateAsync(cancellationToken);
	}
}
