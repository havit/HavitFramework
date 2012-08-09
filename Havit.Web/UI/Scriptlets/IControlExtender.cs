using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets
{
	/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"T:Havit.Web.UI.Scriptlets.IControlExtender")]/*' />   
	public interface IControlExtender
	{
		#region GetPriority
		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetPriority")]/*' />
		int? GetPriority(Control control);
		#endregion

		#region GetInitializeClientSideValueScript
		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetInitializeClientSideValueScript")]/*' />
		void GetInitializeClientSideValueScript(string parameterPrefix, IScriptletParameter parameter, Control control, ScriptBuilder scriptBuilder);		
		#endregion

		#region GetAttachEventsScript
		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetAttachEventsScript")]/*' />
		void GetAttachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder);		
		#endregion

		#region GetDetachEventsScript
		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetDetachEventsScript")]/*' />
		void GetDetachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder); 
		#endregion
	}
}
