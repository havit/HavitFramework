using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories;

public class InterfaceRepositoryGeneratedFileNamingService : FileNamingServiceBase<RepositoryModel>
{
	public InterfaceRepositoryGeneratedFileNamingService(IProject project)
		: base(project)
	{

	}

	protected override string GetClassName(RepositoryModel model)
	{
		return model.InterfaceRepositoryName;
	}

	protected override string GetNamespaceName(RepositoryModel model)
	{
		return model.NamespaceName;
	}
}
