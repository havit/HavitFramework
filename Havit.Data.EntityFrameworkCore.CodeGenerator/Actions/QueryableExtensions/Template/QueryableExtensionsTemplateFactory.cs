using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Template
{
	public class QueryableExtensionsTemplateFactory : ITemplateFactory<QueryableExtensionsModel>
	{
		public ITemplate CreateTemplate(QueryableExtensionsModel model)
		{
			return new QueryableExtensionsTemplate(model);
		}
	}

}
