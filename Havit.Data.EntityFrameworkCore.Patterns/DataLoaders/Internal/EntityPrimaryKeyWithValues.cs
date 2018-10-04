using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal
{
	/// <summary>
	/// Název a hodnoty vlastnosti primárního klíče.
	/// </summary>
	public class EntityPrimaryKeyWithValues
	{
		/// <summary>
		/// Název property primárního klíče.
		/// </summary>
		public string PrimaryKeyName { get; set; }

		/// <summary>
		/// Hodnoty vlastnosti primárního klíče.
		/// </summary>
		public List<int> Values { get; set; }
	}
}