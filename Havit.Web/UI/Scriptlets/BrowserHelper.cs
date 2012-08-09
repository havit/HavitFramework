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
    public static class BrowserHelper
    {
        /// <summary>
        /// Vrací true, pokud byl aktuální HttpRequest pochází z Internet Exploreru 
        /// (nebo shodnì se identifikujícího browseru).
        /// </summary>
        public static bool IsInternetExplorer
        {
            get
            {
                return HttpContext.Current.Request.Browser.Browser == "IE";
            }
        }

        /// <summary>
        /// Vrátí pøíkaz pro pøipojení události k objektu.
        /// Detekuji IE, který pøipojuje události jinak než ostatní prohlížeèe.
        /// </summary>
        /// <param name="attachingObject">Objekt, ke kterému je pøipojována událost.</param>
        /// <param name="eventName">Název události vè "on", napø "onchange", "onclick".</param>
        /// <param name="function">Funkce, která je pøipojována. Obsahuje-li øetìzev, musí být uvozen uvozovkami a nikoliv apostrofy.</param>
        /// <returns>Pøíkaz pøipojující událost k objektu.</returns>
        public static string GetAttachEvent(string attachingObject, string eventName, string function)
        {
            if (function.Contains("("))
                function = String.Format("new Function(\'{0}\')", function);

            if (IsInternetExplorer)
                return String.Format("{0}.attachEvent(\"{1}\", {2});", attachingObject, eventName, function);
            else
                return String.Format("{0}.addEventListener(\"{1}\", {2}, false);", attachingObject, eventName.Substring(2), function);
        }
    }
}