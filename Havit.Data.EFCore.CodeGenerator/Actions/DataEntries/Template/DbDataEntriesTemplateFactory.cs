using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Template
{
	public class DbDataEntriesTemplateFactory : ITemplateFactory<DataEntriesModel>
	{
		public ITemplate CreateTemplate(DataEntriesModel model)
		{
			return new DbDataEntriesTemplate(model);
		}
	}
}
