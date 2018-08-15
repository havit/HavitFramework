using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Template
{
	public class InterfaceDataSourceTemplateFactory : ITemplateFactory<InterfaceDataSourceModel>
	{
		public ITemplate CreateTemplate(InterfaceDataSourceModel model)
		{
			return new InterfaceDataSourceTemplate(model);
		}
	}

}
