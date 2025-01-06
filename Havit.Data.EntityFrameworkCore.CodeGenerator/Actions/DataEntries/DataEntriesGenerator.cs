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
		await _genericGenerator.GenerateAsync(dataEntriesModelSource, new InterfaceDataEntriesTemplateFactory(), new InterfaceDataEntriesFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);

		// db data entries
		await _genericGenerator.GenerateAsync(dataEntriesModelSource, new DbDataEntriesTemplateFactory(), new DbDataEntriesFileNamingService(_dataLayerProject), cancellationToken: cancellationToken);
	}
}
