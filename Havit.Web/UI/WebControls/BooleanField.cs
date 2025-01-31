namespace Havit.Web.UI.WebControls;

/// <summary>
/// Sloupec pro zobrazení boolean hodnoty.
/// </summary>
public class BooleanField : BoundFieldExt
{
	/// <summary>
	/// Text zobrazený, pokud je hodnota true.
	/// </summary>
	public string TrueText
	{
		get
		{
			return (string)ViewState["TrueText"];
		}
		set
		{
			ViewState["TrueText"] = value;
		}
	}

	/// <summary>
	/// Text zobrazený, pokud je hodnota false.
	/// </summary>
	public string FalseText
	{
		get
		{
			return (string)ViewState["FalseText"];
		}
		set
		{
			ViewState["FalseText"] = value;
		}
	}

	/// <summary>
	/// Zajistí transformaci boolean hodnoty na text s použitím vlastností TrueText, FalseText.
	/// </summary>
	/// <param name="value">Hodnota ke zformátování.</param>
	/// <returns>Text k zobrazení.</returns>
	public override string FormatDataValue(object value)
	{
		if ((value != null) && (value is bool))
		{
			return (bool)value ? TrueText : FalseText;
		}
		else
		{
			return base.FormatDataValue(value);
		}
	}
}
