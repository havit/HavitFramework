using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// TextBox pro automatické filtry.
/// Sám se spojí s GridView, získá bidnovaná data a z nich získá distinct hodnoty pro filtrování. Sám (ve spojení s gridem) filtruje jeho bindované hodnoty.
/// </summary>
public class AutoFilterTextBox : TextBoxExt, IAutoFilterControl
{
	/// <summary>
	/// Vlastnost, ve které se vyhledává.
	/// </summary>
	public string DataFilterField
	{
		get
		{
			return (string)(ViewState["DataFilterField"] ?? String.Empty);
		}
		set
		{
			ViewState["DataFilterField"] = value;
		}
	}

	/// <summary>
	/// Konstructor.
	/// </summary>
	public AutoFilterTextBox()
	{
		AutoPostBack = true;
	}

	/// <summary>
	/// Událost oznamuje změnu hodnoty filtru.
	/// </summary>
	public event EventHandler ValueChanged
	{
		add
		{
			Events.AddHandler(eventValueChanged, value);
		}
		remove
		{
			Events.RemoveHandler(eventValueChanged, value);
		}
	}
	private static readonly object eventValueChanged = new object();

	/// <summary>
	/// Vyvolá událost ValueChanged.
	/// </summary>
	protected void OnValueChanged(EventArgs e)
	{
		EventHandler handler = (EventHandler)Events[eventValueChanged];
		if (handler != null)
		{
			handler(this, e);
		}
	}

	/// <summary>
	/// Provede filtrování dat na základě nastavení filtru.
	/// </summary>
	public IEnumerable FilterData(IEnumerable data)
	{
		if (this.Text.Length > 0)
		{
			Regex filterRegex = Havit.Text.RegularExpressions.RegexPatterns.GetWildcardRegex("*" + this.Text + "*");
			return data.Cast<object>().Where(item => filterRegex.IsMatch(DataBinderExt.GetValue(item, this.DataFilterField, ""))).ToList();
		}
		else
		{
			return data;
		}
	}

	/// <summary>
	/// Inicializuje control.
	/// Vyvoláním události s argumentem AutoFilterControlCreatedEventArgs.Empty se registruje jak control pro automatický databind.
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		this.RaiseBubbleEvent(this, AutoFilterControlCreatedEventArgs.Empty); // registrace autofilter controlu
	}

	/// <summary>
	/// Při změně hodnoty vyvolá událost ValueChanged.
	/// </summary>
	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
		this.OnValueChanged(EventArgs.Empty); // oznámení změny hodnoty filtru
	}
}
