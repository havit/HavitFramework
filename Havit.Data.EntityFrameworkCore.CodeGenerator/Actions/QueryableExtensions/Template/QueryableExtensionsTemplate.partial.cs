using Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.QueryableExtensions.Model;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.QueryableExtensions.Template
{
	public partial class QueryableExtensionsTemplate : ITemplate
	{
		protected QueryableExtensionsModel Model { get; private set; }

		public QueryableExtensionsTemplate(QueryableExtensionsModel model)
		{			
			this.Model = model;
		}
	}
}
