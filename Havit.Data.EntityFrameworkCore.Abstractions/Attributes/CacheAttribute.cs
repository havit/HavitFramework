using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Attributes;

/// <summary>
/// Nastavuje cachování pro entitu.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class CacheAttribute : Attribute
{
	/// <summary>
	/// Indikuje, zda je povoleno cachování entit.
	/// Výchozí hodnota je true.
	/// </summary>
	public bool CacheEntities { get; set; } = true;

	/// <summary>
	/// Indikuje, zda je cachováno cachování klíčů pro GetAll.
	/// Výchozí hodnota je true.
	/// </summary>
	public bool CacheAllKeys { get; set; } = true;

	/// <summary>
	/// Nastavení sliding expiration.
	/// Výchozí hodnotou je hodnota 0 reprezentující "žádné nastavení" (nepoužití sliding expiration).
	/// </summary>
	public int SlidingExpirationSeconds { get; set; }

	/// <summary>
	/// Nastavení absolute expiration.
	/// Výchozí hodnotou je hodnota 0 reprezentující "žádné nastavení" (nepoužití absolute expiration).
	/// </summary>
	public int AbsoluteExpirationSeconds { get; set; }

	/// <summary>
	/// Nastavení priority položek v cache.
	/// Výchozí hodnotou je "Normal".
	/// </summary>
	public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;
}
