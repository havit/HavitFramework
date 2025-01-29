namespace Havit.Data.Patterns.Localizations.Internal;

/// <summary>
/// Úložiště párování culture na jazyk.
/// </summary>
public class LanguageByCultureStorage<TLanguageKey> : ILanguageByCultureStorage<TLanguageKey>
{
	/// <summary>
	/// Úložiště párování cultura jazyka.
	/// </summary>
	public Dictionary<string, TLanguageKey> Value { get; set; }
}
