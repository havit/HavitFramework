namespace Havit.Data.Patterns.Localizations.Internal;

/// <summary>
/// Úložiště párování culture na jazyk.
/// </summary>
public class LanguageByCultureStorage<TLanguageKey> : ILanguageByCultureStorage<TLanguageKey>
{
	private volatile Dictionary<string, TLanguageKey> _value;

	/// <summary>
	/// Úložiště párování culture na jazyk.
	/// </summary>
	public Dictionary<string, TLanguageKey> Value
	{
		get => _value;
		set => _value = value;
	}
}
