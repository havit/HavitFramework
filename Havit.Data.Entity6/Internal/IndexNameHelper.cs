using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Internal
{
	/// <summary>
	/// Pomocné metody pro pojmenování indexů.
	/// </summary>
	internal static class IndexNameHelper
	{
		private static int indexNameMaxLengthExceededCounter = 0;

		/// <summary>
		/// Vygeneruje název indexu pro index obsahující zadané sloupce a volitelnou unikánost hodnot.
		/// </summary>
		public static string GetIndexName(EdmProperty[] columns, bool unique)
		{
			Contract.Requires(columns != null);
			Contract.Requires(columns.Length > 0);

			StringBuilder indexNameBuilder = new StringBuilder();
			if (unique)
			{
				indexNameBuilder.Append("U");
			}
			indexNameBuilder.Append("IDX_");
            indexNameBuilder.Append(columns.First().DeclaringType.Name);

			foreach (EdmProperty property in columns)
			{
				indexNameBuilder.Append("_");
				indexNameBuilder.Append(property.Name);
			}

			if (indexNameBuilder.Length <= 128)
			{
				return indexNameBuilder.ToString();
			}
			else
			{
				indexNameMaxLengthExceededCounter += 1;
				string suffix = indexNameMaxLengthExceededCounter.ToString("D4");
				return indexNameBuilder.ToString().Left(128 - suffix.Length) + suffix;
			}
		}
	}
}