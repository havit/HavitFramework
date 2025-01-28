﻿namespace Havit.Data.Patterns.DataEntries;

/// <summary>
/// Párování enumů na identifikátor.
/// </summary>
/// <remarks>
/// Generický typ je zde kvůli DI containeru - pro každý typ entity se udělá vlastní singleton.
/// </remarks>
public interface IDataEntrySymbolStorage<TEntity, TKey>
	where TEntity : class
{
	/// <summary>
	/// Úložiště párování enumů na identifikátor.
	/// </summary>
	Dictionary<string, TKey> Value { get; set; }
}
