﻿namespace Havit.Business.Query;

/// <summary>
/// Vytváří podmínky, které nic netestují.
/// Nyní je taková podmínka reprezentována hodnotou null.
/// </summary>
public static class EmptyCondition
{
	/// <summary>
	/// Vytvoří podmínku reprezentující prázdnou podmínku (nic není testováno). Nyní vrací null.
	/// </summary>
	public static Condition Create()
	{
		return null;
	}
}
