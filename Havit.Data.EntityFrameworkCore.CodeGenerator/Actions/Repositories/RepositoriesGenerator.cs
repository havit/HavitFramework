using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Templates;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;

public class RepositoriesGenerator(
	DbContext _dbContext,
	[FromKeyedServices(Project.DataLayerProjectKey)] IProject _dataLayerProject,
	[FromKeyedServices(Project.ModelProjectKey)] IProject _modelProject,
	CammelCaseNamingStrategy cammelCaseNamingStrategy) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		var dataEntriesModelSource = new DataEntriesModelSource(_dbContext, _modelProject, _dataLayerProject, cammelCaseNamingStrategy);

		CodeWriter codeWriter = new CodeWriter(_dataLayerProject);
		var dbRepositoryModelSource = new RepositoryModelSource(_dbContext, _modelProject, _dataLayerProject, dataEntriesModelSource);
		var dbRepositoryBaseGeneratedGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryBaseGeneratedTemplateFactory(), new DbRepositoryBaseGeneratedFileNamingService(_dataLayerProject), codeWriter);
		var interfaceRepositoryGeneratedGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new InterfaceRepositoryGeneratedTemplateFactory(), new InterfaceRepositoryGeneratedFileNamingService(_dataLayerProject), codeWriter);
		var dbRepositoryGeneratedGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryGeneratedTemplateFactory(), new DbRepositoryGeneratedFileNamingService(_dataLayerProject), codeWriter);
		var interfaceRepositoryGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new InterfaceRepositoryTemplateFactory(), new InterfaceRepositoryFileNamingService(_dataLayerProject), codeWriter, false);
		var dbRepositoryGenerator = new GenericGenerator<RepositoryModel>(dbRepositoryModelSource, new DbRepositoryTemplateFactory(), new DbRepositoryFileNamingService(_dataLayerProject), codeWriter, false);
		await interfaceRepositoryGeneratedGenerator.GenerateAsync(cancellationToken);
		await interfaceRepositoryGenerator.GenerateAsync(cancellationToken);
		await dbRepositoryBaseGeneratedGenerator.GenerateAsync(cancellationToken);
		await dbRepositoryGeneratedGenerator.GenerateAsync(cancellationToken);
		await dbRepositoryGenerator.GenerateAsync(cancellationToken);
	}
}
