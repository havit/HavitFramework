using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Business.Caching;
using Havit.Diagnostics.Contracts;
using Havit.Services.Caching;

namespace Havit.Business
{
	/// <summary>
	/// Ambientní kontext pro Business Layer.
	/// </summary>
	public static class BusinessLayerContexts
	{
		#region BusinessLayerCacheService		
		/// <summary>
		/// Služba pro práci s cache v Business Layer.
		/// </summary>
		public static IBusinessLayerCacheService BusinessLayerCacheService
		{
			get
			{
				if (_businessLayerCacheService == null)
				{
					_businessLayerCacheService = new DefaultBusinessLayerCacheService(new HttpRuntimeCacheService());
				}
				return _businessLayerCacheService;
			}
		}

		private static IBusinessLayerCacheService _businessLayerCacheService;

		/// <summary>
		/// Nastavuje služba pro práci s cache v Business Layer.
		/// </summary>
		public static void SetBusinessLayerCacheService(IBusinessLayerCacheService businessLayerCacheService)
		{
			Contract.Requires<ArgumentNullException>(businessLayerCacheService != null);

			_businessLayerCacheService = businessLayerCacheService;
		}
		#endregion
	}
}
