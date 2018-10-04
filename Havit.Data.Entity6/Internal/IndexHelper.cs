using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using Havit.Data.Entity.ModelConfiguration.Edm;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Internal
{
	/// <summary>
	/// Pomocné metody pro přidání indexu na úrovni EdmProperties.
	/// </summary>
	internal static class IndexHelper
	{
		/// <summary>
		/// Přidá index nad jedním sloupcem.
		/// </summary>
		public static void AddIndex(EdmProperty column, bool unique = false)
		{
			Contract.Requires(column != null);

			AddIndex(new EdmProperty[] { column }, unique);
		}

		/// <summary>
		/// Přidá index nad množinou sloupců (v pořadí).
		/// </summary>
		public static void AddIndex(EdmProperty[] columns, bool unique = false)
		{
			Contract.Requires(columns != null);
			Contract.Requires(columns.Length > 0);

			string indexName = IndexNameHelper.GetIndexName(columns, unique);

			int order = 0;
			foreach (EdmProperty property in columns)
			{
				order += 1;
				AddIndexAnnotation(property, indexName, order, unique);
			}
		}

		/// <summary>
		/// Přidá ke sloupci anotaci pro přidání indexu.
		/// </summary>
		private static void AddIndexAnnotation(EdmProperty property, string indexName, int indexOrder, bool indexIsUnique = false)
		{
			IndexAnnotation indexAnnotation = new IndexAnnotation(new IndexAttribute(indexName, indexOrder) { IsUnique = indexIsUnique });

			object annotation = property.GetAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:Index");

			if (annotation != null)
			{
				indexAnnotation = (IndexAnnotation)((IndexAnnotation)annotation).MergeWith(indexAnnotation);
			}

			property.RemoveAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:Index");
			property.AddAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:Index", indexAnnotation);
		}
	}
}
