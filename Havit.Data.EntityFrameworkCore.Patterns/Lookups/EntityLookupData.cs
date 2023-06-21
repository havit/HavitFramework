using System;
using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Data evidovaná k jedné lookup službě.
/// </summary>
/// <typeparam name="TEntity">Entita (typ), kterou vyhledáváme.</typeparam>
/// <typeparam name="TEntityKey">Typ klíče entity. Aktuálně vždy int.</typeparam>
/// <typeparam name="TLookupKey">Typ hodnoty, dle které vyhledáváme.</typeparam>
public class EntityLookupData<TEntity, TEntityKey, TLookupKey>
{
	/// <summary>
	/// Základní data pro vyhledávání, na základě lookup klíče obsahuje klíč entity (identifikátor).
	/// </summary>
	public Dictionary<TLookupKey, TEntityKey> EntityKeyByLookupKeyDictionary { get; }

	/// <summary>
	/// Reverzní data, slouží pro invalidaci záznamů, na základě klíče entity dohledává evidovaný lookup klíč.
	/// </summary>
	public Dictionary<TEntityKey, TLookupKey> LookupKeyByEntityKeyDictionary { get; }

	/// <summary>
	/// Zkompilovaná expression pro získání klíče z entity.
	/// Kvůli performance, ať se nekompiluje znovu a znovu.
	/// </summary>
	public Func<TEntity, TLookupKey> LookupKeyCompiledLambda { get; }

	/// <summary>
	/// Zkompilovaná lambda filtru (nepovinné).
	/// Kvůli performance, ať se nekompiluje znovu a znovu.
	/// </summary>
	public Func<TEntity, bool> FilterCompiledLambda { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityLookupData(
		Dictionary<TLookupKey, TEntityKey> entityKeyByLookupKeyDictionary,
		Dictionary<TEntityKey, TLookupKey> lookupKeyByEntityKeyDictionary,
		Func<TEntity, TLookupKey> lookupKeyCompiledLambda,
		Func<TEntity, bool> filterCompiledLambda)
	{
		EntityKeyByLookupKeyDictionary = entityKeyByLookupKeyDictionary;
		LookupKeyByEntityKeyDictionary = lookupKeyByEntityKeyDictionary;
		LookupKeyCompiledLambda = lookupKeyCompiledLambda;
		FilterCompiledLambda = filterCompiledLambda;
	}
}
