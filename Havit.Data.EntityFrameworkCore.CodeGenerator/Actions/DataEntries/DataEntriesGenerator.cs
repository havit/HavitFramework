using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Template;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries;

public class DataEntriesGenerator(
	IDataLayerProject _dataLayerProject,
	IModelProject _modelProject,
	DbContext _dbContext,
	ICodeWriter _codeWriter) : IDataLayerGenerator
{
	public async Task GenerateAsync(CancellationToken cancellationToken)
	{
		// TODO: DI?
		DataEntriesModelSource dataEntriesModelSource = new DataEntriesModelSource(_dbContext, _modelProject, _dataLayerProject);

		var interfaceDataEntriesGenerator = new GenericGenerator<DataEntriesModel>(dataEntriesModelSource, new InterfaceDataEntriesTemplateFactory(), new InterfaceDataEntriesFileNamingService(_dataLayerProject), _codeWriter);
		var dbDataEntriesGenerator = new GenericGenerator<DataEntriesModel>(dataEntriesModelSource, new DbDataEntriesTemplateFactory(), new DbDataEntriesFileNamingService(_dataLayerProject), _codeWriter);
		await interfaceDataEntriesGenerator.GenerateAsync(cancellationToken);
		await dbDataEntriesGenerator.GenerateAsync(cancellationToken);
	}
}
