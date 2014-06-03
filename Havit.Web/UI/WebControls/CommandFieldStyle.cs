using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Drawing.Design;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Zapouzdření pro skinování CommandFieldu, uložiště hodnot.
	/// </summary>
	public class CommandFieldStyle : IStateManager
	{
		#region Properties CommandFieldu
		/// <summary>
		/// Gets or sets text that is rendered as the AbbreviatedText property value in some controls.
		/// </summary>
		[Localizable(true)]
		[Category("Accessibility")]
		[DefaultValue("")]
		public virtual string AccessibleHeaderText
		{
			get
			{
				object temp = this.ViewState["AccessibleHeaderText"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, this.ViewState["AccessibleHeaderText"]))
				{
					this.ViewState["AccessibleHeaderText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the button type to display in the button field.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(ButtonType.Link)]
		public virtual ButtonType ButtonType
		{
			get
			{
				object temp = ViewState["ButtonType"];
				if (temp != null)
				{
					return (ButtonType)temp;
				}
				return ButtonType.Link;
			}
			set
			{
				if ((value < ButtonType.Button) || (value > ButtonType.Link))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				object temp = ViewState["ButtonType"];
				if ((temp == null) || (((ButtonType)temp) != value))
				{
					ViewState["ButtonType"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the URL to an image to display for the Cancel button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[UrlProperty]
		[DefaultValue("")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public virtual string CancelImageUrl
		{
			get
			{
				object temp = ViewState["CancelImageUrl"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["CancelImageUrl"]))
				{
					ViewState["CancelImageUrl"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the caption for the Cancel button displayed in a CommandField field.
		/// </summary>
		[DefaultValue(Havit.Web.UI.CommandNames.Cancel)]
		[Category("Appearance")]
		[Localizable(true)]
		public virtual string CancelText
		{
			get
			{
				object temp = ViewState["CancelText"];
				if (temp != null)
				{
					return (string)temp;
				}
				return Havit.Web.UI.CommandNames.Cancel;
			}
			set
			{
				if (!object.Equals(value, ViewState["CancelText"]))
				{
					ViewState["CancelText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether validation is performed when the user clicks a button in a CommandField field.
		/// </summary>
		[DefaultValue(true)]
		[Category("Behavior")]
		public bool CausesValidation
		{
			get
			{
				object temp = ViewState["CausesValidation"];
				if (temp != null)
				{
					return (bool)temp;
				}
				return true;
			}
			set
			{
				CausesValidation = value;
			}
		}

		/// <summary>
		/// Gets or sets the style of any Web server controls contained by the DataControlField object.
		/// </summary>
		[Category("Styles")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DefaultValue((string)null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style ControlStyle
		{
			get
			{
				if (this._controlStyle == null)
				{
					this._controlStyle = new Style();
					if (this._isTrackingViewState)
					{
						((IStateManager)this._controlStyle).TrackViewState();
					}
				}
				return this._controlStyle;
			}
		}
		private Style _controlStyle;

		/// <summary>
		/// Gets or sets the URL to an image to display for a Delete button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[UrlProperty]
		[DefaultValue("")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public virtual string DeleteImageUrl
		{
			get
			{
				object temp = ViewState["DeleteImageUrl"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["DeleteImageUrl"]))
				{
					ViewState["DeleteImageUrl"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Text, na který se má ptát jscript:confirm() před smazáním záznamu. Pokud je prázdný, na nic se neptá.
		/// </summary>
		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue("")]
		public virtual string DeleteConfirmationText
		{
			get
			{
				object temp = ViewState["DeleteConfirmationText"];
				if (temp != null)
				{
					return (string)temp;
				}
				return String.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["DeleteConfirmationText"]))
				{
					ViewState["DeleteConfirmationText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the caption for a Delete button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue(Havit.Web.UI.CommandNames.Delete)]
		public virtual string DeleteText
		{
			get
			{
				object obj2 = ViewState["DeleteText"];
				if (obj2 != null)
				{
					return (string)obj2;
				}
				return Havit.Web.UI.CommandNames.Delete;
			}
			set
			{
				if (!object.Equals(value, ViewState["DeleteText"]))
				{
					ViewState["DeleteText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the URL to an image to display for an Edit button in a CommandField field.
		/// </summary>
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[DefaultValue("")]
		[Category("Appearance")]
		[UrlProperty]
		public virtual string EditImageUrl
		{
			get
			{
				object temp = ViewState["EditImageUrl"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["EditImageUrl"]))
				{
					ViewState["EditImageUrl"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the caption for an Edit button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue(Havit.Web.UI.CommandNames.Edit)]
		public virtual string EditText
		{
			get
			{
				object temp = ViewState["EditText"];
				if (temp != null)
				{
					return (string)temp;
				}
				return Havit.Web.UI.CommandNames.Edit;
			}
			set
			{
				if (!object.Equals(value, ViewState["EditText"]))
				{
					ViewState["EditText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the style of the footer of the data control field.
		/// </summary>
		[Localizable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		public virtual string FooterText
		{
			get
			{
				object obj2 = this.ViewState["FooterText"];
				if (obj2 != null)
				{
					return (string)obj2;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, this.ViewState["FooterText"]))
				{
					this.ViewState["FooterText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the text that is displayed in the footer item of a data control field.
		/// </summary>
		[DefaultValue((string)null)]
		[Category("Styles")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TableItemStyle FooterStyle
		{
			get
			{
				if (this._footerStyle == null)
				{
					this._footerStyle = new TableItemStyle();
					if (this._isTrackingViewState)
					{
						((IStateManager)this._footerStyle).TrackViewState();
					}
				}
				return this._footerStyle;
			}
		}
		private TableItemStyle _footerStyle;

		/// <summary>
		/// Gets or sets the URL of an image that is displayed in the header item of a data control field.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[UrlProperty]
		public virtual string HeaderImageUrl
		{
			get
			{
				object temp = this.ViewState["HeaderImageUrl"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, this.ViewState["HeaderImageUrl"]))
				{
					this.ViewState["HeaderImageUrl"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the style of the header of the data control field.
		/// </summary>
		[Category("Styles")]
		[DefaultValue((string)null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public TableItemStyle HeaderStyle
		{
			get
			{
				if (this._headerStyle == null)
				{
					this._headerStyle = new TableItemStyle();
					if (this._isTrackingViewState)
					{
						((IStateManager)this._headerStyle).TrackViewState();
					}
				}
				return this._headerStyle;
			}
		}
		private TableItemStyle _headerStyle;

		/// <summary>
		/// Gets or sets the text that is displayed in the header item of a data control field.
		/// </summary>
		[Localizable(true)]
		[DefaultValue("")]
		[Category("Appearance")]
		public virtual string HeaderText
		{
			get
			{
				object temp = this.ViewState["HeaderText"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, this.ViewState["HeaderText"]))
				{
					this.ViewState["HeaderText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the URL to an image to display for the Insert button in a CommandField field.
		/// </summary>
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[DefaultValue("")]
		[Category("Appearance")]
		[UrlProperty]
		public virtual string InsertImageUrl
		{
			get
			{
				object temp = ViewState["InsertImageUrl"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["InsertImageUrl"]))
				{
					ViewState["InsertImageUrl"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the caption for the Insert button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue(Havit.Web.UI.CommandNames.Insert)]
		public virtual string InsertText
		{
			get
			{
				object obj2 = ViewState["InsertText"];
				if (obj2 != null)
				{
					return (string)obj2;
				}
				return Havit.Web.UI.CommandNames.Insert;
			}
			set
			{
				if (!object.Equals(value, ViewState["InsertText"]))
				{
					ViewState["InsertText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets the style of any text-based content displayed by a data control field.
		/// </summary>
		[DefaultValue((string)null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Category("Styles")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TableItemStyle ItemStyle
		{
			get
			{
				if (this._itemStyle == null)
				{
					this._itemStyle = new TableItemStyle();
					if (this._isTrackingViewState)
					{
						((IStateManager)this._itemStyle).TrackViewState();
					}
				}
				return this._itemStyle;
			}
		}
		private TableItemStyle _itemStyle;

		/// <summary>
		/// Gets or sets the URL to an image to display for the New button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[UrlProperty]
		public virtual string NewImageUrl
		{
			get
			{
				object temp = ViewState["NewImageUrl"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["NewImageUrl"]))
				{
					ViewState["NewImageUrl"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the caption for the New button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue(Havit.Web.UI.CommandNames.New)]
		public virtual string NewText
		{
			get
			{
				object temp = ViewState["NewText"];
				if (temp != null)
				{
					return (string)temp;
				}
				return Havit.Web.UI.CommandNames.New;
			}
			set
			{
				if (!object.Equals(value, ViewState["NewText"]))
				{
					ViewState["NewText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the URL to an image to display for a Select button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[UrlProperty]
		[DefaultValue("")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public virtual string SelectImageUrl
		{
			get
			{
				object temp = ViewState["SelectImageUrl"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["SelectImageUrl"]))
				{
					ViewState["SelectImageUrl"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the caption for a Select button in a CommandField field.
		/// </summary>
		[Localizable(true)]
		[Category("Appearance")]
		[DefaultValue(Havit.Web.UI.CommandNames.Select)]
		public virtual string SelectText
		{
			get
			{
				object obj2 = ViewState["SelectText"];
				if (obj2 != null)
				{
					return (string)obj2;
				}
				return Havit.Web.UI.CommandNames.Select;
			}
			set
			{
				if (!object.Equals(value, ViewState["SelectText"]))
				{
					ViewState["SelectText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether a Cancel button is displayed in a CommandField field.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		public virtual bool ShowCancelButton
		{
			get
			{
				object temp = ViewState["ShowCancelButton"];
				if (temp != null)
				{
					return (bool)temp;
				}
				return true;
			}
			set
			{
				object temp = ViewState["ShowCancelButton"];
				if ((temp == null) || (((bool)temp) != value))
				{
					ViewState["ShowCancelButton"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether a Delete button is displayed in a CommandField field.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool ShowDeleteButton
		{
			get
			{
				object temp = ViewState["ShowDeleteButton"];
				if (temp != null)
				{
					return (bool)temp;
				}
				return false;
			}
			set
			{
				object temp = ViewState["ShowDeleteButton"];
				if ((temp == null) || (((bool)temp) != value))
				{
					ViewState["ShowDeleteButton"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether an Edit button is displayed in a CommandField field.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool ShowEditButton
		{
			get
			{
				object temp = ViewState["ShowEditButton"];
				if (temp != null)
				{
					return (bool)temp;
				}
				return false;
			}
			set
			{
				object temp = ViewState["ShowEditButton"];
				if ((temp == null) || (((bool)temp) != value))
				{
					ViewState["ShowEditButton"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the header section is displayed in a ButtonFieldBase object.
		/// </summary>
		[DefaultValue(false)]
		[Category("Behavior")]
		public bool ShowHeader
		{
			get
			{
				object temp = ViewState["ShowHeader"];
				if (temp != null)
				{
					return (bool)temp;
				}
				return false;
			}
			set
			{
				object temp = ViewState["ShowHeader"];
				if ((temp == null) || (((bool)temp) != value))
				{
					ViewState["ShowHeader"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether a New button is displayed in a CommandField field.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool ShowInsertButton
		{
			get
			{
				object temp = ViewState["ShowInsertButton"];
				if (temp != null)
				{
					return (bool)temp;
				}
				return false;
			}
			set
			{
				object temp = ViewState["ShowInsertButton"];
				if ((temp == null) || (((bool)temp) != value))
				{
					ViewState["ShowInsertButton"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether a Select button is displayed in a CommandField field.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool ShowSelectButton
		{
			get
			{
				object temp = ViewState["ShowSelectButton"];
				if (temp != null)
				{
					return (bool)temp;
				}
				return false;
			}
			set
			{
				object temp = ViewState["ShowSelectButton"];
				if ((temp == null) || (((bool)temp) != value))
				{
					ViewState["ShowSelectButton"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the URL to an image to display for an Update button in a CommandField field.
		/// </summary>
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[Category("Appearance")]
		[DefaultValue("")]
		[UrlProperty]
		public virtual string UpdateImageUrl
		{
			get
			{
				object temp = ViewState["UpdateImageUrl"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["UpdateImageUrl"]))
				{
					ViewState["UpdateImageUrl"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the caption for an Update button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue(Havit.Web.UI.CommandNames.Update)]
		public virtual string UpdateText
		{
			get
			{
				object obj2 = ViewState["UpdateText"];
				if (obj2 != null)
				{
					return (string)obj2;
				}
				return Havit.Web.UI.CommandNames.Update;
			}
			set
			{
				if (!object.Equals(value, ViewState["UpdateText"]))
				{
					ViewState["UpdateText"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the name of the group of validation controls to validate when a button in a ButtonFieldBase object is clicked.
		/// </summary>
		[DefaultValue("")]
		[Category("Behavior")]
		public virtual string ValidationGroup
		{
			get
			{
				object temp = ViewState["ValidationGroup"];
				if (temp != null)
				{
					return (string)temp;
				}
				return string.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["ValidationGroup"]))
				{
					ViewState["ValidationGroup"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether a data control field is rendered.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		public bool Visible
		{
			get
			{
				object temp = this.ViewState["Visible"];
				if (temp != null)
				{
					return (bool)temp;
				}
				return true;
			}
			set
			{
				object temp = this.ViewState["Visible"];
				if ((temp == null) || (value != ((bool)temp)))
				{
					this.ViewState["Visible"] = value;
					this.OnPropertyChanged();
				}
			}
		}

		#region Tooltips

		#region CancelTooltip
		/// <summary>
		/// Tooltip tlačítka pro zrušení editačního režimu bez úpravy záznamu (cancel).
		/// </summary>
		public string CancelTooltip
		{
			get
			{
				return (string)(ViewState["CancelTooltip"] ?? String.Empty);
			}
			set
			{
				ViewState["CancelTooltip"] = value;
			}
		}
		#endregion

		#region DeleteTooltip
		/// <summary>
		/// Tooltip tlačítka pro smazání záznamu.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string DeleteTooltip
		{
			get
			{
				return (string)(ViewState["DeleteTooltip"] ?? String.Empty);
			}
			set
			{
				ViewState["DeleteTooltip"] = value;
			}
		}
		#endregion

		#region EditTooltip
		/// <summary>
		/// Tooltip tlačítka pro vstup do editace záznamu.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string EditTooltip
		{
			get
			{
				return (string)(ViewState["EditTooltip"] ?? String.Empty);
			}
			set
			{
				ViewState["EditTooltip"] = value;
			}
		}
		#endregion

		#region InsertTooltip
		/// <summary>
		/// Tooltip tlačítka pro vložení nového záznamu.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string InsertTooltip
		{
			get
			{
				return (string)(ViewState["InsertTooltip"] ?? String.Empty);
			}
			set
			{
				ViewState["InsertTooltip"] = value;
			}
		}
		#endregion

		#region InsertTooltip
		/// <summary>
		/// Tooltip tlačítka pro vložení nového záznamu.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string NewTooltip
		{
			get
			{
				return (string)(ViewState["NewTooltip"] ?? String.Empty);
			}
			set
			{
				ViewState["NewTooltip"] = value;
			}
		}
		#endregion

		#region SelectTooltip
		/// <summary>
		/// Tooltip tlačítka pro výběr řádku.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string SelectTooltip
		{
			get
			{
				return (string)(ViewState["SelectTooltip"] ?? String.Empty);
			}
			set
			{
				ViewState["SelectTooltip"] = value;
			}
		}
		#endregion

		#region UpdateTooltip
		/// <summary>
		/// Tooltip  tlačítka pro potvrzení úpravy záznamu.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string UpdateTooltip
		{
			get
			{
				return (string)(ViewState["UpdateTooltip"] ?? String.Empty);
			}
			set
			{
				ViewState["UpdateTooltip"] = value;
			}
		}
		#endregion
		#endregion

		#region CssClasses

		#region CancelCssClass
		/// <summary>
		/// CssClass povoleného tlačítka pro zrušení editačního režimu bez úpravy záznamu (cancel). Je-li hodnota vlastnosti CancelDisabledCssClass prázdná, použije se i pro zakázané tlačítko.
		/// </summary>
		public string CancelCssClass
		{
			get
			{
				return (string)(ViewState["CancelCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["CancelCssClass"] = value;
			}
		}
		#endregion

		#region CancelDisabledCssClass
		/// <summary>
		/// CssClass zakázaného tlačítka pro zrušení editačního režimu bez úpravy záznamu (cancel). Je-li hodnota prázdná, použije se vlastnost CancelCssClass i pro zakázané tlačítko.
		/// </summary>
		public string CancelDisabledCssClass
		{
			get
			{
				return (string)(ViewState["CancelDisabledCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["CancelDisabledCssClass"] = value;
			}
		}
		#endregion

		#region DeleteCssClass
		/// <summary>
		/// CssClass povoleného tlačítka pro smazání záznamu. Je-li hodnota vlastnosti DeleteDisabledCssClass prázdná, použije se i pro zakázané tlačítko.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string DeleteCssClass
		{
			get
			{
				return (string)(ViewState["DeleteCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["DeleteCssClass"] = value;
			}
		}
		#endregion

		#region DeleteDisabledCssClass
		/// <summary>
		/// CssClass zakázaného tlačítka pro smazání záznamu. Je-li hodnota prázdná, použije se vlastnost DeleteCssClass i pro zakázané tlačítko.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string DeleteDisabledCssClass
		{
			get
			{
				return (string)(ViewState["DeleteDisabledCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["DeleteDisabledCssClass"] = value;
			}
		}
		#endregion

		#region EditCssClass
		/// <summary>
		/// CssClass povolené tlačítka pro vstup do editace záznamu. Je-li hodnota vlastnosti EditDisabledCssClass prázdná, použije se i pro zakázané tlačítko.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string EditCssClass
		{
			get
			{
				return (string)(ViewState["EditCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["EditCssClass"] = value;
			}
		}
		#endregion

		#region EditDisabledCssClass
		/// <summary>
		/// CssClass zakázané tlačítka pro vstup do editace záznamu. Je-li hodnota prázdná, použije se vlastnost EditCssClass i pro zakázané tlačítko.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string EditDisabledCssClass
		{
			get
			{
				return (string)(ViewState["EditDisabledCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["EditDisabledCssClass"] = value;
			}
		}
		#endregion

		#region InsertCssClass
		/// <summary>
		/// CssClass povoleného tlačítka pro vložení nového záznamu. Je-li hodnota vlastnosti InsertDisabledCssClass prázdná, použije se i pro zakázané tlačítko.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string InsertCssClass
		{
			get
			{
				return (string)(ViewState["InsertCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["InsertCssClass"] = value;
			}
		}
		#endregion

		#region InsertDisabledCssClass
		/// <summary>
		/// CssClass zakázaného tlačítka pro vložení nového záznamu. Je-li hodnota prázdná, použije se vlastnost InsertCssClass i pro zakázané tlačítko.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string InsertDisabledCssClass
		{
			get
			{
				return (string)(ViewState["InsertDisabledCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["InsertDisabledCssClass"] = value;
			}
		}
		#endregion

		#region SelectCssClass
		/// <summary>
		/// CssClass povoleného tlačítka pro výběr řádku. Je-li hodnota vlastnosti SelectDisabledCssClass prázdná, použije se i pro zakázané tlačítko.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string SelectCssClass
		{
			get
			{
				return (string)(ViewState["SelectCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["SelectCssClass"] = value;
			}
		}
		#endregion

		#region SelectDisabledCssClass
		/// <summary>
		/// CssClass zakázaného tlačítka pro výběr řádku. Je-li hodnota prázdná, použije se vlastnost SelectCssClass i pro zakázané tlačítko.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string SelectDisabledCssClass
		{
			get
			{
				return (string)(ViewState["SelectDisabledCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["SelectDisabledCssClass"] = value;
			}
		}
		#endregion

		#region UpdateCssClass
		/// <summary>
		/// CssClass povoleného tlačítka pro potvrzení úpravy záznamu. Je-li hodnota vlastnosti UpdateDisabledCssClass prázdná, použije se i pro zakázané tlačítko.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string UpdateCssClass
		{
			get
			{
				return (string)(ViewState["UpdateCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["UpdateCssClass"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the CssClass for the New button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue("")]
		public virtual string NewCssClass
		{
			get
			{
				object temp = ViewState["NewCssClass"];
				if (temp != null)
				{
					return (string)temp;
				}
				return String.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["NewCssClass"]))
				{
					ViewState["NewCssClass"] = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the CssClass for the disabed New button in a CommandField field.
		/// </summary>
		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue("")]
		public virtual string NewDisabledCssClass
		{
			get
			{
				object temp = ViewState["NewDisabledCssClass"];
				if (temp != null)
				{
					return (string)temp;
				}
				return String.Empty;
			}
			set
			{
				if (!object.Equals(value, ViewState["NewDisabledCssClass"]))
				{
					ViewState["NewDisabledCssClass"] = value;
					OnPropertyChanged();
				}
			}
		}
		#endregion

		#region UpdateDisabledCssClass
		/// <summary>
		/// CssClass zakázaného tlačítka pro potvrzení úpravy záznamu. Je-li hodnota prázdná, použije se vlastnost UpdateCssClass i pro zakázané tlačítko.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("")]
		public string UpdateDisabledCssClass
		{
			get
			{
				return (string)(ViewState["UpdateDisabledCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["UpdateDisabledCssClass"] = value;
			}
		}
		#endregion

		#endregion

		#endregion

		#region ViewState
		/// <summary>
		/// ViewState pro ukládání hodnot.
		/// </summary>
		internal StateBag ViewState
		{
			get
			{
				return this._viewState;
			}
		}
		private StateBag _viewState;
		#endregion

		#region PropertyChanged (event)
		/// <summary>
		/// Událost, která se zavolá při změně některé property.
		/// </summary>
		public event EventHandler PropertyChanged;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci.
		/// </summary>
		public CommandFieldStyle()
		{
			this._viewState = new StateBag();
		}
		#endregion

		#region OnPropertyChanged 
		/// <summary>
		/// Zajišťuje spuštění události <see cref="PropertyChanged"/>.
		/// </summary>
		protected void OnPropertyChanged()
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, EventArgs.Empty);
			}
		}
		#endregion

		#region IStateManager interface implementation
		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this._isTrackingViewState;
			}
		}
		private bool _isTrackingViewState;

		void IStateManager.LoadViewState(object savedState)
		{
			if (savedState != null)
			{
				object[] objArray = (object[])savedState;
				if (objArray[0] != null)
				{
					((IStateManager)this.ViewState).LoadViewState(objArray[0]);
				}
				if (objArray[1] != null)
				{
					((IStateManager)this.ItemStyle).LoadViewState(objArray[1]);
				}
				if (objArray[2] != null)
				{
					((IStateManager)this.HeaderStyle).LoadViewState(objArray[2]);
				}
				if (objArray[3] != null)
				{
					((IStateManager)this.FooterStyle).LoadViewState(objArray[3]);
				}

				// tohle není v .NET Frameworku !?
				if (objArray[4] != null)
				{
					((IStateManager)this.ControlStyle).LoadViewState(objArray[4]);
				}
			}
		}

		object IStateManager.SaveViewState()
		{
			object obj0 = ((IStateManager)this.ViewState).SaveViewState();
			object obj1 = (this._itemStyle != null) ? ((IStateManager)this._itemStyle).SaveViewState() : null;
			object obj2 = (this._headerStyle != null) ? ((IStateManager)this._headerStyle).SaveViewState() : null;
			object obj3 = (this._footerStyle != null) ? ((IStateManager)this._footerStyle).SaveViewState() : null;
			object obj4 = (this._controlStyle != null) ? ((IStateManager)this._controlStyle).SaveViewState() : null;
			if (((obj0 == null) && (obj1 == null)) && (((obj2 == null) && (obj3 == null)) && (obj4 == null)))
			{
				return null;
			}
			return new object[] { obj0, obj1, obj2, obj3, obj4 };
		}

		void IStateManager.TrackViewState()
		{
			this._isTrackingViewState = true;
			((IStateManager)this.ViewState).TrackViewState();
			if (this._itemStyle != null)
			{
				((IStateManager)this._itemStyle).TrackViewState();
			}
			if (this._headerStyle != null)
			{
				((IStateManager)this._headerStyle).TrackViewState();
			}
			if (this._footerStyle != null)
			{
				((IStateManager)this._footerStyle).TrackViewState();
			}
			if (this._controlStyle != null)
			{
				((IStateManager)this._controlStyle).TrackViewState();
			}
		}
		#endregion

	}
}
