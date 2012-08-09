using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Havit.Web.UI.Scriptlets
{
	/// <summary>
	/// Interface pro substituce v klientském skriptu scriptletu.
	/// </summary>
    public interface IScriptSubstitution
    {
		#region Substitute
		/// <summary>
		/// Substituje ve skriptu.
		/// </summary>
		/// <param name="script">Skript, ve kterém má dojít k substituci.</param>
		/// <returns>Substituovaný skript.</returns>
        string Substitute(string script);
		#endregion
	}
}