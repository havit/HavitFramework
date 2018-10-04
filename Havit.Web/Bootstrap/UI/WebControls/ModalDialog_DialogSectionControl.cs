using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using Havit.Diagnostics.Contracts;
using Havit.Web.UI;
using Havit.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	public partial class ModalDialog
	{
		/// <summary>
		/// Control for rendering control section (header, footer, content).
		/// </summary>
		internal class DialogSectionControl : Control
		{
			#region Private fields
			private readonly Func<string> getSectionCssClass;
			#endregion

			#region Constructor
			/// <summary>
			/// Constructor.
			/// </summary>
			public DialogSectionControl(Func<string> getSectionCssClass)
			{
				Contract.Assert(getSectionCssClass != null);
				this.getSectionCssClass = getSectionCssClass;
			}
			#endregion

			#region Render
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
			#endregion
		}
	}
}
