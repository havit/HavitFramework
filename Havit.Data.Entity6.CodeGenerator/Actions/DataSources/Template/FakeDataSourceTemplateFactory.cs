using Havit.Data.Entity.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataSources.Template
{
	public class FakeDataSourceTemplateFactory : ITemplateFactory<FakeDataSourceModel>
	{
		public ITemplate CreateTemplate(FakeDataSourceModel model)
		{
			return new FakeDataSourceTemplate(model);
		}
	}

}
