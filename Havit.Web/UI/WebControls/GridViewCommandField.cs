using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Globalization;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Vylepšený command-field urèený pro použití s GridViewExt.
	/// </summary>
	public class GridViewCommandField : CommandField
	{
		public string DeleteConfirmText
		{
			get { return (string)ViewState["DeleteConfirmText"]; }
			set { ViewState["DeleteConfirmText"] = value; }
		}

		#region InitializeCell
		public override void InitializeCell(
			DataControlFieldCell cell,
			DataControlCellType cellType,
			DataControlRowState rowState,
			int rowIndex)
		{
			if (cellType != DataControlCellType.DataCell)
			{
				// Header a Footer øeší korektnì ButtonFieldBase, pøedek CommandFieldu
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
							this.AddButtonToCell(cell, "Update", this.UpdateText, causesValidation, validationGroup, rowIndex, this.UpdateImageUrl);
							if (showCancelButton)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
								this.AddButtonToCell(cell, "Cancel", this.CancelText, false, string.Empty, rowIndex, this.CancelImageUrl);
							}
						}
						if (((rowState & DataControlRowState.Insert) != DataControlRowState.Normal) && showInsertButton)
						{
							// Nechceme Cancel
							this.AddButtonToCell(cell, "Insert", this.InsertText, causesValidation, validationGroup, rowIndex, this.InsertImageUrl);
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
						if (showEditButton)
						{
							this.AddButtonToCell(cell, "Edit", this.EditText, false, string.Empty, rowIndex, this.EditImageUrl);
							insertSpace = false;
						}
						if (showDeleteButton)
						{
							if (!insertSpace)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
							}
							
							IButtonControl button = this.AddButtonToCell(cell, "Delete", this.DeleteText, false, string.Empty, rowIndex, this.DeleteImageUrl);

							// doplneni o DeleteConfirmText
							if (!String.IsNullOrEmpty(DeleteConfirmText))
							{
								((WebControl)button).Attributes.Add("onclick", String.Format("return confirm('{0}');", DeleteConfirmText.Replace("'", "''")));
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
						if (showSelectButton)
						{
							if (!insertSpace)
							{
								child = new LiteralControl("&nbsp;");
								cell.Controls.Add(child);
							}
							this.AddButtonToCell(cell, "Select", this.SelectText, false, string.Empty, rowIndex, this.SelectImageUrl);
							insertSpace = false;
						}
					}
				}
			}
		}
		#endregion

		#region AddButtonToCell
		private IButtonControl AddButtonToCell(DataControlFieldCell cell, string commandName, string buttonText, bool causesValidation, string validationGroup, int rowIndex, string imageUrl)
		{
			IButtonControl control;
			IPostBackContainer container = base.Control as IPostBackContainer;
			bool flag = true;
			switch (this.ButtonType)
			{
				case ButtonType.Button:
					if ((container == null) || causesValidation)
					{
						control = new Button();
					}
					else
					{
						control = new DataControlButtonExt(container);
						flag = false;
					}
					break;

				case ButtonType.Link:
					if ((container == null) || causesValidation)
					{
						control = new DataControlLinkButtonExt(null);
					}
					else
					{
						control = new DataControlLinkButtonExt(container);
						flag = false;
					}
					break;

				default:
					if ((container != null) && !causesValidation)
					{
						control = new DataControlImageButtonExt(container);
						flag = false;
					}
					else
					{
						control = new ImageButton();
					}
					((ImageButton)control).ImageUrl = imageUrl;
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
			cell.Controls.Add((WebControl)control);
			return control;
		}
		#endregion
	}
}