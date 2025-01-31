namespace Havit.Web.UI.WebControls.ControlsValues;

/// <summary>
/// IPersisterControlExtender pro AutoCompleteTextBox.
/// </summary>
public class AutoCompleteTextBoxControlExtender : IPersisterControlExtender
{
	/// <summary>
	/// Získá hodnotu (stav) zadaného controlu.
	/// </summary>
	public object GetValue(System.Web.UI.Control control)
	{
		AutoCompleteTextBox box = (AutoCompleteTextBox)control;
		return new AutoCompleteTextBoxValues()
		{
			Text = box.Text,
			SelectedValue = box.SelectedValue
		};
	}

	/// <summary>
	/// Získá typ hodnoty zadaného controlu.
	/// </summary>
	public Type GetValueType()
	{
		return typeof(Tuple<string, string>);
	}

	/// <summary>
	/// Nastaví hodnotu do controlu.
	/// </summary>
	public void SetValue(System.Web.UI.Control control, object value)
	{
		AutoCompleteTextBox box = (AutoCompleteTextBox)control;
		AutoCompleteTextBoxValues data = (AutoCompleteTextBoxValues)value;
		box.SelectedValue = data.SelectedValue;
		box.Text = data.Text;
	}

	/// <summary>
	/// Vrací prioritu se kterou je tento IPersisterControlExtender použitelný
	/// pro zpracování daného controlu.
	/// Slouží k řešení situací, kdy potřebujeme předka a potomka zpracovávat jinak
	/// (např. DropDownList a EnterpriseDropDownList - registrací na typ DDL
	/// se automaticky chytneme i na EDDL, proto v extenderu pro EDDL použijeme
	/// vyšší prioritu).
	/// Vyšší priorita vyhravá.Není-li control extender použitelný, nechť vrací null.
	/// </summary>
	public int? GetPriority(System.Web.UI.Control control)
	{
		if (control is AutoCompleteTextBox)
		{
			return 1;
		}
		return null;
	}

	/// <summary>
	/// Pokud je true, ControlsValuesPersister se pokusí uložit i hodnoty child controlů.
	/// Implicitně vrací false.
	/// </summary>
	public bool PersistsChildren(System.Web.UI.Control control)
	{
		return false;
	}
}