using System;
using System.Collections.Generic;
using System.Text;
using Havit.Services;
using Havit.Services.Caching;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Strategie definující, zda může být entita cachována. Řídí se anotacemi, ke kterým doplňuje výchozí hodnoty.
	/// </summary>	
	public class AnnotationsWithDefaultsEntityCacheOptionsGenerator : AnnotationsEntityCacheOptionsGenerator
	{
		private readonly TimeSpan? absoluteExpiration;
		private readonly TimeSpan? slidingExpiration;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AnnotationsWithDefaultsEntityCacheOptionsGenerator(IDbContextFactory dbContextFactory, TimeSpan absoluteExpiration, TimeSpan slidingExpiration) : base(dbContextFactory)
		{
			this.absoluteExpiration = (absoluteExpiration == TimeSpan.Zero) ? (TimeSpan?)null : absoluteExpiration;
			this.slidingExpiration = (slidingExpiration == TimeSpan.Zero) ? (TimeSpan?)null : slidingExpiration;
		}

		/// <inheritdoc />
		protected override CacheOptions GetCacheOptions(IEntityType entityType)
		{
			CacheOptions result = base.GetCacheOptions(entityType);
			
			if (result == null)
			{
				result = new CacheOptions();
			}

			if (result.AbsoluteExpiration == null)
			{
				result.SlidingExpiration = absoluteExpiration;
			}

			if (result.SlidingExpiration == null)
			{
				result.SlidingExpiration = slidingExpiration;
			}

			return result;
		}
	}
}
