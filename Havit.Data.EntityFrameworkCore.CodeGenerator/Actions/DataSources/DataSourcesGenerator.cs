using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources;

public class DataSourcesGenerator(
	IDataLayerProject _dataLayerProject,
	InterfaceDataSourceModelSource _interfaceDataSourceModelSource,
	DbDataSourceModelSource _dbDataSourceModelSource,
	FakeDataSourceModelSource _fakeDataSourceModelSource,
	IGenericGenerator _genericGenerator) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		// interface data sources
		await _genericGenerator.GenerateAsync(_interfaceDataSourceModelSource, interfaceDataSourceModel => new InterfaceDataSourceTemplate(interfaceDataSourceModel), new InterfaceDataSourceFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db data sources
		await _genericGenerator.GenerateAsync(_dbDataSourceModelSource, dbDataSourceModel => new DbDataSourceTemplate(dbDataSourceModel), new DbDataSourceFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// fake data sources
		await _genericGenerator.GenerateAsync(_fakeDataSourceModelSource, fakeDataSourceModel => new FakeDataSourceTemplate(fakeDataSourceModel), new FakeDataSourceFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);
	}
}
