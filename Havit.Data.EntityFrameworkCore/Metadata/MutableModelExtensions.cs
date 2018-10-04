using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata;
using Havit.Linq;

namespace Havit.Data.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// Extension metody k IMutableModel.
	/// </summary>
	public static class MutableModelExtensions
	{
		/// <summary>
		/// Vrací entity, které nejsou systémové, nejsou Owned a nejsou QueryType.
		/// </summary>
		public static IEnumerable<IMutableEntityType> GetApplicationEntityTypes(this IMutableModel model, bool includeManyToManyEntities = true)
		{
			return ((IModel)model).GetApplicationEntityTypes(includeManyToManyEntities: includeManyToManyEntities).Cast<IMutableEntityType>();
		}
	}
}
