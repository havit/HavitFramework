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
	/// Parametr obsažený ve <see cref="Scriptlet">Scripletu</see>.
	/// </summary>
	public interface IScriptletParameter
	{
		#region Scriptlet
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"P:Havit.Web.UI.Scriptlets.IScriptletParameter.Scriptlet")]/*' />
		Scriptlet Scriptlet { get; }
		#endregion

		#region Name
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"P:Havit.Web.UI.Scriptlets.IScriptletParameter.Name")]/*' />
		string Name { get; }
		#endregion

		#region CheckProperties
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.CheckProperties")]/*' />
		void CheckProperties();
		#endregion

		#region GetInitializeClientSideValueScript
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetInitializeClientSideValueScript")]/*' />
		void GetInitializeClientSideValueScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder);
		#endregion

		#region GetAttachEventsScript
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetAttachEventsScript")]/*' />
		void GetAttachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder);
		#endregion
		
		#region GetDetachEventsScript
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetDetachEventsScript")]/*' />
		void GetDetachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder);
		#endregion
	}
}
