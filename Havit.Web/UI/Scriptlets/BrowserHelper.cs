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
    /// Pomocník pro identifikaci prohlížeèe a pøípravu browser-specific skriptù.
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
		/// <param name="functionDelegateName">Delegát, který je pøipojován.</param>
		/// <returns>Pøíkaz pøipojující událost k objektu.</returns>
		public static string GetAttachEventScript(string attachingObject, string eventName, string functionDelegateName)
		{
			if (functionDelegateName.Contains("("))
			{
				throw new ArgumentException("Je nutné pøedat identifikátor promìnné nesoucí hodnotu delegáta.", "functionDelegateName");
				//function = String.Format("new Function(\'{0}\')", function);
			}

			if (IsInternetExplorer)
			{
				return String.Format("{0}.attachEvent(\"{1}\", {2});", attachingObject, eventName, functionDelegateName);
			}
			else
			{
				return String.Format("{0}.addEventListener(\"{1}\", {2}, false);", attachingObject, eventName.Substring(2), functionDelegateName);
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
		/// <param name="functionDelegateName">Delegát, který je pøipojován.</param>
		/// <returns>Pøíkaz odpojující událost od objektu.</returns>
		public static string GetDetachEventScript(string detachingObject, string eventName, string functionDelegateName)
		{
			if (functionDelegateName.Contains("("))
			{
				throw new ArgumentException("Je nutné pøedat identifikátor promìnné nesoucí hodnotu delegáta.", "functionDelegateName");
				//				function = String.Format("new Function(\'{0}\')", function);
			}

			if (IsInternetExplorer)
			{
				return String.Format("{0}.detachEvent(\"{1}\", {2});", detachingObject, eventName, functionDelegateName);
			}
			else
			{
				return String.Format("{0}.removeEventListener(\"{1}\", {2}, false);", detachingObject, eventName.Substring(2), functionDelegateName);
			}
		}
		#endregion

		#region GetAttachDetachEventScriptEventHandler (internal)
		/// <summary>
		/// Delegát funkcí GetAttachEventScript a GetDetachEventScript.
		/// </summary>
		/// <param name="manipulatingObject">Cílový objekt pro navázání/odvázání události.</param>
		/// <param name="eventName">Název události vèetnì "on", napøíklad "onchange", "onclick", atp.</param>
		/// <param name="functionDelegateName">Delegát, který je pøipojován.</param>
		/// <returns>Pøíkaz pøipojující událost k objektu.</returns>
		internal delegate string GetAttachDetachEventScriptEventHandler(string manipulatingObject, string eventName, string functionDelegateName);
		#endregion

	}
}