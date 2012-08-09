using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Reflection;

namespace Havit.Web.UI
{
	/// <summary>
	/// Updatepanel, který napravuje vlastnost .NETu, podle které není možno odebrat ze stránky UpdatePanel a znovu jej přidat
	/// (například databindingem), pokud je update panel uvnitř jiného UpdateOanelu s režimem Conditional.
	/// </summary>
	public class UpdatePanelExt: UpdatePanel
	{
		#region Private fields
		/// <summary>
		/// Přiznak, zda již proběhl PreRenderComplete.
		/// </summary>
		bool pagePreRenderCompleted = false;
		#endregion

		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);			
		}
		#endregion

		#region Page_PreRenderComplete
		void Page_PreRenderComplete(object sender, EventArgs e)
		{
			// Zaznamenáme si, že již proběhl PreRenderComplete.
			pagePreRenderCompleted = true;
		}
		#endregion

		protected override void OnUnload(EventArgs e)
		{
			// pokud ještě neproběhl PreRenderCompleted (tj. nejde o standardní OnUnload na konci životního cylku stránky, ale jen vyhození controlu ze stromu controlů)
			if (!pagePreRenderCompleted && (this.Page != null))
			{
				ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
				if (scriptManager != null)
				{
					// jako fix zavoláme metodu RegisterUpdatePanel, aby mohl korektně proběhnout OnUload, ve kterém je odregistrace controlu
					MethodInfo methodInfo = typeof (ScriptManager).GetMethod("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel", BindingFlags.NonPublic | BindingFlags.Instance);
					methodInfo.Invoke(scriptManager, new object[] {this});
				}
			}
			base.OnUnload(e);
		}
	}
}
