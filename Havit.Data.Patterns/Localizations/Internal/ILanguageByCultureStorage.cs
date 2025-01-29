namespace Havit.Data.Patterns.Localizations.Internal;

/// <summary>
/// Úložiště párování culture na jazyk.
/// </summary>
public interface ILanguageByCultureStorage<TLanguageKey>
{
	/// <summary>
	/// Párování culture na jazyk.
	/// </summary>
	Dictionary<string, TLanguageKey> Value { get; set; }
}
