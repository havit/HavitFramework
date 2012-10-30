using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.UI;
using Havit.Web.Management;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Control zajišťuje funkci health monitoringu i v asychronním postbacku (což standardně v ASP.NET nefunguje, bohužel).
	/// Řešeno obsluhou události AsyncPostBackError ScriptManageru.
	/// </summary>
	public class AjaxHealthMonitoring : Control
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
			if (scriptManager == null)
			{
				throw new InvalidOperationException("Ve stránce nebyl nalezen ScriptManager, který je controlem AjaxHealthMonitoring vyžadován.");
			}
			scriptManager.AsyncPostBackError += new EventHandler<AsyncPostBackErrorEventArgs>(ScriptManager_AsyncPostBackError);
		}
		#endregion

		#region ScriptManager_AsyncPostBackError
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
		#endregion
	}
}
