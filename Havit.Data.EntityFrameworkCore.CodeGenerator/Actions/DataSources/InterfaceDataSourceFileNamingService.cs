using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources;

public class InterfaceDataSourceFileNamingService : FileNamingServiceBase<InterfaceDataSourceModel>
{
	public InterfaceDataSourceFileNamingService(IDataLayerProject dataLayerProject)
		: base(dataLayerProject)
	{

	}

	protected override string GetClassName(InterfaceDataSourceModel model)
	{
		return model.InterfaceDataSourceName;
	}

	protected override string GetNamespaceName(InterfaceDataSourceModel model)
	{
		return model.NamespaceName;
	}
}
