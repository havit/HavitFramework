﻿using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Rozšířená verze <see cref="System.Web.UI.WebControls.TemplateField"/>.
/// </summary>
public class TemplateFieldExt : TemplateField, IIdentifiableField, IFilterField
{
	/// <summary>
	/// Identifikátor fieldu na který se lze odkazovat pomocí <see cref="GridViewExt.FindColumn(string)"/>.
	/// </summary>
	public string ID
	{
		get
		{
			object tmp = ViewState["ID"];
			if (tmp != null)
			{
				return (string)tmp;
			}
			return String.Empty;
		}
		set
		{
			ViewState["ID"] = value;
		}
	}

	/// <summary>
	/// Template pro filtr.
	/// </summary>
	[TemplateContainer(typeof(IDataItemContainer))]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public virtual ITemplate FilterTemplate { get; set; }

	/// <summary>
	/// Styl buňky filtru.
	/// </summary>
	[DefaultValue(null)]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public TableItemStyle FilterStyle
	{
		get
		{
			if (this._filterStyle == null)
			{
				this._filterStyle = new TableItemStyle();
				if (this.IsTrackingViewState)
				{
					((IStateManager)this._filterStyle).TrackViewState();
				}
			}
			return this._filterStyle;
		}
	}
	private TableItemStyle _filterStyle;

	/// <summary>
	/// Copies the properties of the current System.Web.UI.WebControls.TemplateField-derived object to the specified System.Web.UI.WebControls.DataControlField object.
	/// </summary>
	protected override void CopyProperties(DataControlField newField)
	{
		base.CopyProperties(newField);
		if (newField is IFilterField)
		{
			((IFilterField)newField).FilterStyle.CopyFrom(this.FilterStyle);
		}
	}

	/// <summary>
	/// Saves the changes made to the System.Web.UI.WebControls.DataControlField view state since the time the page was posted back to the server.
	/// </summary>
	protected override object SaveViewState()
	{
		return new object[]
		{
			base.SaveViewState(),
			(_filterStyle != null) ? ((IStateManager)_filterStyle).SaveViewState() : null
		};
	}

	/// <summary>
	/// Restores the data source view's previously saved view state.
	/// </summary>
	protected override void LoadViewState(object savedState)
	{
		object[] saveStateData = (object[])savedState;

		base.LoadViewState(saveStateData[0]);
		if (saveStateData[1] != null)
		{
			((IStateManager)this.FilterStyle).LoadViewState(saveStateData[1]);
		}
	}

	/// <summary>
	/// Causes the System.Web.UI.WebControls.DataControlField object to track changes to its view state so they can be stored in the control's System.Web.UI.WebControls.DataControlField.ViewState property and persisted across requests for the same page.
	/// </summary>
	protected override void TrackViewState()
	{
		base.TrackViewState();
		if (_filterStyle != null)
		{
			((IStateManager)_filterStyle).TrackViewState();
		}
	}

	TableItemStyle IFilterField.FilterStyleInternal
	{
		get { return _filterStyle; }
	}

	/// <summary>
	/// Inicializuje buňku filtru.
	/// </summary>
	void IFilterField.InitializeFilterCell(DataControlFieldCell cell)
	{
		if (FilterTemplate != null)
		{
			FilterTemplate.InstantiateIn(cell);
		}
	}
}
