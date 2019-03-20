using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Parametr předávající do klientského objektu konstantní řetězcovou hodnotu.
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

	    /// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetInitializeClientSideValueScript")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		public override void GetInitializeClientSideValueScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder)
        {
            scriptBuilder.AppendFormat("{0}.{1} = '{2}';\n", parameterPrefix, Name, Value.Replace("'", "\\'"));
        }

	    /// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetAttachEventsScript")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		public override void GetAttachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			//NOOP
		}

	    /// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetDetachEventsScript")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		public override void GetDetachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			//NOOP
		}
    }
}