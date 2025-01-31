using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories.Templates;

public partial class DbRepositoryBaseGeneratedTemplate : ITemplate
{
	protected RepositoryModel Model { get; private set; }

	public DbRepositoryBaseGeneratedTemplate(RepositoryModel model)
	{
		this.Model = model;
	}
}
