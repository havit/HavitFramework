using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries;

public class InterfaceDataEntriesFileNamingService(IProject _project) : FileNamingServiceBase<DataEntriesModel>(_project)
{
	protected override string GetClassName(DataEntriesModel model)
	{
		return model.InterfaceName;
	}

	protected override string GetNamespaceName(DataEntriesModel model)
	{
		return model.NamespaceName;
	}
}
