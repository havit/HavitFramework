using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources;

public class DataSourcesGenerator(
	IDataLayerProject _dataLayerProject,
	IModelProject _modelProject,
	DbContext _dbContext,
	IGenericGenerator _genericGenerator) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		IModelSource<InterfaceDataSourceModel> interfaceDataSourceModelSource = new InterfaceDataSourceModelSource(_dbContext, _modelProject, _dataLayerProject);
		IModelSource<DbDataSourceModel> dbDataSourceModelSource = new DbDataSourceModelSource(_dbContext, _modelProject, _dataLayerProject);
		IModelSource<FakeDataSourceModel> fakeDataSourceModelSource = new FakeDataSourceModelSource(_dbContext, _modelProject, _dataLayerProject);

		// interface data sources
		await _genericGenerator.GenerateAsync(interfaceDataSourceModelSource, interfaceDataSourceModel => new InterfaceDataSourceTemplate(interfaceDataSourceModel), new InterfaceDataSourceFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db data sources
		await _genericGenerator.GenerateAsync(dbDataSourceModelSource, dbDataSourceModel => new DbDataSourceTemplate(dbDataSourceModel), new DbDataSourceFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// fake data sources
		await _genericGenerator.GenerateAsync(fakeDataSourceModelSource, fakeDataSourceModel => new FakeDataSourceTemplate(fakeDataSourceModel), new FakeDataSourceFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);
	}
}
