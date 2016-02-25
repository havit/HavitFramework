using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataEntries
{
	public class DbDataEntriesFileNamingService : FileNamingServiceBase<DataEntriesModel>
	{
		public DbDataEntriesFileNamingService(Project project)
			: base(project)
		{
			
		}

		protected override string GetClassName(DataEntriesModel model)
		{
			return model.DbClassName;
		}

		protected override string GetNamespaceName(DataEntriesModel model)
		{
			return model.NamespaceName;
		}
	}
}
