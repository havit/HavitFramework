using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources;

public class FakeDataSourceFileNamingService : FileNamingServiceBase<FakeDataSourceModel>
{
	public FakeDataSourceFileNamingService(IDataLayerProject dataLayerProject)
		: base(dataLayerProject)
	{

	}

	protected override string GetClassName(FakeDataSourceModel model)
	{
		return model.FakeDataSourceClassName;
	}

	protected override string GetNamespaceName(FakeDataSourceModel model)
	{
		return model.NamespaceName;
	}
}
