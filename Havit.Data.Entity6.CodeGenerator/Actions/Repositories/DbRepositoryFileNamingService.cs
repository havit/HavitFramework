using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories;

public class DbRepositoryFileNamingService : DbRepositoryGeneratedFileNamingService
{
	public DbRepositoryFileNamingService(IProject project)
		: base(project)
	{

	}

	protected override bool UseGeneratedFolder => false;
}
