using System.Web.UI;
using System.Reflection;

namespace Havit.Web.UI;

/// <summary>
/// Updatepanel, který napravuje vlastnost .NETu, podle které není možno odebrat ze stránky UpdatePanel a znovu jej přidat
/// (například databindingem), pokud je update panel uvnitř jiného UpdateOanelu s režimem Conditional.
/// </summary>
public class UpdatePanelExt : UpdatePanel
{
	/// <summary>
	/// Přiznak, zda již proběhl PreRenderComplete.
	/// </summary>
	private bool pagePreRenderCompleted = false;

	/// <summary>
	/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
	/// </summary>
	/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param><exception cref="T:System.InvalidOperationException">The <see cref="P:System.Web.UI.UpdatePanel.ContentTemplate"/> property is being defined when the <see cref="P:System.Web.UI.UpdatePanel.ContentTemplateContainer"/> property has already been created.</exception>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
	}

	private void Page_PreRenderComplete(object sender, EventArgs e)
	{
		// Zaznamenáme si, že již proběhl PreRenderComplete.
		pagePreRenderCompleted = true;
	}

	/// <summary>
	/// Raises the base <see cref="E:System.Web.UI.Control.Unload"/> event.
	/// </summary>
	/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains event data.</param>
	protected override void OnUnload(EventArgs e)
	{
		// pokud ještě neproběhl PreRenderCompleted (tj. nejde o standardní OnUnload na konci životního cylku stránky, ale jen vyhození controlu ze stromu controlů)
		if (!pagePreRenderCompleted && (this.Page != null))
		{
			ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
			if (scriptManager != null)
			{
				// jako fix zavoláme metodu RegisterUpdatePanel, aby mohl korektně proběhnout OnUload, ve kterém je odregistrace controlu
				MethodInfo methodInfo = typeof(ScriptManager).GetMethod("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel", BindingFlags.NonPublic | BindingFlags.Instance);
				methodInfo.Invoke(scriptManager, new object[] { this });
			}
		}
		base.OnUnload(e);
	}
}
