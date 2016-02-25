using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories
{
	public class DbRepositoryBaseFileNamingService : FileNamingServiceBase<RepositoryModel>
	{
		public DbRepositoryBaseFileNamingService(Project project)
			: base(project)
		{
			
		}

		protected override string GetClassName(RepositoryModel model)
		{
			return model.DbRepositoryBaseName;
		}

		protected override string GetNamespaceName(RepositoryModel model)
		{
			return model.NamespaceName;
		}
	}
}
