using System;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

/// <summary>
/// Reprezentuje změnu v datech.
/// </summary>
public abstract class Change
{
	/// <summary>
	/// Typ změny.
	/// </summary>
	public required ChangeType ChangeType { get; init; }

	/// <summary>
	/// Měněná entita.
	/// </summary>
	public required object Entity { get; init; }

	/// <summary>
	/// Typ měněné entity, pokud jde o entitu.
	/// Pokud jde o objekt reprezentující many-to-many vztah (ve variantě skip navigation), je hodnota null.
	/// </summary>
	/// <remarks>
	/// Byť bychom se bez ClrType obešli a stačil by nám (I)EntityType, necháváme zde, protože se velmi snadno používá v unit testech.
	/// </remarks>
	public required Type ClrType { get; init; }

	/// <summary>
	/// Typ měněné entity. Umožní získat informaci pro SkipNavigation entitu (entitu reprezentující vazbu many-to-many pomocí třídy (I)Dictionary).
	/// </summary>
	public required IEntityType EntityType { get; init; }

	/// <summary>
	/// Vrací aktuální hodnotu pro danou vlastnost.
	/// </summary>
	/// <remarks>
	/// Použití v abstraktní třídě umožňuje implementaci pomocí EF Core (EntityEntry)
	/// a zároveň "fake" implementaci pro unit-testy.
	/// </remarks>
	public abstract object GetCurrentValue(IProperty property);

	/// <summary>
	/// Vrací původní hodnotu (hodnotu načtenou z db před provedením změny) pro danou vlastnost.
	/// </summary>
	/// <remarks>
	/// Použití v abstraktní třídě umožňuje implementaci pomocí EF Core (EntityEntry)
	/// a zároveň "fake" implementaci pro unit-testy.
	/// </remarks>
	public abstract object GetOriginalValue(IProperty property);
}
