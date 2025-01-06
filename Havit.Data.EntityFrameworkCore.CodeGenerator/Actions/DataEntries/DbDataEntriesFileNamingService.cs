using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries;

public class DbDataEntriesFileNamingService(IDataLayerProject dataLayerProject) : FileNamingServiceBase<DataEntriesModel>(dataLayerProject)
{
	protected override string GetClassName(DataEntriesModel model)
	{
		return model.DbClassName;
	}

	protected override string GetNamespaceName(DataEntriesModel model)
	{
		return model.NamespaceName;
	}
}
