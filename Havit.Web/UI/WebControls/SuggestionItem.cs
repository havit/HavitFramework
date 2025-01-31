using System.Runtime.Serialization;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Položka našeptávače.
/// </summary>
[DataContract]
public class SuggestionItem
{
	/// <summary>
	/// Identifikátor položky.
	/// </summary>
	[DataMember(Name = "data")]
	public string Value { get; set; }

	/// <summary>
	/// Textová hodnota položky.
	/// </summary>
	[DataMember(Name = "value")]
	public string Text { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="SuggestionItem"/> class.
	/// </summary>
	public SuggestionItem(string data, string text)
	{
		Value = data;
		Text = text;
	}
}
