using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore
{
	/// <summary>
	/// Nastavení DbContextu.
	/// </summary>
	public class DbContextSettings
	{
		/// <summary>
		/// Indikuje používání konvence CascadeDeleteToRestrictConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseCascadeDeleteToRestrictConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence CacheAttributeToAnnotationConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseCacheAttributeToAnnotationConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence DataTypeAttributeConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseDataTypeAttributeConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence StringPropertiesDefaultValueConvention.
		/// Výchozí hodnota je false.
		/// </summary>
		public bool UseStringPropertiesDefaultValueConvention { get; set; } = false; // by default vypnuto

		/// <summary>
		/// Indikuje používání konvence ManyToManyEntityKeyDiscoveryConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseManyToManyEntityKeyDiscoveryConvention { get; set; } = true;
	}
}
