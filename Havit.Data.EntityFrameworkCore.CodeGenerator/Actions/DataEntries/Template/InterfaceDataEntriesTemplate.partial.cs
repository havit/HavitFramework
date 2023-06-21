using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Template;

public partial class InterfaceDataEntriesTemplate : ITemplate
{
	protected DataEntriesModel Model { get; private set; }

	public InterfaceDataEntriesTemplate(DataEntriesModel model)
	{
		this.Model = model;
	}
}
