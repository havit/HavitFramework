using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Templates;

public partial class InterfaceRepositoryTemplate : ITemplate
{
	protected RepositoryModel Model { get; private set; }

	public InterfaceRepositoryTemplate(RepositoryModel model)
	{
		Model = model;
	}
}
