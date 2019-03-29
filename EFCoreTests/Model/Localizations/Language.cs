using System;
using System.ComponentModel.DataAnnotations;
using Havit.Data.EntityFrameworkCore.Attributes;
using Havit.Model.Localizations;

namespace Havit.EFCoreTests.Model.Localizations
{
	[CacheAttribute(SlidingExpirationSeconds = 60, CacheAllKeys = false, AbsoluteExpirationSeconds = 5, Priority = CacheItemPriority.High)]
	public class Language : ILanguage
	{
		public int Id { get; set; }

		[MaxLength(200)]
		public string Name { get; set; }
		
		[MaxLength(10)]
		public string Culture { get; set; }
		
		[MaxLength(10)]
		public string UiCulture { get; set; }

		[MaxLength(50)]
		public string Symbol { get; set; }
	}
}
