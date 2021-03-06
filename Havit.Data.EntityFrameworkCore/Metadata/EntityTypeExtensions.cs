﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// Extension metody k IEntityType.
	/// </summary>
    public static class EntityTypeExtensions
    {
	    /// <summary>
	    /// Vrací true, pokud jde o systémovou entitu, tj. entitu zaregistrovanou EF automaticky.
	    /// </summary>
	    public static bool IsSystemType(this IEntityType entityType)
	    {
			return (entityType.ClrType == typeof(Havit.Data.EntityFrameworkCore.Model.DataSeedVersion))
				|| (entityType.ClrType == typeof(Microsoft.EntityFrameworkCore.Migrations.HistoryRow));
		}

		internal static bool HasExactlyTwoNotNullablePropertiesWhichAreAlsoForeignKeys(this IEntityType entityType)
		{
			return (entityType.GetProperties().Count() == 2) // třída má právě dvě (skalární) vlastnosti
				&& (entityType.GetProperties().All(item => !item.IsNullable))
				&& (entityType.GetProperties().All(item => !item.IsShadowProperty())) // jen pro jistotu
				&& (entityType.GetProperties().All(item => item.IsForeignKey())); // všechny vlastnosti třídy jsou cizím klíčem
		}

		/// <summary>
		/// Vrací true, pokud je entita vztahovou entitou M:N vztahu.
		/// </summary>
		public static bool IsManyToManyEntity(this IEntityType entityType)
	    {
			// GetProperties neobsahuje vlastnosti z nadřazených tříd, v tomto scénáři to nevadí, dědičnost pro tabulky se dvěma sloupci primárního klíče neuvažujeme
			return !entityType.IsOwned()
				&& !entityType.IsKeyless()
				&& (entityType.FindPrimaryKey()?.Properties.Count == 2) // třída má složený primární klíč ze svou vlastností
				&& HasExactlyTwoNotNullablePropertiesWhichAreAlsoForeignKeys(entityType); // třída má právě dvě (skalární) vlastnosti a ty jsou i cizím klíčem
		}

		/// <summary>
		/// Vrací true, pokud jde o aplikační entitu - není systémová, nejde o QueryType a není Owned.
		/// </summary>
		internal static bool IsApplicationEntity(this IEntityType entityType)
		{
			return !entityType.IsKeyless()
				&& !entityType.IsOwned()
				&& !entityType.IsSystemType()
				// Pokud není k dispozici anotace IsApplicationEntity, pak považujeme entitu za aplikační entitu (bool? != false),
				// pokud je anotace k dispozici, vezmeme její hodnotu.
				// Cílem je umožnit aplikačně vyjmout některé entity, které sdílejí stejný DbContext (např. entity IdentityServeru).
				// Anotace nepřebíjí ostatní podmínky kladené na aplikační entity (viz výše - IsKeyLess, atp.).
				&& ((bool?)entityType.FindAnnotation(ApplicationEntityAnnotationConstants.IsApplicationEntityAnnotationName)?.Value != false); // lze zapsat .GetValueOrDefault(true), ale nechávám záměrně s != false
		}

		/// <summary>
		/// Vrací true, pokud jde o keyless type (dříve QueryType).
		/// IsQueryType (&lt; EF Core 3.0) bylo publikováno v interface IEntityType, IsKeyless se v interface nenachází a je pouze v implementaci EntityType;
		/// </summary>
		public static bool IsKeyless(this IEntityType entityType)
		{
			if (entityType is Microsoft.EntityFrameworkCore.Metadata.Internal.EntityType entityTypeCasted)
			{
#pragma warning disable EF1001 // Internal EF Core API usage.
				return entityTypeCasted.IsKeyless;
#pragma warning restore EF1001 // Internal EF Core API usage.
			}
			return false;
		}
	}
}
