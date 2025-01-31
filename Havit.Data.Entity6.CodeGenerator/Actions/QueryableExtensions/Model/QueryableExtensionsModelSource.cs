using Havit.Data.Entity.CodeGenerator.Services;
using Havit.Data.Entity.Mapping.Internal;

namespace Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Model;

public class QueryableExtensionsModelSource : IModelSource<QueryableExtensionsModel>
{
	private readonly DbContext dbContext;
	private readonly IProject dataLayerProject;

	public QueryableExtensionsModelSource(DbContext dbContext, IProject dataLayerProject)
	{
		this.dbContext = dbContext;
		this.dataLayerProject = dataLayerProject;
	}

	public IEnumerable<QueryableExtensionsModel> GetModels()
	{
		yield return new QueryableExtensionsModel()
		{
			NamespaceName = dataLayerProject.GetProjectRootNamespace(),
			ModelClassesFullNames = dbContext.GetRegisteredEntities().Select(item => item.FullName).ToList()
		};
	}
}
