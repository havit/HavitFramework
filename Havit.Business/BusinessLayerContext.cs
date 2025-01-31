﻿using Havit.Business.Caching;
using Havit.Diagnostics.Contracts;
using Havit.Services.Caching;

namespace Havit.Business;

/// <summary>
/// Ambientní kontext pro Business Layer.
/// </summary>
public static class BusinessLayerContext
{
	/// <summary>
	/// Služba pro práci s cache v Business Layer.
	/// </summary>
	public static IBusinessLayerCacheService BusinessLayerCacheService
	{
		get
		{
			if (_businessLayerCacheServiceFunc != null)
			{
				return _businessLayerCacheServiceFunc();
			}

			// zpětná kompatibilita
			if (_defaultBusinessLayerCacheService == null)
			{
#if NETFRAMEWORK
				_defaultBusinessLayerCacheService = new DefaultBusinessLayerCacheService(new HttpRuntimeCacheService());
#else
				_defaultBusinessLayerCacheService = new DefaultBusinessLayerCacheService(new ObjectCacheService(System.Runtime.Caching.MemoryCache.Default));
#endif
			}

			return _defaultBusinessLayerCacheService;
		}
	}

	private static Func<IBusinessLayerCacheService> _businessLayerCacheServiceFunc;
	private static IBusinessLayerCacheService _defaultBusinessLayerCacheService;

	/// <summary>
	/// Nastavuje služba pro práci s cache v Business Layer.
	/// </summary>
	public static void SetBusinessLayerCacheService(IBusinessLayerCacheService businessLayerCacheService)
	{
		SetBusinessLayerCacheService(() => businessLayerCacheService);
	}

	/// <summary>
	/// Nastavuje služba pro práci s cache v Business Layer.
	/// </summary>
	public static void SetBusinessLayerCacheService(Func<IBusinessLayerCacheService> businessLayerCacheServiceFunc)
	{
		Contract.Requires<ArgumentNullException>(businessLayerCacheServiceFunc != null);

		_businessLayerCacheServiceFunc = businessLayerCacheServiceFunc;
	}
}
