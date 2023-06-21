using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI.ClientScripts;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Control zajišťuje, aby bylo zachováno pořadí controlů při ovládání klávesnicí a klávesou TAB i v případě, že na některém z prvků dojde k AutoPostBacku.
/// Vyžaduje nuget balík jQuery.UI.Combined - "jQuery UI (Combined Library)".
/// </summary>
public class ControlFocusPersister : Control
{
	/// <summary>
	/// Zaregistruje hidden field, ve kterém se drží poslední control, na kterém byl focus.
	/// </summary>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		Page.ClientScript.RegisterHiddenField("_lastFocusPersister", this.Context.Request.Form["_lastFocusPersister"] ?? String.Empty);
	}

	/// <summary>
	/// Zaregistruje javascript zajišťující chování ControlFocusPersisteru.
	/// </summary>
	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		HavitFrameworkClientScriptHelper.RegisterHavitFrameworkClientScript(Page);
		ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(Page, "jquery.ui.combined");

		ScriptManager.RegisterStartupScript(this.Page, typeof(ControlFocusPersister), "ControlFocusPersister", "$(document).ready(function () { havitControlFocusPersisterExtensions.init(); });", true);
	}
}