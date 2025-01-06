using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources;

public class DataSourcesGenerator(
	[FromKeyedServices(Project.DataLayerProjectKey)] IProject _dataLayerProject,
	[FromKeyedServices(Project.ModelProjectKey)] IProject _modelProject,
	DbContext _dbContext,
	ICodeWriter _codeWriter) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		IModelSource<InterfaceDataSourceModel> interfaceDataSourceModelSource = new InterfaceDataSourceModelSource(_dbContext, _modelProject, _dataLayerProject);
		IModelSource<DbDataSourceModel> dbDataSourceModelSource = new DbDataSourceModelSource(_dbContext, _modelProject, _dataLayerProject);
		IModelSource<FakeDataSourceModel> fakeDataSourceModelSource = new FakeDataSourceModelSource(_dbContext, _modelProject, _dataLayerProject);
		var interfaceDataSourceGenerator = new GenericGenerator<InterfaceDataSourceModel>(interfaceDataSourceModelSource, new InterfaceDataSourceTemplateFactory(), new InterfaceDataSourceFileNamingService(_dataLayerProject), _codeWriter);
		var dbDataSourceGenerator = new GenericGenerator<DbDataSourceModel>(dbDataSourceModelSource, new DbDataSourceTemplateFactory(), new DbDataSourceFileNamingService(_dataLayerProject), _codeWriter);
		var fakeDataSourceGenerator = new GenericGenerator<FakeDataSourceModel>(fakeDataSourceModelSource, new FakeDataSourceTemplateFactory(), new FakeDataSourceFileNamingService(_dataLayerProject), _codeWriter);
		await interfaceDataSourceGenerator.GenerateAsync(cancellationToken);
		await dbDataSourceGenerator.GenerateAsync(cancellationToken);
		await fakeDataSourceGenerator.GenerateAsync(cancellationToken);
	}
}
