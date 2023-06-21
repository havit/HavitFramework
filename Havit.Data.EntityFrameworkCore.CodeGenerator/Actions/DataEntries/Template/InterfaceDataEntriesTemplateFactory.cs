using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Template;

public class InterfaceDataEntriesTemplateFactory : ITemplateFactory<DataEntriesModel>
{
	public ITemplate CreateTemplate(DataEntriesModel model)
	{
		return new InterfaceDataEntriesTemplate(model);
	}
}
