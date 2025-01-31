using System.Web;

namespace Havit.Web.UI.Scriptlets;

/// <summary>
/// Pomocník pro identifikaci prohlížeče a přípravu browser-specific skriptů.
/// </summary>
internal static class BrowserHelper
{
	/// <summary>
	/// Vrací <c>true</c>, pokud byl aktuální <see cref="System.Web.HttpRequest">HttpRequest</see> pochází z Internet Exploreru 
	/// (nebo shodně se identifikujícího browseru).
	/// </summary>
	public static bool IsInternetExplorer
	{
		get
		{
			string browser = HttpContext.Current.Request.Browser.Browser;
			return (browser == "IE") || (browser == "InternetExplorer");
		}
	}

	/// <summary>
	/// Vrátí příkaz pro připojení události k objektu.
	/// Detekuji IE, který připojuje události jinak než ostatní prohlížeče.
	/// </summary>
	/// <param name="attachingObject">Objekt, ke kterému je připojována událost.</param>
	/// <param name="eventName">Název události včetně "on", například "onchange", "onclick", atp.</param>
	/// <param name="functionDelegateName">Delegát, který je připojován.</param>
	/// <returns>Příkaz připojující událost k objektu.</returns>
	public static string GetAttachEventScript(string attachingObject, string eventName, string functionDelegateName)
	{
		if (functionDelegateName.Contains("("))
		{
			throw new ArgumentException("Je nutné předat identifikátor proměnné nesoucí hodnotu delegáta.", "functionDelegateName");
			//function = String.Format("new Function(\'{0}\')", function);
		}

		return String.Format("$({0}).on('{1}', {2});", attachingObject, eventName.Substring(2), functionDelegateName);
	}

	/// <summary>
	/// Vrátí příkaz pro odpojení události od objektu.
	/// Detekuji IE, který odpojuje události jinak než ostatní prohlížeče.
	/// </summary>
	/// <param name="detachingObject">Objekt, od kterého je odpojována událost.</param>
	/// <param name="eventName">Název události vč "on", např "onchange", "onclick".</param>
	/// <param name="functionDelegateName">Delegát, který je připojován.</param>
	/// <returns>Příkaz odpojující událost od objektu.</returns>
	public static string GetDetachEventScript(string detachingObject, string eventName, string functionDelegateName)
	{
		if (functionDelegateName.Contains("("))
		{
			throw new ArgumentException("Je nutné předat identifikátor proměnné nesoucí hodnotu delegáta.", "functionDelegateName");
			//				function = String.Format("new Function(\'{0}\')", function);
		}

		return String.Format("$({0}).off('{1}', {2});", detachingObject, eventName.Substring(2), functionDelegateName);
	}

	/// <summary>
	/// Delegát funkcí GetAttachEventScript a GetDetachEventScript.
	/// </summary>
	/// <param name="manipulatingObject">Cílový objekt pro navázání/odvázání události.</param>
	/// <param name="eventName">Název události včetně "on", například "onchange", "onclick", atp.</param>
	/// <param name="functionDelegateName">Delegát, který je připojován.</param>
	/// <returns>Příkaz připojující událost k objektu.</returns>
	internal delegate string GetAttachDetachEventScriptEventHandler(string manipulatingObject, string eventName, string functionDelegateName);
}