using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataSources.Template;

public partial class FakeDataSourceTemplate : ITemplate
{
	protected FakeDataSourceModel Model { get; private set; }

	public FakeDataSourceTemplate(FakeDataSourceModel model)
	{
		Model = model;
	}
}
