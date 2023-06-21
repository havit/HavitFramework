using Havit.Diagnostics.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Control pro použití jako editor s obousměrným databindingem. Podporuje jen jednu šablonu (ContentTemplate) narozdíl od FormView (ItemTemplate, EditItemTemplate, InsertItemTemplate),
/// čímž umožníme generovat designeru fieldy pro controly dle control ID.
/// Implementováno zapouzdřením FormViewExt.
/// </summary>
[ParseChildren(true)]
[PersistChildren(false)]
internal class EditView : Control
{
	private readonly FormViewExt formViewExt;

	/// <summary>
	/// Šablona pro editaci.
	/// </summary>
	[TemplateInstance(TemplateInstance.Single)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	[TemplateContainer(typeof(FormView), BindingDirection.TwoWay)]
	public ITemplate ContentTemplate
	{
		get
		{
			return formViewExt.EditItemTemplate;
		}
		set
		{
			formViewExt.EditItemTemplate = value;
		}
	}

	/// <summary>
	/// Item type. Pro strong type databinding.
	/// </summary>
	[Themeable(false)]
	public virtual string ItemType
	{
		get
		{
			return (this._itemType ?? string.Empty);
		}
		set
		{
			if (!string.Equals(this._itemType, value, StringComparison.OrdinalIgnoreCase))
			{
				this._itemType = value;
				//this.OnDataPropertyChanged();
			}
		}
	}
	private string _itemType;

	/// <summary>
	/// Nastavuje automatický databind.
	/// </summary>
	public bool AutoDataBind
	{

		get
		{
			return formViewExt.AutoDataBind;
		}
		set
		{
			formViewExt.AutoDataBind = value;
		}
	}

	/// <summary>
	/// Nastaví objekt nebo kolekci jako datový zdroj FormView.
	/// </summary>
	public object DataSource
	{
		get
		{
			return formViewExt.DataSource;
		}
		set
		{
			formViewExt.DataSource = value;
		}
	}

	public bool RenderOuterTable
	{
		get
		{
			return formViewExt.RenderOuterTable;
		}
		set
		{
			formViewExt.RenderOuterTable = value;
		}
	}

	/// <summary>
	/// Indikuje, zda je vyžadován DataBinding.
	/// </summary>
	public bool RequiresDatabinding
	{
		get
		{
			return formViewExt.RequiresDataBinding;
		}
	}

	public new event EventHandler DataBinding
	{
		add
		{
			formViewExt.DataBinding += value;
		}
		remove
		{
			formViewExt.DataBinding -= value;
		}
	}

	public event EventHandler DataBound
	{
		add
		{
			formViewExt.DataBound += value;
		}
		remove
		{
			formViewExt.DataBound -= value;

		}
	}

	public event FormViewUpdateEventHandler ItemUpdating
	{
		add
		{
			formViewExt.ItemUpdating += value;
		}
		remove
		{
			formViewExt.ItemUpdating -= value;
		}
	}

	public event FormViewUpdatedEventHandler ItemUpdated
	{
		add
		{
			formViewExt.ItemUpdated += value;
		}
		remove
		{
			formViewExt.ItemUpdated -= value;
		}
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public EditView()
	{
		formViewExt = new FormViewExt();
		formViewExt.ChangeMode(FormViewMode.Edit);
		formViewExt.ModeChanging += this.FormViewExt_ModeChanging;
	}

	/// <summary>
	/// Zamezí změně módu zobrazení ve FormView.
	/// </summary>
	private void FormViewExt_ModeChanging(object sender, FormViewModeEventArgs e)
	{
		e.Cancel = true;
	}

	/// <summary>
	/// Vloží zapouzdřený FormViewExt do stromu controlů.
	/// </summary>
	protected override void CreateChildControls()
	{
		base.CreateChildControls();
		this.Controls.Add(formViewExt);
	}

	/// <summary>
	/// Zajistí volání CreateChildControls.
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		EnsureChildControls();
	}

	/// <summary>
	/// Vyzvedne hodnoty, které jsou nabidnované způsobem pro two-way databinding.
	/// Hodnoty nastaví jako vlastnosti předanému datovému objektu.
	/// </summary>
	/// <param name="dataObject">Datová objekt, jehož hodnoty jsou nastaveny.</param>
	public void ExtractValues(object dataObject)
	{
		formViewExt.ExtractValues(dataObject);
	}

	/// <summary>
	/// Hledá control se zadaným ID ve FormView.
	/// </summary>
	public override Control FindControl(string id)
	{
		return formViewExt.Row.FindControl(id);
	}

	/// <summary>
	/// Nastaví RequiresDataBinding na true.
	/// Zajistí zavolání databindingu ještě v aktuálním requestu. Běžně v OnPreRender,
	/// pokud je ale FormView schovaný, pak se DataBind volá z Page.PreRenderComplete.
	/// </summary>
	public void SetRequiresDatabinding()
	{
		formViewExt.SetRequiresDatabinding();
	}
}
