using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Template
{
	public class InterfaceDataEntriesTemplateFactory : ITemplateFactory<DataEntriesModel>
	{
		public ITemplate CreateTemplate(DataEntriesModel model)
		{
			return new InterfaceDataEntriesTemplate(model);
		}
	}

}
