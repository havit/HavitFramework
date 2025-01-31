namespace Havit.Web.UI.WebControls;

/// <summary>
/// Režim autosuggest menu.
/// </summary>
public enum AutoSuggestMenuMode
{
	/// <summary>
	/// Klasický režim, který neřeší, co se s textem děje, pokud není nic vybráno.
	/// Určeno pro zpětnou kompatibilitu (výchozí hodnota).
	/// </summary>
	Classic,

	/// <summary>
	/// Při opuštění vyčistí textové pole, pokud není nic vybráno.
	/// </summary>
	ClearTextOnNoSelection
}
