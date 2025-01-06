using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries;

public class DbDataEntriesFileNamingService(IProject _project) : FileNamingServiceBase<DataEntriesModel>(_project)
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
