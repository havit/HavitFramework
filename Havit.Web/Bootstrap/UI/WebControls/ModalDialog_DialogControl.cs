using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls;

public partial class ModalDialog
{
	/// <summary>
	/// Control for rendering dialog.
	/// </summary>
	internal class DialogControl : Control
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public DialogControl(ModalDialog modalDialog)
		{
			this.modalDialog = modalDialog;
		}
		private readonly ModalDialog modalDialog;

		protected override void Render(HtmlTextWriter writer)
		{
			RenderBeginTags(writer);
			base.Render(writer); // render children
			RenderEndTags(writer);
		}

		private void RenderBeginTags(HtmlTextWriter writer)
		{
			writer.AddAttribute("id", this.ClientID);
			writer.AddAttribute(HtmlTextWriterAttribute.Class, modalDialog.UseAnimations ? "modal fade" : "modal");
			writer.AddAttribute("role", "dialog");
			writer.AddAttribute("data-backdrop", "static");
			writer.AddAttribute("data-keyboard", "false");
			writer.AddAttribute("onkeypress", "return Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.suppressFireDefaultButton(event);");

			//writer.AddAttribute("aria-labelledby", "dialog");
			//writer.AddAttribute("aria-hidden", "true");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			string modalDialogCssClass = ("modal-dialog " + modalDialog.CssClass).Trim();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, modalDialogCssClass);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			writer.AddAttribute(HtmlTextWriterAttribute.Class, "modal-content");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
		}

		private void RenderEndTags(HtmlTextWriter writer)
		{
			writer.RenderEndTag(); // modal-content
			writer.RenderEndTag(); // modal-dialog
			writer.RenderEndTag(); // modal
		}
	}
}
