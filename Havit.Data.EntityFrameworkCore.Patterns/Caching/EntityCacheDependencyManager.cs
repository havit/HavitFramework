using Havit.Diagnostics.Contracts;
using Havit.Services.Caching;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Služba pro poskytnutí strinkových klíčů do cache.
	/// </summary>
	public class EntityCacheDependencyManager : IEntityCacheDependencyManager
	{
		private readonly ICacheService cacheService;
		private static readonly object StaticCacheValue = new object();

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public EntityCacheDependencyManager(ICacheService cacheService)
		{
			this.cacheService = cacheService;
		}

		/// <inheritdoc />
		public string GetAnySaveCacheDependencyKey(Type entityType, bool ensureInCache = true)
		{
			EnsureSupportsCacheDependencies();
			string dependencyKey = entityType + "|AnySave";
			if (ensureInCache)
			{
				EnsureInCache(dependencyKey);
			}
			return dependencyKey;
		}

		/// <inheritdoc />
		public string GetSaveCacheDependencyKey(Type entityType, object key, bool ensureInCache = true)
		{
			EnsureSupportsCacheDependencies();
			string dependencyKey = entityType + "|Save|ID=" + key.ToString();
			if (ensureInCache)
			{
				EnsureInCache(dependencyKey);
			}
			return dependencyKey;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureSupportsCacheDependencies()
		{
			Contract.Assert<InvalidOperationException>(cacheService.SupportsCacheDependencies, "Dependency keys can be generated only for ICacheService whitch supports cache dependencies.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureInCache(string dependencyKey)
		{
			// musíme se nejprve zeptat, zda exisuje
			// kdybychom se nezeptali a došlo jen k Addu, invalidoval by se dosavadní klíč a vzal by s sebou své závislosti.
			if (!cacheService.Contains(dependencyKey))
			{
				cacheService.Add(dependencyKey, StaticCacheValue);
			}
		}
	}
}
