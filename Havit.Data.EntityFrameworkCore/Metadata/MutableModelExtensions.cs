using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// Extension metody k IMutableModel.
	/// </summary>
	public static class MutableModelExtensions
	{
		/// <summary>
		/// Vrací entity (včetně M:N), které nejsou systémové.
		/// </summary>
		public static IEnumerable<IMutableEntityType> GetEntityTypesExcludingSystemTypes(this IMutableModel model)
		{
			return model.GetEntityTypes().Where(entityType => !entityType.IsSystemType());
		}
	}
}
