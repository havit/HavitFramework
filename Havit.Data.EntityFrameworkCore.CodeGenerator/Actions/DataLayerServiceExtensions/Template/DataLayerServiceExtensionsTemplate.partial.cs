using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataLayerServiceExtensions.Template;

public partial class DataLayerServiceExtensionsTemplate : ITemplate
{
	protected DataLayerServiceExtensionsModel Model { get; private set; }

	public DataLayerServiceExtensionsTemplate(DataLayerServiceExtensionsModel model)
	{
		Model = model;
	}
}
