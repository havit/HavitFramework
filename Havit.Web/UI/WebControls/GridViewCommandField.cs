﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Vylepšený command-field určený pro použití s GridViewExt.
	/// </summary>
	/// <remarks>
	/// GridViewCommandField lze skinovat, pokud rodičovský GridView implementuje rozhraní <see cref="ICommandFieldStyle"/>,
	/// což implementuje například <see cref="GridViewExt"/>.
	/// </remarks>
	/// <example>
	/// Skinovat lze pomocí:
	/// <code>
	/// &lt;havit:GridViewExt ... &gt;
	///     &lt;CommandFieldStyle ButtonType=&quot;Image&quot; ... /&gt;
	/// &lt;/havit:GridViewExt&gt;
	/// </code>
	/// </example>
	public class GridViewCommandField : CommandFieldExt
	{
		#region Static fields
		private static readonly MethodInfo _registerWebFormsScriptMethod = typeof(Page).GetMethod("RegisterWebFormsScript", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		#endregion

		#region DeleteConfirmationText
		/// <summary>
		/// Text, na který se má ptát jscript:confirm() před smazáním záznamu. Pokud je prázdný, na nic se neptá.
		/// </summary>
		public string DeleteConfirmationText
		{
			get
			{
				object tmp = ViewState["DeleteConfirmationText"];
				if (tmp != null)
				{
					return (string)tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["DeleteConfirmationText"] = value;
			}
		}
		#endregion

		#region Tooltipy (EditTooltip, CancelTooltip, SelectTooltip, UpdateTooltip, DeleteTooltip, InsertTooltip)

		#region EditTooltip
		/// <summary>
		/// Tooltip tlačítka pro vstup do editace záznamu.
		/// </summary>
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

		#region SelectTooltip
		/// <summary>
		/// Tooltip tlačítka pro výběr řádku.
		/// </summary>
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

		#region DeleteTooltip
		/// <summary>
		/// Tooltip tlačítka pro smazání záznamu.
		/// </summary>
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

		#region InsertTooltip
		/// <summary>
		/// Tooltip tlačítko pro vložení nového záznamu.
		/// </summary>
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

		#endregion

		#region Initialize
		/// <summary>
		/// Inicializuje field (volá se jednou z GridView.CreateChildControls()).
		/// </summary>
		/// <param name="sortingEnabled">indikuje, zda-li je povolený sorting</param>
		/// <param name="control">parent control (GridView)</param>
		/// <returns>false (vždy)</returns>
		public override bool Initialize(bool sortingEnabled, System.Web.UI.Control control)
		{
			if (control is ICommandFieldStyle)
			{
				ApplyStyle(((ICommandFieldStyle)control).CommandFieldStyle, !String.IsNullOrEmpty(control.Page.Theme), !String.IsNullOrEmpty(control.Page.StyleSheetTheme));
			}

			return base.Initialize(sortingEnabled, control);
		}
		#endregion

		#region InitializeCell
		/// <summary>
		/// Inicializuje buňku.
		/// </summary>
		public override void InitializeCell(
			DataControlFieldCell cell,
			DataControlCellType cellType,
			DataControlRowState rowState,
			int rowIndex)
		{
			if (cellType != DataControlCellType.DataCell)
			{
				// Header a Footer řeší korektně ButtonFieldBase, předek CommandFieldu
				base.InitializeCell(cell, cellType, rowState, rowIndex);
			}
			else
			{
				bool showEditButton = this.ShowEditButton;
				bool showDeleteButton = this.ShowDeleteButton;
				bool showInsertButton = this.ShowInsertButton;
				bool showSelectButton = this.ShowSelectButton;
				bool showCancelButton = this.ShowCancelButton;
				bool insertSpace = true;
				bool causesValidation = this.CausesValidation;
				string validationGroup = this.ValidationGroup;

				if (cellType == DataControlCellType.DataCell)
				{
					LiteralControl child;
					if ((rowState & (DataControlRowState.Insert | DataControlRowState.Edit)) != DataControlRowState.Normal)
					{
						if (((rowState & DataControlRowState.Edit) != DataControlRowState.Normal) && showEditButton)
						{
							// stejné jako CommandField
							Control control = (Control) this.AddButtonToCell(cell, "Update", HttpUtilityExt.GetResourceString(this.UpdateText), HttpUtilityExt.GetResourceString(this.UpdateTooltip), causesValidation, validationGroup, rowIndex, this.UpdateImageUrl);
							control.PreRender += (sender, ea) => RegisterDefaultButton(control); // v tento okamžik není dostupný NamingContainer (control ještě není v řádku)
							if (showCancelButton)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
								this.AddButtonToCell(cell, "Cancel", HttpUtilityExt.GetResourceString(this.CancelText), HttpUtilityExt.GetResourceString(this.CancelTooltip), false, string.Empty, rowIndex, this.CancelImageUrl);
							}
						}
						if (((rowState & DataControlRowState.Insert) != DataControlRowState.Normal) && showInsertButton)
						{
							// Nechceme Cancel
							Control control = (Control)this.AddButtonToCell(cell, "Insert", HttpUtilityExt.GetResourceString(this.InsertText), HttpUtilityExt.GetResourceString(this.InsertTooltip), causesValidation, validationGroup, rowIndex, this.InsertImageUrl);
							control.PreRender += (sender, ea) => RegisterDefaultButton(control); // v tento okamžik není dostupný NamingContainer (control ještě není v řádku)
							/*
							if (showCancelButton)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
								this.AddButtonToCell(cell, "Cancel", this.CancelText, false, string.Empty, rowIndex, this.CancelImageUrl);
							}
							 */
						}
					}
					else
					{
						if (showSelectButton)
						{
							this.AddButtonToCell(cell, "Select", HttpUtilityExt.GetResourceString(this.SelectText), HttpUtilityExt.GetResourceString(this.SelectTooltip), false, string.Empty, rowIndex, this.SelectImageUrl);
							insertSpace = false;
						}
						if (showEditButton)
						{
							if (!insertSpace)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
							}
							this.AddButtonToCell(cell, "Edit", HttpUtilityExt.GetResourceString(this.EditText), HttpUtilityExt.GetResourceString(this.EditTooltip), false, string.Empty, rowIndex, this.EditImageUrl);
							insertSpace = false;
						}
						if (showDeleteButton)
						{
							if (!insertSpace)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
							}

							IButtonControl button = this.AddButtonToCell(cell, "Delete", HttpUtilityExt.GetResourceString(this.DeleteText), HttpUtilityExt.GetResourceString(this.DeleteTooltip), false, string.Empty, rowIndex, this.DeleteImageUrl);

							// doplneni o DeleteConfirmText
							string deleteConfirmationTextResolved = HttpUtilityExt.GetResourceString(DeleteConfirmationText);
							if (!String.IsNullOrEmpty(deleteConfirmationTextResolved))
							{
								if (button is Button)
								{
									((Button)button).OnClientClick = String.Format("if (!confirm('{0}')) return false;", deleteConfirmationTextResolved.Replace("'", "''"));
								}
								else if (button is LinkButton)
								{
									((LinkButton)button).OnClientClick = String.Format("if (!confirm('{0}')) return false;", deleteConfirmationTextResolved.Replace("'", "''"));
								}
								else
								{
									Debug.Assert(button is ImageButton);
									((ImageButton)button).OnClientClick = String.Format("if (!confirm('{0}')) return false;", deleteConfirmationTextResolved.Replace("'", "''"));
								}
							}

							insertSpace = false;
						}
						// U Insertu nechceme New
						/*
						if (showInsertButton)
						{
							if (!flag6)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
							}
							this.AddButtonToCell(cell, "New", this.NewText, false, string.Empty, rowIndex, this.NewImageUrl);
							flag6 = false;
						}
						*/
					}
				}
			}
		}

		#endregion

		#region RegisterDefaultButton
		/// <summary>
		/// Zaregistruje button jako default button pro řádek.
		/// </summary>
		private static void RegisterDefaultButton(Control control)
		{
			IAttributeAccessor attributeAccessor = control.NamingContainer as IAttributeAccessor;
			if ((attributeAccessor != null) && (_registerWebFormsScriptMethod != null))
			{
				// Tato metoda zajistí vložení vložení WebForms skriptů do stránky pro emulaci funčnosti DefaultButton u Panelu (který nepřímo tuto metodu používá taky - přes jinou internal)
				_registerWebFormsScriptMethod.Invoke(control.Page, null);

				string firstScript = attributeAccessor.GetAttribute("onkeypressed") ?? String.Empty;
				if (!String.IsNullOrEmpty(firstScript) && !firstScript.EndsWith(";"))
				{
					firstScript += ";";
				}
				attributeAccessor.SetAttribute("onkeypress", firstScript + "if (WebForm_FireDefaultButton) return WebForm_FireDefaultButton(event, '" + control.ClientID + "');");
			}
		}
		#endregion

		#region AddButtonToCell
		private IButtonControl AddButtonToCell(DataControlFieldCell cell, string commandName, string buttonText, string tooltipText, bool causesValidation, string validationGroup, int rowIndex, string imageUrl)
		{
			IButtonControl control;
			IPostBackContainer container = this.Control as IPostBackContainer;
			bool flag = true;
			switch (this.ButtonType)
			{
				case ButtonType.Button:
					Button button;
					if ((container == null) || causesValidation)
					{
						button = new Button();
					}
					else
					{
						button = new DataControlButtonExt(container);
						flag = false;
					}
					button.ToolTip = tooltipText;
					control = button;
					break;

				case ButtonType.Link:
					LinkButton linkButton;
					if ((container == null) || causesValidation)
					{
						linkButton = new DataControlLinkButtonExt(null);
					}
					else
					{
						linkButton = new DataControlLinkButtonExt(container);
						flag = false;
					}
					linkButton.ToolTip = tooltipText;
					control = linkButton;
					break;

				default:
					ImageButton imageButton;
					if ((container != null) && !causesValidation)
					{
						imageButton = new DataControlImageButtonExt(container);
						flag = false;
					}
					else
					{
						imageButton = new ImageButton();
					}
					imageButton.ToolTip = tooltipText;
					imageButton.ImageUrl = imageUrl;
					control = imageButton;
					break;
			}
			control.Text = buttonText;
			control.CommandName = commandName;
			control.CommandArgument = rowIndex.ToString(CultureInfo.InvariantCulture);
			if (flag)
			{
				control.CausesValidation = causesValidation;
			}
			control.ValidationGroup = validationGroup;

			/* Customizace jednotlivých řádek */
			Control buttonControl = (Control)control;
			buttonControl.DataBinding += new EventHandler(this.ButtonControl_DataBinding);

			cell.Controls.Add((WebControl)control);
			return control;
		}

		#endregion

		#region ButtonControl_DataBinding (customizace command-buttonu)
		/// <summary>
		/// Handles the DataBinding event of the buttonControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ButtonControl_DataBinding(object sender, EventArgs e)
		{
			Debug.Assert(sender != null);
			Debug.Assert(sender is IButtonControl);
			Debug.Assert((sender is LinkButton) || (sender is ImageButton) || (sender is Button));

			Control control = (Control)sender;
			IButtonControl buttonControl = (IButtonControl)sender;

			DataControlFieldCell cell = (DataControlFieldCell)control.Parent;
			Debug.Assert(cell != null);

			GridViewRow row = (GridViewRow)cell.Parent;
			Debug.Assert(row != null);

			Table childTable = (Table)row.Parent;
			Debug.Assert(childTable != null);

			GridViewExt gridViewExt = childTable.Parent as GridViewExt;
			if (gridViewExt == null)
			{
				// je použito mimo GridViewExt, neděláme nic
				return;
			}

			// připravíme argumenty
			GridViewRowCustomizingCommandButtonEventArgs args = new GridViewRowCustomizingCommandButtonEventArgs(
				buttonControl.CommandName,
				row.RowIndex,
				row.DataItem);

			// JK: Tohle není úplně OK! Pozor na schování v nadřazeném controlu během databindingu!
			args.Visible = true;
			args.Enabled = true;

			// zavoláme obsluhu události
			gridViewExt.OnRowCustomizingCommandButton(args);

			// nastavíme výsledek z argumentů do buttonu

			control.Visible = args.Visible;

			// pokud je ZA controlem mezera (LiteralControl), chceme ji taky zobrazit/schovat
			int index = control.Parent.Controls.IndexOf(control);
			if ((index < control.Parent.Controls.Count - 1) && (control.Parent.Controls[index + 1] is LiteralControl))
			{
				control.Parent.Controls[index + 1].Visible = args.Visible;
			}

			if (sender is LinkButton)
			{
				LinkButton linkButton = (LinkButton)sender;
				linkButton.Enabled = args.Enabled;
			}
			else if (sender is ImageButton)
			{
				ImageButton imageButton = (ImageButton)sender;
				imageButton.Enabled = args.Enabled;
			}
			else if (sender is Button)
			{
				Button button = (Button)sender;
				button.Enabled = args.Enabled;
			}
		}
		#endregion

		#region ApplyStyle
		/// <summary>
		/// Aplikuje CommandFieldStyle na field.
		/// </summary>
		/// <param name="style">styl k aplikování</param>
		/// <param name="theme">režim Theme (přepsat vše) zapnut</param>
		/// <param name="styleSheetTheme">režim StyleSheetTheme (lokální nastavení mají prioritu) zapnut</param>
		private void ApplyStyle(CommandFieldStyle style, bool theme, bool styleSheetTheme)
		{
			if (style != null)
			{
				if (styleSheetTheme)
				{
					// pokud sami nastaveni nejsme (ViewState je nepoužitý), pak volíme styl

					if (ViewState["AccessibleHeaderText"] == null)
					{
						this.AccessibleHeaderText = style.AccessibleHeaderText;
					}
					if (ViewState["ButtonType"] == null)
					{
						this.ButtonType = style.ButtonType;
					}
					if (ViewState["CancelImageUrl"] == null)
					{
						this.CancelImageUrl = style.CancelImageUrl;
					}
					if (ViewState["CancelText"] == null)
					{
						this.CancelText = style.CancelText;
					}
					if (ViewState["CausesValidation"] == null)
					{
						this.CausesValidation = style.CausesValidation;
					}
					if (this.ControlStyle.IsEmpty)
					{
						this.ControlStyle.CopyFrom(style.ControlStyle);
					}
					if (ViewState["DeleteConfirmationText"] == null)
					{
						this.DeleteConfirmationText = style.DeleteConfirmationText;
					}
					if (ViewState["DeleteImageUrl"] == null)
					{
						this.DeleteImageUrl = style.DeleteImageUrl;
					}
					if (ViewState["DeleteText"] == null)
					{
						this.DeleteText = style.DeleteText;
					}
					if (ViewState["EditImageUrl"] == null)
					{
						this.EditImageUrl = style.EditImageUrl;
					}
					if (ViewState["EditText"] == null)
					{
						this.EditText = style.EditText;
					}
					if (this.FooterStyle.IsEmpty)
					{
						this.FooterStyle.CopyFrom(style.FooterStyle);
					}
					if (ViewState["FooterText"] == null)
					{
						this.FooterText = style.FooterText;
					}
					if (ViewState["HeaderImageUrl"] == null)
					{
						this.HeaderImageUrl = style.HeaderImageUrl;
					}
					if (ViewState["HeaderStyle.IsEmpty"] == null)
					{
						this.HeaderStyle.CopyFrom(style.HeaderStyle);
					}
					if (ViewState["HeaderText"] == null)
					{
						this.HeaderText = style.HeaderText;
					}
					if (ViewState["InsertImageUrl"] == null)
					{
						this.InsertImageUrl = style.InsertImageUrl;
					}
					if (ViewState["InsertText"] == null)
					{
						this.InsertText = style.InsertText;
					}
					if (this.ItemStyle.IsEmpty)
					{
						this.ItemStyle.CopyFrom(style.ItemStyle);
					}
					if (ViewState["NewImageUrl"] == null)
					{
						this.NewImageUrl = style.NewImageUrl;
					}
					if (ViewState["NewText"] == null)
					{
						this.NewText = style.NewText;
					}
					if (ViewState["SelectImageUrl"] == null)
					{
						this.SelectImageUrl = style.SelectImageUrl;
					}
					if (ViewState["SelectText"] == null)
					{
						this.SelectText = style.SelectText;
					}
					if (ViewState["ShowCancelButton"] == null)
					{
						this.ShowCancelButton = style.ShowCancelButton;
					}
					if (ViewState["ShowDeleteButton"] == null)
					{
						this.ShowDeleteButton = style.ShowDeleteButton;
					}
					if (ViewState["ShowEditButton"] == null)
					{
						this.ShowEditButton = style.ShowEditButton;
					}
					if (ViewState["ShowHeader"] == null)
					{
						this.ShowHeader = style.ShowHeader;
					}
					if (ViewState["ShowInsertButton"] == null)
					{
						this.ShowInsertButton = style.ShowInsertButton;
					}
					if (ViewState["ShowSelectButton"] == null)
					{
						this.ShowSelectButton = style.ShowSelectButton;
					}
					if (ViewState["UpdateImageUrl"] == null)
					{
						this.UpdateImageUrl = style.UpdateImageUrl;
					}
					if (ViewState["UpdateText"] == null)
					{
						this.UpdateText = style.UpdateText;
					}
					if (ViewState["ValidationGroup"] == null)
					{
						this.ValidationGroup = style.ValidationGroup;
					}
					if (ViewState["Visible"] == null)
					{
						this.Visible = style.Visible;
					}

					// tooltipy
					if (ViewState["CancelTooltip"] == null)
					{
						this.CancelTooltip = style.CancelTooltip;
					}
					if (ViewState["DeleteTooltip"] == null)
					{
						this.DeleteTooltip = style.DeleteTooltip;
					}
					if (ViewState["EditTooltip"] == null)
					{
						this.EditTooltip = style.EditTooltip;
					}
					if (ViewState["InsertTooltip"] == null)
					{
						this.InsertTooltip = style.InsertTooltip;
					}
					if (ViewState["SelectTooltip"] == null)
					{
						this.SelectTooltip = style.SelectTooltip;
					}
					if (ViewState["UpdateTooltip"] == null)
					{
						this.UpdateTooltip = style.UpdateTooltip;
					}
				}

				if (theme)
				{
					// pokud je nastaven skin, pak volíme vždy styl

					if (style.ViewState["AccessibleHeaderText"] != null)
					{
						this.AccessibleHeaderText = style.AccessibleHeaderText;
					}
					if (style.ViewState["ButtonType"] != null)
					{
						this.ButtonType = style.ButtonType;
					}
					if (style.ViewState["CancelImageUrl"] != null)
					{
						this.CancelImageUrl = style.CancelImageUrl;
					}
					if (style.ViewState["CancelText"] != null)
					{
						this.CancelText = style.CancelText;
					}
					if (style.ViewState["CausesValidation"] != null)
					{
						this.CausesValidation = style.CausesValidation;
					}
					if (this.ControlStyle.IsEmpty)
					{
						this.ControlStyle.CopyFrom(style.ControlStyle);
					}
					if (style.ViewState["DeleteConfirmationText"] != null)
					{
						this.DeleteConfirmationText = style.DeleteConfirmationText;
					}
					if (style.ViewState["DeleteImageUrl"] != null)
					{
						this.DeleteImageUrl = style.DeleteImageUrl;
					}
					if (style.ViewState["DeleteText"] != null)
					{
						this.DeleteText = style.DeleteText;
					}
					if (style.ViewState["EditImageUrl"] != null)
					{
						this.EditImageUrl = style.EditImageUrl;
					}
					if (style.ViewState["EditText"] != null)
					{
						this.EditText = style.EditText;
					}
					if (this.FooterStyle.IsEmpty)
					{
						this.FooterStyle.CopyFrom(style.FooterStyle);
					}
					if (style.ViewState["FooterText"] != null)
					{
						this.FooterText = style.FooterText;
					}
					if (style.ViewState["HeaderImageUrl"] != null)
					{
						this.HeaderImageUrl = style.HeaderImageUrl;
					}
					if (style.ViewState["HeaderStyle.IsEmpty"] != null)
					{
						this.HeaderStyle.CopyFrom(style.HeaderStyle);
					}
					if (style.ViewState["HeaderText"] != null)
					{
						this.HeaderText = style.HeaderText;
					}
					if (style.ViewState["InsertImageUrl"] != null)
					{
						this.InsertImageUrl = style.InsertImageUrl;
					}
					if (style.ViewState["InsertText"] != null)
					{
						this.InsertText = style.InsertText;
					}
					if (this.ItemStyle.IsEmpty)
					{
						this.ItemStyle.CopyFrom(style.ItemStyle);
					}
					if (style.ViewState["NewImageUrl"] != null)
					{
						this.NewImageUrl = style.NewImageUrl;
					}
					if (style.ViewState["NewText"] != null)
					{
						this.NewText = style.NewText;
					}
					if (style.ViewState["SelectImageUrl"] != null)
					{
						this.SelectImageUrl = style.SelectImageUrl;
					}
					if (style.ViewState["SelectText"] != null)
					{
						this.SelectText = style.SelectText;
					}
					if (style.ViewState["ShowCancelButton"] != null)
					{
						this.ShowCancelButton = style.ShowCancelButton;
					}
					if (style.ViewState["ShowDeleteButton"] != null)
					{
						this.ShowDeleteButton = style.ShowDeleteButton;
					}
					if (style.ViewState["ShowEditButton"] != null)
					{
						this.ShowEditButton = style.ShowEditButton;
					}
					if (style.ViewState["ShowHeader"] != null)
					{
						this.ShowHeader = style.ShowHeader;
					}
					if (style.ViewState["ShowInsertButton"] != null)
					{
						this.ShowInsertButton = style.ShowInsertButton;
					}
					if (style.ViewState["ShowSelectButton"] != null)
					{
						this.ShowSelectButton = style.ShowSelectButton;
					}
					if (style.ViewState["UpdateImageUrl"] != null)
					{
						this.UpdateImageUrl = style.UpdateImageUrl;
					}
					if (style.ViewState["UpdateText"] != null)
					{
						this.UpdateText = style.UpdateText;
					}
					if (style.ViewState["ValidationGroup"] != null)
					{
						this.ValidationGroup = style.ValidationGroup;
					}
					if (style.ViewState["Visible"] != null)
					{
						this.Visible = style.Visible;
					}

					// tooltipy
					if (style.ViewState["CancelTooltip"] != null)
					{
						this.CancelTooltip = style.CancelTooltip;
					}
					if (style.ViewState["DeleteTooltip"] != null)
					{
						this.DeleteTooltip = style.DeleteTooltip;
					}
					if (style.ViewState["EditTooltip"] != null)
					{
						this.EditTooltip = style.EditTooltip;
					}
					if (style.ViewState["InsertTooltip"] != null)
					{
						this.InsertTooltip = style.InsertTooltip;
					}
					if (style.ViewState["SelectTooltip"] != null)
					{
						this.SelectTooltip = style.SelectTooltip;
					}
					if (style.ViewState["UpdateTooltip"] != null)
					{
						this.UpdateTooltip = style.UpdateTooltip;
					}
				}
			}
		}
		#endregion
	}
}