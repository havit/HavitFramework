using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Template;

public class FakeDataSourceTemplateFactory : ITemplateFactory<FakeDataSourceModel>
{
	public ITemplate CreateTemplate(FakeDataSourceModel model)
	{
		return new FakeDataSourceTemplate(model);
	}
}
