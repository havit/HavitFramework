using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories
{
	public class DbRepositoryGeneratedFileNamingService : FileNamingServiceBase<RepositoryModel>
	{
		public DbRepositoryGeneratedFileNamingService(IProject project)
			: base(project)
		{
			
		}

		protected override string GetClassName(RepositoryModel model)
		{
			return model.DbRepositoryName;
		}

		protected override string GetNamespaceName(RepositoryModel model)
		{
			return model.NamespaceName;
		}
	}
}
