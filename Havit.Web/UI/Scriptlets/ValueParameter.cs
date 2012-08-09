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
		#region Value
		/// <summary>
		/// Hodnota parametru.
		/// </summary>
		public string Value
		{
			get { return (string)(ViewState["Value"] ?? String.Empty); }
			set { ViewState["Value"] = value; }
		}		
		#endregion

		#region GetInitializeClientSideValueScript
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetInitializeClientSideValueScript")]/*' />
		public override void GetInitializeClientSideValueScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder)
        {
            scriptBuilder.AppendFormat("{0}.{1} = '{2}';\n", parameterPrefix, Name, Value.Replace("'", "\\'"));
        }
		#endregion

		#region GetAttachEventsScript
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetAttachEventsScript")]/*' />
		public override void GetAttachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			//NOOP
		}
		#endregion

		#region GetDetachEventsScript
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetDetachEventsScript")]/*' />
		public override void GetDetachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			//NOOP
		}
		#endregion
	}
}