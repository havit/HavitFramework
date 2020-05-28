using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Collections
{
	/// <summary>
	/// Extension metody k SortDirection.
	/// </summary>
	public static class SortDirectionExtensions
	{
		/// <summary>
		/// Vrátí opačný směr řazení.
		/// </summary>
		public static SortDirection Reverse(this SortDirection source)
		{
			return source switch
			{
				SortDirection.Ascending => SortDirection.Descending,
				SortDirection.Descending => SortDirection.Ascending,
				_ => throw new InvalidOperationException(source.ToString())
			};
		}
	}
}
