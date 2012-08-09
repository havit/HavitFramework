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
    /// Parametr obsažený ve scripletu.
    /// </summary>
    public interface IScriptletParameter
    {
        /// <summary>
        /// Zpøístupòuje scriptlet, ve kterém je parametr obsažen.
        /// </summary>
        Scriptlet Scriptlet { get; }

        /// <summary>
        /// Název parametru, pod kterým bude identifikován v klientském skriptu.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Zkontroluje nastavení parametru. Je-li nìjaké nastavení chybnì,
        /// má být vyhozena výjimka.
        /// </summary>
        void CheckProperties();

        /// <summary>
        /// Vytvoøí klientský skript pro parametr.
        /// </summary>
        /// <param name="parameterPrefix">Prefix pro název parametru. Controly mohou být složené (napø. TextBox v Repeateru).</param>
        /// <param name="parentControl">Rodièovský prvek, pro který je parametr renderován.</param>
		/// <param name="scriptBuilder">Script builder.</param>
        void CreateParameter(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder);
    }
}
