using Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources
{
	public class DbDataSourceFileNamingService : FileNamingServiceBase<DbDataSourceModel>
	{
		public DbDataSourceFileNamingService(Project project)
			: base(project)
		{
			
		}

		protected override string GetClassName(DbDataSourceModel model)
		{
			return model.DbDataSourceClassName;
		}

		protected override string GetNamespaceName(DbDataSourceModel model)
		{
			return model.NamespaceName;
		}
	}
}
