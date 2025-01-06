using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;

public class InterfaceRepositoryFileNamingService : InterfaceRepositoryGeneratedFileNamingService
{
	public InterfaceRepositoryFileNamingService(IProject project)
		: base(project)
	{

	}

	protected override bool UseGeneratedFolder => false;
}
