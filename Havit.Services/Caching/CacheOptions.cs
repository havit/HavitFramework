using System;

namespace Havit.Services.Caching
{
	/// <summary>
	/// Parametry pro uložení položky v cache.
	/// </summary>
	public class CacheOptions
	{
		/// <summary>
		/// Doba, po které má být položka vyhozena z cache (od okamžiku přidání).
		/// </summary>
		public TimeSpan? AbsoluteExpiration { get; set; }

		/// <summary>
		/// Doba, po které má být položka vyhozena z cache (od posledního přístupu k položce).
		/// </summary>
		public TimeSpan? SlidingExpiration { get; set; }

		/// <summary>
		/// Priorita položky v cache.
		/// </summary>
		public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;

		/// <summary>
		/// Cache dependencies - Klíče, na kterých je položka závislá.
		/// </summary>
		public string[] CacheDependencyKeys { get; set; }
	}
}
