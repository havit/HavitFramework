using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model;
using Havit.Data.Entity.CodeGenerator.Entity;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.QueryableExtensions.Model
{
	public class QueryableExtensionsModelSource : IModelSource<QueryableExtensionsModel>
	{
		private readonly RegisteredEntityEnumerator registeredEntityEnumerator;
		private readonly Project dataLayerProject;

		public QueryableExtensionsModelSource(RegisteredEntityEnumerator registeredEntityEnumerator, Project dataLayerProject)
		{
			this.registeredEntityEnumerator = registeredEntityEnumerator;
			this.dataLayerProject = dataLayerProject;
		}

		public IEnumerable<QueryableExtensionsModel> GetModels()
		{
			yield return new QueryableExtensionsModel()
			{
				NamespaceName = dataLayerProject.GetProjectRootNamespace(),
				ModelClassesFullNames = registeredEntityEnumerator.GetRegisteredEntities().Select(item => item.FullName).ToList()
			};
		}
	}
}
