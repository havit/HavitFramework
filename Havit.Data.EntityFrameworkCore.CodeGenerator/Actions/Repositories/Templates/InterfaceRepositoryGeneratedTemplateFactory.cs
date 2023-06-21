using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.Repositories.Templates;

public class InterfaceRepositoryGeneratedTemplateFactory : ITemplateFactory<RepositoryModel>
{
	public ITemplate CreateTemplate(RepositoryModel model)
	{
		return new InterfaceRepositoryGeneratedTemplate(model);
	}
}
