using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Templates;

public partial class DbRepositoryBaseGeneratedTemplate : ITemplate
{
	protected RepositoryModel Model { get; private set; }

	public DbRepositoryBaseGeneratedTemplate(RepositoryModel model)
	{
		this.Model = model;
	}
}
