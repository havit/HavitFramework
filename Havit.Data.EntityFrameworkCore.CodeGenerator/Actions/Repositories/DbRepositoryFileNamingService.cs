using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;

public class DbRepositoryFileNamingService : DbRepositoryGeneratedFileNamingService
{
	public DbRepositoryFileNamingService(IDataLayerProject dataLayerProject)
		: base(dataLayerProject)
	{

	}

	protected override bool UseGeneratedFolder => false;
}
