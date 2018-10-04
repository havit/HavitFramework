using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataEntries
{
	public class InterfaceDataEntriesFileNamingService : FileNamingServiceBase<DataEntriesModel>
	{
		public InterfaceDataEntriesFileNamingService(Project project)
			: base(project)
		{
			
		}

		protected override string GetClassName(DataEntriesModel model)
		{
			return model.InterfaceName;
		}

		protected override string GetNamespaceName(DataEntriesModel model)
		{
			return model.NamespaceName;
		}
	}
}
