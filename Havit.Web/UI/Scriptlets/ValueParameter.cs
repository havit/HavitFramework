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
    /// Parametr pøedávající do klientského objektu konstantní øetìzcovou hodnotu.
    /// </summary>
    public class ValueParameter : ParameterBase
    {
        /// <summary>
        /// Hodnota parametru.
        /// </summary>
        public string Value
        {
            get { return (string)(ViewState["Value"] ?? String.Empty); }
            set { ViewState["Value"] = value; }
        }

		/// <summary>
		/// Vytvoøí klientský skript pro parametr.
		/// </summary>
		/// <param name="parameterPrefix">Prefix parametru.</param>
		/// <param name="parentControl">Control, v rámci kterého je tento parametr.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		public override void CreateParameter(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder)
        {
            scriptBuilder.AppendFormat("{0}.{1} = '{2}';\n", parameterPrefix, Name, Value);
        }
    }
}