using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories.Templates;

public partial class InterfaceRepositoryGeneratedTemplate : ITemplate
{
	protected RepositoryModel Model { get; private set; }

	public InterfaceRepositoryGeneratedTemplate(RepositoryModel model)
	{
		this.Model = model;
	}
}
