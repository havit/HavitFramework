using Havit.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Třída návratové hodnoty služby pro AutoCompleteTextBox
/// </summary>
[DataContract]
public class GetSuggestionsResult
{
	/// <summary>
	/// Položky našeptávače.
	/// </summary>
	[DataMember(Name = "suggestions")]
	public List<SuggestionItem> Suggestions { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="GetSuggestionsResult"/> class.
	/// </summary>
	public GetSuggestionsResult()
	{
		Suggestions = new List<SuggestionItem>();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="GetSuggestionsResult"/> class.
	/// </summary>
	/// <param name="suggestions">Kolekce položek.</param>
	public GetSuggestionsResult(IEnumerable<SuggestionItem> suggestions)
	{
		Contract.Requires(suggestions != null);

		Suggestions = suggestions.ToList();
	}

	/// <summary>
	/// Přidá položky do kolegce Suggestions.
	/// </summary>
	/// <param name="items">Položky které se mají přidat.</param>
	/// <param name="valueSelector">Funkce pro výběr hodnoty prvku.</param>
	/// <param name="textSelector">Funkce pro výběr názvu prvku.</param>
	public void Fill<TItem>(IEnumerable<TItem> items, Func<TItem, string> valueSelector, Func<TItem, string> textSelector)
	{
		Suggestions.AddRange(items.Select(i => new SuggestionItem(valueSelector(i), textSelector(i))));
	}
}