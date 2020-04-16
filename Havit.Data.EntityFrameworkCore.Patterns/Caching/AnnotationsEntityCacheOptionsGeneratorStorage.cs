using Havit.Services.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Úložiště CacheOptions k jednotlivým entitám
	/// </summary>
	public class AnnotationsEntityCacheOptionsGeneratorStorage : IAnnotationsEntityCacheOptionsGeneratorStorage
	{
		/// <summary>
		/// Úložiště CacheOptions k jednotlivým entitám
		/// </summary>
		public Dictionary<Type, CacheOptions> Value { get; set; }
	}
}
