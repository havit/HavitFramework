using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.QueryableExtensions.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.QueryableExtensions.Template
{
	public class QueryableExtensionsTemplateFactory : ITemplateFactory<QueryableExtensionsModel>
	{
		public ITemplate CreateTemplate(QueryableExtensionsModel model)
		{
			return new QueryableExtensionsTemplate(model);
		}
	}

}
