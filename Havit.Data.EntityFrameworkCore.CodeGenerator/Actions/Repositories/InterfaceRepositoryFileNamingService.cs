using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories;

public class InterfaceRepositoryFileNamingService : InterfaceRepositoryGeneratedFileNamingService
{
	public InterfaceRepositoryFileNamingService(IDataLayerProject dataLayerProject)
		: base(dataLayerProject)
	{

	}

	protected override bool UseGeneratedFolder => false;
}
