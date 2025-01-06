using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries;

public class DataEntriesGenerator(
	IDataLayerProject _dataLayerProject,
	IModelProject _modelProject,
	DbContext _dbContext,
	IGenericGenerator _genericGenerator) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		DataEntriesModelSource dataEntriesModelSource = new DataEntriesModelSource(_dbContext, _modelProject, _dataLayerProject);

		// interface data entries
		await _genericGenerator.GenerateAsync(dataEntriesModelSource, dataEntriesModel => new InterfaceDataEntriesTemplate(dataEntriesModel), new InterfaceDataEntriesFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db data entries
		await _genericGenerator.GenerateAsync(dataEntriesModelSource, dataEntriesModel => new DbDataEntriesTemplate(dataEntriesModel), new DbDataEntriesFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);
	}
}
