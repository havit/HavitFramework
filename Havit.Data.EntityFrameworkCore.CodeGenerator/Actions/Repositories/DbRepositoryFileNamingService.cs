using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;

public class DbRepositoryFileNamingService : DbRepositoryGeneratedFileNamingService
{
	public DbRepositoryFileNamingService(IProject project)
		: base(project)
	{

	}

	protected override bool UseGeneratedFolder => false;
}
