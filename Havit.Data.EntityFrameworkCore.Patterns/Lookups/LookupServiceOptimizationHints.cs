using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Nápověda pro lookup service, aby dokázal fungovat efektivněji.
/// Flags.
/// </summary>
[Flags]
public enum LookupServiceOptimizationHints
{
	/// <summary>
	/// Žádná nápověda.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indikuje read only entitu (pro takovou není třeba provádět invalidace, atp.).
	/// </summary>
	EntityIsReadOnly = 1
}
