using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets
{
	/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"T:Havit.Web.UI.Scriptlets.IControlExtender")]/*' />   
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public interface IControlExtender
	{
		#region GetPriority
		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetPriority")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		int? GetPriority(Control control);
		#endregion

		#region GetInitializeClientSideValueScript
		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetInitializeClientSideValueScript")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		void GetInitializeClientSideValueScript(string parameterPrefix, IScriptletParameter parameter, Control control, ScriptBuilder scriptBuilder);		
		#endregion

		#region GetAttachEventsScript
		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetAttachEventsScript")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		void GetAttachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder);		
		#endregion

		#region GetDetachEventsScript
		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetDetachEventsScript")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		void GetDetachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder); 
		#endregion
	}
}
