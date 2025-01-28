using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;

public class DbRepositoryQueryProviderFileGeneratedNamingService : FileNamingServiceBase<RepositoryModel>
{
	public DbRepositoryQueryProviderFileGeneratedNamingService(IDataLayerProject dataLayerProject)
		: base(dataLayerProject)
	{
	}

	protected override string GetClassName(RepositoryModel model)
	{
		return model.RepositoryQueryProviderClassName;
	}

	protected override string GetNamespaceName(RepositoryModel model)
	{
		return model.NamespaceName;
	}
}
