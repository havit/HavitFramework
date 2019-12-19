using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Konfigurace pro AnnotationsWithDefaultsEntityCacheOptionsGenerator.
	/// </summary>
	public class AnnotationsWithDefaultsEntityCacheOptionsGeneratorOptions
	{
		/// <summary>
		/// Čas absolutní expirace.
		/// </summary>
		public TimeSpan? AbsoluteExpiration { get; set; }

		/// <summary>
		/// Čas sliding expirace.
		/// </summary>
		public TimeSpan? SlidingExpiration { get; set; }
	}
}
