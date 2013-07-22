using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Rozšířená verze <see cref="System.Web.UI.WebControls.TemplateField"/>.
	/// </summary>
	public class TemplateFieldExt : TemplateField, IIdentifiableField, IFilterField
	{
		#region ID (IIdentifiableField Members)
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
		#endregion

		#region FilterTemplate
		/// <summary>
		/// Template pro filtr.
		/// </summary>
		[TemplateContainer(typeof(IDataItemContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ITemplate FilterTemplate { get; set; }
		#endregion

		#region FilterStyle
		/// <summary>
		/// Styl buňky filtru.
		/// </summary>
		[DefaultValue((string)null), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
		#endregion

		#region CopyProperties
		protected override void CopyProperties(DataControlField newField)
		{
			base.CopyProperties(newField);
			if (newField is TemplateFieldExt)
			{
				((TemplateFieldExt)newField).FilterStyle.CopyFrom(this.FilterStyle);
			}
		}
		#endregion

		#region SaveViewState, LoadViewState, TrackViewState
		protected override object SaveViewState()
		{
			return new object[]
			{
				base.SaveViewState(),
				(_filterStyle != null) ? ((IStateManager)_filterStyle).SaveViewState() : null
			};
		}

		protected override void LoadViewState(object savedState)
		{
			object[] saveStateData = (object[])savedState;

			base.LoadViewState(saveStateData[0]);
			if (saveStateData[1] != null)
			{
				((IStateManager)this.FilterStyle).LoadViewState(saveStateData[1]);
			}
		}

		protected override void TrackViewState()
		{
			base.TrackViewState();
			if (_filterStyle != null)
			{
				((IStateManager)_filterStyle).TrackViewState();
			}
		}
		#endregion

		#region IFilterField.FilterStyleInternal
		TableItemStyle IFilterField.FilterStyleInternal
		{
			get { return _filterStyle; }
		}
		#endregion

		#region IFilterField.InitializeFilterCell
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
		#endregion

	}
}
