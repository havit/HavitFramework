using System;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata;

/// <summary>
/// Extensinsion metody k EntityType s fakty pro oblast (tabulku) Language v Business Layer.
/// </summary>
public static class LanguageEntityTypeExtensions
{
	/// <summary>
	/// Název tabulky s jazyky.
	/// </summary>
	private const string LanguageEntityName = "Language";

	/// <summary>
	/// Indikuje, zda jde o tabulku s jazyky (se seznamem jazyků).
	/// </summary>
	public static bool IsBusinessLayerLanguageEntity(this IReadOnlyEntityType entityType)
	{
		Contract.Requires<ArgumentNullException>(entityType != null);

		return entityType.ClrType?.Name == LanguageEntityName;
	}

	/// <summary>
	/// Indikuje, zda jde o sloupec UiCulture v tabulce jazyků.
	/// </summary>
	public static IReadOnlyProperty GetBusinessLayerUICultureProperty(this IReadOnlyEntityType entityType)
	{
		Contract.Requires<ArgumentNullException>(entityType != null);

		if (!entityType.IsBusinessLayerLanguageEntity())
		{
			return null;
		}

		return entityType.FindProperty("UiCulture");
	}
}