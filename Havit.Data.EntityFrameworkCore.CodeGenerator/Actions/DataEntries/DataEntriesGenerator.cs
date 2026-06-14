using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries;

public class DataEntriesGenerator(
	IDataLayerProject _dataLayerProject,
	DataEntriesModelSource _dataEntriesModelSource,
	IGenericGenerator _genericGenerator) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		// interface data entries
		await _genericGenerator.GenerateAsync(_dataEntriesModelSource, dataEntriesModel => new InterfaceDataEntriesTemplate(dataEntriesModel), new InterfaceDataEntriesFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db data entries
		await _genericGenerator.GenerateAsync(_dataEntriesModelSource, dataEntriesModel => new DbDataEntriesTemplate(dataEntriesModel), new DbDataEntriesFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);
	}
}
