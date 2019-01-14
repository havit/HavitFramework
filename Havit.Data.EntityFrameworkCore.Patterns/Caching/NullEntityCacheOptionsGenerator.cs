using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Services;
using Havit.Services.Caching;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Vrací vždy null. Pro použití tam, kde nepotřebujeme řešit CacheOptions, např. v unit testech.
	/// </summary>
	public class NullEntityCacheOptionsGenerator : IEntityCacheOptionsGenerator
	{
		/// <inheritdoc />
		public CacheOptions GetEntityCacheOptions<TEntity>(TEntity entity)
			where TEntity : class
		{
			return null;
		}

		/// <inheritdoc />
		public CacheOptions GetAllKeysCacheOptions<TEntity>()
			where TEntity : class
		{
			return null;
		}

	}
}
