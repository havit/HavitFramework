using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Template;

public partial class QueryableExtensionsTemplate : ITemplate
{
	protected QueryableExtensionsModel Model { get; private set; }

	public QueryableExtensionsTemplate(QueryableExtensionsModel model)
	{			
		this.Model = model;
	}
}
