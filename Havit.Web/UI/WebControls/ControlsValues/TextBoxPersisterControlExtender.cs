using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues;

/// <summary>
/// IPersisterControlExtender pro NumericBox.
/// </summary>
public class TextBoxPersisterControlExtender : IPersisterControlExtender
{
	/// <summary>
	/// Získá hodnotu (stav) zadaného controlu.		
	/// </summary>
	public object GetValue(System.Web.UI.Control control)
	{
		return ((TextBox)control).Text;
	}

	/// <summary>
	/// Získá typ hodnoty zadaného controlu.
	/// </summary>		
	public Type GetValueType()
	{
		return typeof(String);
	}

	/// <summary>
	/// Nastaví hodnotu do controlu.
	/// </summary>
	public void SetValue(System.Web.UI.Control control, object value)
	{
		((TextBox)control).Text = (string)value;
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
	public int? GetPriority(Control control)
	{
		if (control is TextBox)
		{
			return 1;
		}
		return null;
	}

	/// <summary>
	/// Pokud je true, ControlsValuesPersister se pokusí uložit i hodnoty child controlů.
	/// Implicitně vrací false.
	/// </summary>
	public bool PersistsChildren(Control control)
	{
		return false;
	}
}
