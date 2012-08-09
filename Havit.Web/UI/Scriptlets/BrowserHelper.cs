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
    /// Pomocník pro identifikaci prohlížeèe.
    /// </summary>
    internal static class BrowserHelper
    {
		#region IsInternetExplorer
		/// <summary>
		/// Vrací <c>true</c>, pokud byl aktuální <see cref="System.Web.HttpRequest">HttpRequest</see> pochází z Internet Exploreru 
		/// (nebo shodnì se identifikujícího browseru).
		/// </summary>
		public static bool IsInternetExplorer
		{
			get
			{
				return HttpContext.Current.Request.Browser.Browser == "IE";
			}
		}
		#endregion

		#region GetAttachEventScript
		/// <summary>
		/// Vrátí pøíkaz pro pøipojení události k objektu.
		/// Detekuji IE, který pøipojuje události jinak než ostatní prohlížeèe.
		/// </summary>
		/// <param name="attachingObject">Objekt, ke kterému je pøipojována událost.</param>
		/// <param name="eventName">Název události vèetnì "on", napøíklad "onchange", "onclick", atp.</param>
		/// <param name="function">Funkce, která je pøipojována. Obsahuje-li øetìzec, musí být uvozen uvozovkami a nikoliv apostrofy.</param>
		/// <returns>Pøíkaz pøipojující událost k objektu.</returns>
		public static string GetAttachEventScript(string attachingObject, string eventName, string function)
		{
			if (function.Contains("("))
			{
				function = String.Format("new Function(\'{0}\')", function);
			}

			if (IsInternetExplorer)
			{
				return String.Format("{0}.attachEvent(\"{1}\", {2});", attachingObject, eventName, function);
			}
			else
			{
				return String.Format("{0}.addEventListener(\"{1}\", {2}, false);", attachingObject, eventName.Substring(2), function);
			}
		}
		#endregion

		#region GetDetachEventScript
		/// <summary>
		/// Vrátí pøíkaz pro odpojení události od objektu.
		/// Detekuji IE, který odpojuje události jinak než ostatní prohlížeèe.
		/// </summary>
		/// <param name="detachingObject">Objekt, od kterého je odpojována událost.</param>
		/// <param name="eventName">Název události vè "on", napø "onchange", "onclick".</param>
		/// <param name="function">Funkce, která je odpojována. Obsahuje-li øetìzec, musí být uvozen uvozovkami a nikoliv apostrofy.</param>
		/// <returns>Pøíkaz odpojující událost od objektu.</returns>
		public static string GetDetachEventScript(string detachingObject, string eventName, string function)
		{
			if (function.Contains("("))
			{
				function = String.Format("new Function(\'{0}\')", function);
			}

			if (IsInternetExplorer)
			{
				return String.Format("{0}.detachEvent(\"{1}\", {2});", detachingObject, eventName, function);
			}
			else
			{
				return String.Format("{0}.removeEventListener(\"{1}\", {2}, false);", detachingObject, eventName.Substring(2), function);
			}
		}
		#endregion
		
		internal delegate string GetAttachDetachEventScriptEventHandler(string manipulatingObject, string eventName, string function);
	}
}