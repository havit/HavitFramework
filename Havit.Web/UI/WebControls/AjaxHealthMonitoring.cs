using System.Web;
using System.Web.UI;
using Havit.Diagnostics.Contracts;
using Havit.Web.Management;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Control zajišťuje funkci health monitoringu i v asychronním postbacku (což standardně v ASP.NET nefunguje, bohužel).
/// Řešeno obsluhou události AsyncPostBackError ScriptManageru.
/// </summary>
public class AjaxHealthMonitoring : Control
{
	/// <summary>
	/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
	/// </summary>
	/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data. </param>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

		ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
		Contract.Requires<InvalidOperationException>(scriptManager != null, "Ve stránce nebyl nalezen ScriptManager, který je controlem AjaxHealthMonitoring vyžadován.");

		scriptManager.AsyncPostBackError += new EventHandler<AsyncPostBackErrorEventArgs>(ScriptManager_AsyncPostBackError);
	}

	/// <summary>
	/// Obsluha události AsyncPostBackError ScriptManageru. Zajistí vyvolání události health monitoringu.
	/// </summary>
	private void ScriptManager_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
	{
		if (e.Exception != null)
		{
			new WebRequestErrorEventExt(e.Exception.Message, this, e.Exception, HttpContext.Current).Raise();
		}
	}
}
