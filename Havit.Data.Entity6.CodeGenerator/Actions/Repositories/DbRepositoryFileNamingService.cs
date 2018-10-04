using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories
{
	public class DbRepositoryFileNamingService : DbRepositoryGeneratedFileNamingService
	{
		public DbRepositoryFileNamingService(Project project)
			: base(project)
		{
			
		}

		protected override bool UseGeneratedFolder => false;
	}
}
