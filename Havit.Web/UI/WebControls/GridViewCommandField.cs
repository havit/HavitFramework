using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
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

		#region Tooltipy (EditTooltip, CancelTooltip, SelectTooltip, UpdateTooltip, DeleteTooltip, InsertTooltip, NewTooltip)

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
		/// Tooltip tlačítka pro vložení nového záznamu.
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

		#region NewTooltip
		/// <summary>
		/// Tooltip tlačítka pro založení nového záznamu.
		/// </summary>
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

		#endregion

		#region CssClasses (EditCssClass, EditDisabledCssClass, CancelCssClass, CancelDisabledCssClass, SelectCssClass, SelectDisabledCssClass, UpdateCssClass, UpdateDisabledCssClass, DeleteCssClass, DeleteDisabledCssClass, InsertCssClass, InsertDisabledCssClass, NewCssClass, NewDisabledCssClass, HeaderNewCssClass, HeaderNewDisabledCssClass)

		#region EditCssClass
		/// <summary>
		/// CssClass povolené tlačítka pro vstup do editace záznamu. Je-li hodnota vlastnosti EditDisabledCssClass prázdná, použije se i pro zakázané tlačítko.
		/// </summary>
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

		#region SelectCssClass
		/// <summary>
		/// CssClass povoleného tlačítka pro výběr řádku. Je-li hodnota vlastnosti SelectDisabledCssClass prázdná, použije se i pro zakázané tlačítko.
		/// </summary>
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
		#endregion

		#region UpdateDisabledCssClass
		/// <summary>
		/// CssClass zakázaného tlačítka pro potvrzení úpravy záznamu. Je-li hodnota prázdná, použije se vlastnost UpdateCssClass i pro zakázané tlačítko.
		/// </summary>
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

		#region DeleteCssClass
		/// <summary>
		/// CssClass povoleného tlačítka pro smazání záznamu. Je-li hodnota vlastnosti DeleteDisabledCssClass prázdná, použije se i pro zakázané tlačítko.
		/// </summary>
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

		#region InsertCssClass
		/// <summary>
		/// CssClass povoleného tlačítka pro vložení nového záznamu. Je-li hodnota vlastnosti InsertDisabledCssClass prázdná, použije se i pro zakázané tlačítko.
		/// </summary>
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

		#region NewCssClass
		/// <summary>
		/// CssClass tlačítka pro vložení nového záznamu.
		/// </summary>
		public string NewCssClass
		{
			get
			{
				return (string)(ViewState["NewCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["NewCssClass"] = value;
			}
		}
		#endregion

		#region NewDisabledCssClass
		/// <summary>
		/// CssClass zakázaného tlačítka pro vložení nového záznamu.
		/// </summary>
		public string NewDisabledCssClass
		{
			get
			{
				return (string)(ViewState["NewDisabledCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["NewDisabledCssClass"] = value;
			}
		}
		#endregion

		#region HeaderNewCssClass
		/// <summary>
		/// CssClass buňky v hlavičce tabulky s tlačítkem pro vložení nového záznamu.
		/// </summary>
		public string HeaderNewCssClass
		{
			get
			{
				return (string)(ViewState["HeaderNewCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["HeaderNewCssClass"] = value;
			}
		}
		#endregion

		#region HeaderNewDisabledCssClass
		/// <summary>
		/// CssClass buňky v hlavičce tabulky se zakázaným tlačítkem pro vložení nového záznamu.
		/// </summary>
		public string HeaderNewDisabledCssClass
		{
			get
			{
				return (string)(ViewState["HeaderNewDisabledCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["HeaderNewDisabledCssClass"] = value;
			}
		}
		#endregion

		#endregion

		#region ShowNewButtonForInsertByEditorExtender
		/// <summary>
		/// Indikuje, zda má být zobrazeno tlačítko NewButton pro vkládání nového záznamu externím editorem.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool ShowNewButtonForInsertByEditorExtender
		{
			get
			{
				return (bool)(ViewState["ShowNewButtonForInsertByEditorExtender"] ?? true);
			}
			set
			{
				ViewState["ShowNewButtonForInsertByEditorExtender"] = value;
			}
		}
		#endregion

		#region ShowNewButton
		/// <summary>
		/// Indikuje, zda má být tlačítko pro pořízení nového záznamu v header buňce (určeno pro otevření dialogu s editací nového záznamu).
		/// </summary>
		public bool ShowNewButton
		{
			get
			{
				return (bool)(ViewState["ShowNewButton"] ?? false);
			}
			set
			{
				ViewState["ShowNewButton"] = value;
			}
		}
		#endregion

		#region RowClickEnabledInGridView
		/// <summary>
		/// Indikuje, zda je povoleno RowClickEnabled v nadřazeném GridView
		/// </summary>
		internal bool RowClickEnabledInGridView { get; set; }
		#endregion // záměrně bez ViewState

		#region Initialize
		/// <summary>
		/// Inicializuje field (volá se jednou z GridView.CreateChildControls()).
		/// </summary>
		/// <param name="sortingEnabled">indikuje, zdali je povolený sorting</param>
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
			if (RowClickEnabledInGridView)
			{
				cell.Attributes["data-suppressrowclick"] = "true";
			}

			if (cellType == DataControlCellType.Header)
			{
				if (ShowNewButton)
				{
					string newText = NewText;
					string newTooltip = NewTooltip;
					string newCssClass = NewCssClass;
					string newDisabledCssClass = NewDisabledCssClass;
					string newImageUrl = NewImageUrl;
					if (String.IsNullOrEmpty(newText) && String.IsNullOrEmpty(newTooltip) && String.IsNullOrEmpty(newCssClass) && String.IsNullOrEmpty(newDisabledCssClass) && String.IsNullOrEmpty(newImageUrl))
					{
						newText = InsertText;
						newTooltip = InsertTooltip;
						newCssClass = InsertCssClass;
						newDisabledCssClass = InsertDisabledCssClass;
						newImageUrl = InsertImageUrl;						
					}
					AddButtonToCell(cell, CommandNames.New, HttpUtilityExt.GetResourceString(newText), HttpUtilityExt.GetResourceString(newTooltip), newCssClass, newDisabledCssClass, false, String.Empty, rowIndex, newImageUrl);
				}
			}
			else if (cellType == DataControlCellType.DataCell)
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
							Control control = (Control)this.AddButtonToCell(cell, "Update", HttpUtilityExt.GetResourceString(this.UpdateText), HttpUtilityExt.GetResourceString(this.UpdateTooltip), UpdateCssClass, UpdateDisabledCssClass, causesValidation, validationGroup, rowIndex, this.UpdateImageUrl);
							control.PreRender += (sender, ea) => RegisterDefaultButton(control); // v tento okamžik není dostupný NamingContainer (control ještě není v řádku)
							if (showCancelButton)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
								this.AddButtonToCell(cell, "Cancel", HttpUtilityExt.GetResourceString(this.CancelText), HttpUtilityExt.GetResourceString(this.CancelTooltip), CancelCssClass, CancelDisabledCssClass, false, string.Empty, rowIndex, this.CancelImageUrl);
							}
						}
						if (((rowState & DataControlRowState.Insert) != DataControlRowState.Normal) && showInsertButton)
						{
							// Nechceme Cancel
							Control control = (Control)this.AddButtonToCell(cell, "Insert", HttpUtilityExt.GetResourceString(this.InsertText), HttpUtilityExt.GetResourceString(this.InsertTooltip), InsertCssClass, InsertDisabledCssClass, causesValidation, validationGroup, rowIndex, this.InsertImageUrl);
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
							this.AddButtonToCell(cell, "Select", HttpUtilityExt.GetResourceString(this.SelectText), HttpUtilityExt.GetResourceString(this.SelectTooltip), SelectCssClass, SelectDisabledCssClass, false, string.Empty, rowIndex, this.SelectImageUrl);
							insertSpace = false;
						}
						if (showEditButton)
						{
							if (!insertSpace)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
							}
							this.AddButtonToCell(cell, "Edit", HttpUtilityExt.GetResourceString(this.EditText), HttpUtilityExt.GetResourceString(this.EditTooltip), EditCssClass, EditDisabledCssClass, false, string.Empty, rowIndex, this.EditImageUrl);
							insertSpace = false;
						}
						if (showDeleteButton)
						{
							if (!insertSpace)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
							}

							IButtonControl button = this.AddButtonToCell(cell, "Delete", HttpUtilityExt.GetResourceString(this.DeleteText), HttpUtilityExt.GetResourceString(this.DeleteTooltip), DeleteCssClass, DeleteDisabledCssClass, false, string.Empty, rowIndex, this.DeleteImageUrl);

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
					}
				}
			}
			else
			{
				base.InitializeCell(cell, cellType, rowState, rowIndex);					
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
		private IButtonControl AddButtonToCell(DataControlFieldCell cell, string commandName, string buttonText, string tooltipText, string cssClass, string cssClassDisabled, bool causesValidation, string validationGroup, int rowIndex, string imageUrl)
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
					button.CssClass = cssClass;
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
					linkButton.CssClass = cssClass;
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
					imageButton.CssClass = cssClass;
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
			buttonControl.DataBinding += (sender, eventArgs) => ButtonControl_HandleDataBinding((Control)sender, cssClassDisabled);

			cell.Controls.Add((WebControl)control);
			return control;
		}

		#endregion

		#region ButtonControl_HandleDataBinding (customizace command-buttonu)
		private void ButtonControl_HandleDataBinding(Control control, string cssClassDisabled)
		{
			Debug.Assert(control != null);
			Debug.Assert(control is IButtonControl);
			Debug.Assert((control is LinkButton) || (control is ImageButton) || (control is Button));

			IButtonControl buttonControl = (IButtonControl)control;

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
			args.Enabled = gridViewExt.Enabled;

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
			
			if (control is LinkButton)
			{
				LinkButton linkButton = (LinkButton)control;
				linkButton.Enabled = args.Enabled;
				if (!args.Enabled && !String.IsNullOrEmpty(cssClassDisabled))
				{
					linkButton.CssClass = cssClassDisabled;
				}
			}
			else if (control is ImageButton)
			{
				ImageButton imageButton = (ImageButton)control;
				imageButton.Enabled = args.Enabled;
				if (!args.Enabled && !String.IsNullOrEmpty(cssClassDisabled))
				{
					imageButton.CssClass = cssClassDisabled;
				}
			}
			else if (control is Button)
			{
				Button button = (Button)control;
				button.Enabled = args.Enabled;
				if (!args.Enabled && !String.IsNullOrEmpty(cssClassDisabled))
				{
					button.CssClass = cssClassDisabled;
				}
			}

			if (args.CommandName == CommandNames.New)
			{
				TableCell headerCell = control.Parent as TableCell;
				if (headerCell != null)
				{
					headerCell.CssClass = (HeaderStyle.CssClass + " " + (args.Enabled ? HeaderNewCssClass : HeaderNewDisabledCssClass)).Trim();
				}
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
					if (ViewState["NewCssClass"] == null)
					{
						this.NewCssClass = style.NewCssClass;
					}
					if (ViewState["NewDisabledCssClass"] == null)
					{
						this.NewDisabledCssClass = style.NewDisabledCssClass;
					}
					if (ViewState["NewImageUrl"] == null)
					{
						this.NewImageUrl = style.NewImageUrl;
					}
					if (ViewState["NewText"] == null)
					{
						this.NewText = style.NewText;
					}
					if (ViewState["NewTooltip"] == null)
					{
						this.NewTooltip = style.NewTooltip;
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

					// css classes
					if (ViewState["CancelCssClass"] == null)
					{
						this.CancelCssClass = style.CancelCssClass;
					}
					if (ViewState["CancelDisabledCssClass"] == null)
					{
						this.CancelDisabledCssClass = style.CancelDisabledCssClass;
					}
					if (ViewState["DeleteCssClass"] == null)
					{
						this.DeleteCssClass = style.DeleteCssClass;
					}
					if (ViewState["DeleteDisabledCssClass"] == null)
					{
						this.DeleteDisabledCssClass = style.DeleteDisabledCssClass;
					}
					if (ViewState["EditCssClass"] == null)
					{
						this.EditCssClass = style.EditCssClass;
					}
					if (ViewState["EditDisabledCssClass"] == null)
					{
						this.EditDisabledCssClass = style.EditDisabledCssClass;
					}
					if (ViewState["HeaderNewCssClass"] == null)
					{
						this.HeaderNewCssClass = style.HeaderNewCssClass;
					}
					if (ViewState["HeaderNewDisabledCssClass"] == null)
					{
						this.HeaderNewDisabledCssClass = style.HeaderNewDisabledCssClass;
					}
					if (ViewState["InsertCssClass"] == null)
					{
						this.InsertCssClass = style.InsertCssClass;
					}
					if (ViewState["InsertDisabledCssClass"] == null)
					{
						this.InsertDisabledCssClass = style.InsertDisabledCssClass;
					}
					if (ViewState["SelectCssClass"] == null)
					{
						this.SelectCssClass = style.SelectCssClass;
					}
					if (ViewState["SelectDisabledCssClass"] == null)
					{
						this.SelectDisabledCssClass = style.SelectDisabledCssClass;
					}
					if (ViewState["UpdateCssClass"] == null)
					{
						this.UpdateCssClass = style.UpdateCssClass;
					}
					if (ViewState["UpdateDisabledCssClass"] == null)
					{
						this.UpdateDisabledCssClass = style.UpdateDisabledCssClass;
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
					if (style.ViewState["ShowNewButton"] != null)
					{
						this.ShowNewButton = style.ShowNewButton;
					}
					if (style.ViewState["ShowNewButtonForInsertByEditorExtender"] != null)
					{
						this.ShowNewButtonForInsertByEditorExtender = style.ShowNewButtonForInsertByEditorExtender;
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

					// css classes
					if (style.ViewState["CancelCssClass"] != null)
					{
						this.CancelCssClass = style.CancelCssClass;
					}
					if (style.ViewState["CancelDisabledCssClass"] != null)
					{
						this.CancelDisabledCssClass = style.CancelDisabledCssClass;
					}
					if (style.ViewState["DeleteCssClass"] != null)
					{
						this.DeleteCssClass = style.DeleteCssClass;
					}
					if (style.ViewState["DeleteDisabledCssClass"] != null)
					{
						this.DeleteDisabledCssClass = style.DeleteDisabledCssClass;
					}
					if (style.ViewState["EditCssClass"] != null)
					{
						this.EditCssClass = style.EditCssClass;
					}
					if (style.ViewState["EditDisabledCssClass"] != null)
					{
						this.EditDisabledCssClass = style.EditDisabledCssClass;
					}
					if (style.ViewState["InsertCssClass"] != null)
					{
						this.InsertCssClass = style.InsertCssClass;
					}
					if (style.ViewState["InsertDisabledCssClass"] != null)
					{
						this.InsertDisabledCssClass = style.InsertDisabledCssClass;
					}
					if (style.ViewState["SelectCssClass"] != null)
					{
						this.SelectCssClass = style.SelectCssClass;
					}
					if (style.ViewState["SelectDisabledCssClass"] != null)
					{
						this.SelectDisabledCssClass = style.SelectDisabledCssClass;
					}
					if (style.ViewState["UpdateCssClass"] != null)
					{
						this.UpdateCssClass = style.UpdateCssClass;
					}
					if (style.ViewState["UpdateDisabledCssClass"] != null)
					{
						this.UpdateDisabledCssClass = style.UpdateDisabledCssClass;
					}
				}
			}
		}
		#endregion
	}
}