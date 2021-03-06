﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// Identifikátory konvencí pro možnost potlačení konvencí v modelu.
	/// </summary>
	public static class ConventionIdentifiers
	{
		/// <summary>
		/// Identifikátor konvence ManyToManyEntityKeyDiscoveryConvention.
		/// </summary>
		public const string ManyToManyEntityKeyDiscoveryConvention = nameof(ManyToManyEntityKeyDiscoveryConvention);

		/// <summary>
		/// Identifikátor konvence StringPropertiesDefaultValueConvention.
		/// </summary>
		public const string StringPropertiesDefaultValueConvention = nameof(StringPropertiesDefaultValueConvention);
	}
}
