using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions;

public class QueryableExtensionsFileNamingService : FileNamingServiceBase<QueryableExtensionsModel>
{
	public QueryableExtensionsFileNamingService(IProject project)
		: base(project)
	{
		
	}

	protected override string GetClassName(QueryableExtensionsModel model)
	{
		return "QueryableExtensions";
	}

	protected override string GetNamespaceName(QueryableExtensionsModel model)
	{
		return model.NamespaceName;
	}
}
