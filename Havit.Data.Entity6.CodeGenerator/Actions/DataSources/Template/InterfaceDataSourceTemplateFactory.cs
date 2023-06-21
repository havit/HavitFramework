using Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources.Template;

public class InterfaceDataSourceTemplateFactory : ITemplateFactory<InterfaceDataSourceModel>
{
	public ITemplate CreateTemplate(InterfaceDataSourceModel model)
	{
		return new InterfaceDataSourceTemplate(model);
	}
}
