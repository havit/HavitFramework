using Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories.Templates;

public class InterfaceRepositoryGeneratedTemplateFactory : ITemplateFactory<RepositoryModel>
{
	public ITemplate CreateTemplate(RepositoryModel model)
	{
		return new InterfaceRepositoryGeneratedTemplate(model);
	}
}
