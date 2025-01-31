using System.ComponentModel;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Label with ToolTip displayed as Bootstrap tooltip.
/// </summary>
[ParseChildren(false)]
[PersistChildren(true)]
public class ToolTip : System.Web.UI.WebControls.WebControl
{
	/// <summary>
	/// ToolTip position.
	/// </summary>
	[DefaultValue(ToolTipPosition.Top)]
	public ToolTipPosition ToolTipPosition
	{
		get
		{
			return (ToolTipPosition)(ViewState["ToolTipPosition"] ?? ToolTipPosition.Top);
		}
		set
		{
			ViewState["ToolTipPosition"] = value;
		}
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public ToolTip()
		: base(HtmlTextWriterTag.Span)
	{
	}

	/// <summary>
	/// Adds to the specified writer those HTML attributes and styles that need to be rendered.
	/// </summary>
	protected override void AddAttributesToRender(HtmlTextWriter writer)
	{
		if (!String.IsNullOrEmpty(ToolTip))
		{
			writer.AddAttribute("data-toggle", "tooltip");
			writer.AddAttribute("data-placement", ToolTipPosition.ToString().ToLower());
		}
		base.AddAttributesToRender(writer);
	}

	/// <summary>
	/// Raises the PreRender event. This method uses event arguments to pass the event data to the control.
	/// </summary>
	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		ScriptManager.RegisterStartupScript(this, typeof(ToolTip), "ToolTipInitialization", @"$(function() { $('[data-toggle=""tooltip""]').tooltip(); });", true);
	}
}
