using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata
{
	/// <summary>
	/// Identifikátory konvencí pro možnost potlačení konvencí v modelu.
	/// </summary>
	public static class ConventionIdentifiers
	{
		/// <summary>
		/// Identifikátor konvence CharColumnTypeForCharPropertyConvention.
		/// </summary>
		public const string CharColumnTypeForCharPropertyConvention = nameof(CharColumnTypeForCharPropertyConvention);

		/// <summary>
		/// Identifikátor konvence CollectionExtendedPropertiesConvention.
		/// </summary>
		public const string CollectionExtendedPropertiesConvention = nameof(CollectionExtendedPropertiesConvention);

		/// <summary>
		/// Identifikátor konvence CollectionOrderIndexConvention.
		/// </summary>
		public const string CollectionOrderIndexConvention = nameof(CollectionOrderIndexConvention);

		/// <summary>
		/// Identifikátor konvence ForeignKeysColumnNamesConvention.
		/// </summary>
		public const string ForeignKeysColumnNamesConvention = nameof(ForeignKeysColumnNamesConvention);

		/// <summary>
		/// Identifikátor konvence ForeignKeysIndexConvention.
		/// </summary>
		public const string ForeignKeysIndexConvention = nameof(ForeignKeysIndexConvention);

		/// <summary>
		/// Identifikátor konvence LocalizationTablesParentEntitiesConvention.
		/// </summary>
		public const string LocalizationTablesParentEntitiesConvention = nameof(LocalizationTablesParentEntitiesConvention);

		/// <summary>
		/// Identifikátor konvence NamespaceExtendedPropertyConvention.
		/// </summary>
		public const string NamespaceExtendedPropertyConvention = nameof(NamespaceExtendedPropertyConvention);

		/// <summary>
		/// Identifikátor konvence PrefixedTablePrimaryKeysConvention.
		/// </summary>
		public const string PrefixedTablePrimaryKeysConvention = nameof(PrefixedTablePrimaryKeysConvention);
	}
}
