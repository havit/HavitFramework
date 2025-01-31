using System.Web.UI;
using Havit.Diagnostics.Contracts;

namespace Havit.Web.Bootstrap.UI.WebControls;

public partial class ModalDialog
{
	/// <summary>
	/// Control for rendering control section (header, footer, content).
	/// </summary>
	internal class DialogSectionControl : Control
	{
		private readonly Func<string> getSectionCssClass;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DialogSectionControl(Func<string> getSectionCssClass)
		{
			Contract.Assert(getSectionCssClass != null);
			this.getSectionCssClass = getSectionCssClass;
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (HasControls())
			{
				string sectionCssClass = getSectionCssClass();
				writer.AddAttribute(HtmlTextWriterAttribute.Class, sectionCssClass);
				writer.RenderBeginTag(HtmlTextWriterTag.Div);

				base.Render(writer);

				writer.RenderEndTag(); // div.sectionCssClass
			}
		}
	}
}
