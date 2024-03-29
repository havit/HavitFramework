﻿using System.ComponentModel;

namespace Havit.Services.Caching;

/// <summary>
/// Parametry pro uložení položky v cache.
/// </summary>
public class CacheOptions
{
	/// <summary>
	/// Doba, po které má být položka vyhozena z cache (od okamžiku přidání).
	/// </summary>
	public TimeSpan? AbsoluteExpiration
	{
		get
		{
			return absoluteExpiration;
		}
		set
		{
			ThrowIfFrozen();
			absoluteExpiration = value;
		}
	}
	private TimeSpan? absoluteExpiration;

	/// <summary>
	/// Doba, po které má být položka vyhozena z cache (od posledního přístupu k položce).
	/// </summary>
	public TimeSpan? SlidingExpiration
	{
		get
		{
			return slidingExpiration;
		}
		set
		{
			ThrowIfFrozen();
			slidingExpiration = value;
		}
	}
	private TimeSpan? slidingExpiration;

	/// <summary>
	/// Priorita položky v cache.
	/// </summary>
	public CacheItemPriority Priority
	{
		get
		{
			return priority;
		}
		set
		{
			ThrowIfFrozen();
			priority = value;
		}
	}
	private CacheItemPriority priority = CacheItemPriority.Normal;

	/// <summary>
	/// Odhadovaná velikost položky v cache (použito jen pro IMemoryCache, bezrozměrná jednotka).
	/// </summary>
	public int? Size
	{
		get
		{
			return size;
		}
		set
		{
			ThrowIfFrozen();
			size = value;
		}
	}
	private int? size;

	/// <summary>
	/// Cache dependencies - Klíče, na kterých je položka závislá.
	/// </summary>
	public string[] CacheDependencyKeys
	{
		get
		{
			return cacheDependencyKeys;
		}
		set
		{
			ThrowIfFrozen();
			cacheDependencyKeys = value;
		}
	}
	private string[] cacheDependencyKeys;

	/// <summary>
	/// Vytvoří kopii cache options.
	/// Pokud je zdrojová cache options uzamčena pro změny, kopie uzamčena nebude.
	/// </summary>
	public CacheOptions Clone()
	{
		return new CacheOptions
		{
			AbsoluteExpiration = this.AbsoluteExpiration,
			SlidingExpiration = this.SlidingExpiration,
			Priority = this.Priority,
			Size = this.Size,
			CacheDependencyKeys = this.CacheDependencyKeys
		};
	}

	/// <summary>
	/// Zamkne kolekci vůči změnám. Od toho okamžiku není možné změnit položky v kolekci.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void Freeze()
	{
		isFrozen = true;
	}
	private bool isFrozen = false;

	/// <summary>
	/// Pokud je nastaven příznak isFrozen, vyhodí výjimku InvalidOperationException.
	/// </summary>
	private void ThrowIfFrozen()
	{
		if (this.isFrozen)
		{
			throw new InvalidOperationException("CacheOptions are frozen, cannot be modified.");
		}
	}
}
