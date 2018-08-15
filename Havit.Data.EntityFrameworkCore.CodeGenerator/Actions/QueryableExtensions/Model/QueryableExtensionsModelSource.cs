using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.QueryableExtensions.Model
{
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
				ModelClassesFullNames = dbContext.Model.GetEntityTypes().Select(item => item.ClrType.FullName).ToList()
			};
		}
	}
}
