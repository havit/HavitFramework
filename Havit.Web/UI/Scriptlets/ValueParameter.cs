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
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlParameter.GetInitializeClientSideValueScript")]/*' />
		public override void GetInitializeClientSideValueScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder)
        {
            scriptBuilder.AppendFormat("{0}.{1} = '{2}';\n", parameterPrefix, Name, Value);
        }
		#endregion

		#region CreateOnLoadScript
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlParameter.GetAttachEventsScript")]/*' />
		public override void GetAttachEventsScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder)
		{
			//NOOP
		}
		#endregion
		
		#region CreateOnUnLoadScript
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlParameter.GetDetachEventsScript")]/*' />
		public override void GetDetachEventsScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder)
		{
			//NOOP
		}
		#endregion
	}
}