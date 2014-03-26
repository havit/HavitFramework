using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Havit.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Label with ToolTip displayed as Bootstrap tooltip.
	/// </summary>
	[ParseChildren(false), PersistChildren(true)]
	public class Tooltip : System.Web.UI.WebControls.WebControl
	{
		#region TooltipPosition
		/// <summary>
		/// Tooltip position.
		/// </summary>
		public TooltipPosition TooltipPosition
		{
			get
			{
				return (TooltipPosition)(ViewState["TooltipPosition"] ?? TooltipPosition.Top);
			}
			set
			{
				ViewState["TooltipPosition"] = value;
			}
		}
		#endregion

		#region Constuctor
		/// <summary>
		/// Constructor.
		/// </summary>
		public Tooltip()
			: base(HtmlTextWriterTag.Span)
		{
		}
		#endregion

		#region AddAttributesToRender
		/// <summary>
		/// Adds to the specified writer those HTML attributes and styles that need to be rendered.
		/// </summary>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (!String.IsNullOrEmpty(ToolTip))
			{
				writer.AddAttribute("data-toggle", "tooltip");
				writer.AddAttribute("data-placement", TooltipPosition.ToString().ToLower());
			}
			base.AddAttributesToRender(writer);
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Raises the PreRender event. This method uses event arguments to pass the event data to the control.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ScriptManager.RegisterStartupScript(this, typeof(Tooltip), "TooltipInitialization", @"$(function() { $('[data-toggle=""tooltip""]').tooltip(); });", true);
		}
		#endregion
	}
}
