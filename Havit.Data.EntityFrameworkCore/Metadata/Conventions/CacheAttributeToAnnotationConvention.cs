﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Data.EntityFrameworkCore.Attributes;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Konvence nastaví hodnoty z CacheAttribute do anotací.
	/// </summary>
	public class CacheAttributeToAnnotationConvention : EntityTypeAttributeConventionBase<CacheAttribute>
	{
		/// <summary>
		/// Název anotace určující, zda je na povoleno cachování entity.
		/// </summary>
		public const string CacheEntitiesAnnotationName = "Caching-EntitiesEnabled";

		/// <summary>
		/// Název anotace určující, zda je na povoleno cachování AllKeys.
		/// </summary>
		public const string CacheAllKeysAnnotationName = "Caching-AllKeysEnabled";

		/// <summary>
		/// Název anotace určující nastavení sliding expirace.
		/// </summary>
		public const string SlidingExpirationAnnotationName = "Caching-SlidingExpiration";

		/// <summary>
		/// Název anotace určující nastavení absolute expirace.
		/// </summary>
		public const string AbsoluteExpirationAnnotationName = "Caching-AbsoluteExpiration";

		/// <summary>
		/// Název anotace určující nastavení priority položek v cache.
		/// </summary>
		public const string PriorityAnnotationName = "Caching-Priority";

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public CacheAttributeToAnnotationConvention(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
		{
		}

		/// <inheritdoc />
		protected override void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, CacheAttribute attribute, IConventionContext<IConventionEntityTypeBuilder> context)
		{
			if (attribute.CacheEntities)
			{
				entityTypeBuilder.HasAnnotation(CacheEntitiesAnnotationName, true, fromDataAnnotation: true);
			}

			if (attribute.CacheAllKeys)
			{
				entityTypeBuilder.HasAnnotation(CacheAllKeysAnnotationName, true, fromDataAnnotation: true);
			}

			if (attribute.AbsoluteExpirationSeconds != 0)
			{
				entityTypeBuilder.HasAnnotation(AbsoluteExpirationAnnotationName, attribute.AbsoluteExpirationSeconds, fromDataAnnotation: true);
			}

			if (attribute.SlidingExpirationSeconds != 0)
			{
				entityTypeBuilder.HasAnnotation(AbsoluteExpirationAnnotationName, attribute.SlidingExpirationSeconds, fromDataAnnotation: true);
			}

			if (attribute.Priority != CacheItemPriority.Normal)
			{
				entityTypeBuilder.HasAnnotation(PriorityAnnotationName, attribute.Priority, fromDataAnnotation: true);
			}
		}
	}
}
