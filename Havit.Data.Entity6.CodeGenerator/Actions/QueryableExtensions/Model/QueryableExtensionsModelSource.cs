using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Entity;
using Havit.Data.Entity.CodeGenerator.Services;
using Havit.Data.Entity.Mapping.Internal;

namespace Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Model
{
	public class QueryableExtensionsModelSource : IModelSource<QueryableExtensionsModel>
	{
		private readonly DbContext dbContext;
		private readonly Project dataLayerProject;

		public QueryableExtensionsModelSource(DbContext dbContext, Project dataLayerProject)
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
}
