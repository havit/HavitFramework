using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets
{
	/// <summary>
	/// ControlExtender určený ke tvorbě potomků, kteří provádějí substituci za jiný control.
	/// Např. pokud je potřeba substituovat UserControl za některý vnořený control.
	/// </summary>
	public abstract class SubstitutionControlExtenderBase : IControlExtender
	{
		#region GetPriorityValue (virtual)
		/// <summary>
		/// Vrátí hodnotu priority ControlExtenderu, která se použije, pokud je 
		/// ControlExtender použitelný pro zpracování controlu.
		/// </summary>
		protected virtual int GetPriorityValue
		{
			get
			{
				return 100;
			}
		}
		#endregion

		#region GetPriority
		/// <summary>
		/// Vrací priotitu vhodnosti extenderu pro zpracování controlu.
		/// Pokud extender není vhodný pro zpracování controlu, vrací null.
		/// </summary>
		/// <param name="control">Ověřovaný control.</param>
		/// <returns>Priorita.</returns>
		public int? GetPriority(Control control)
		{
			return (this.GetSupportedControlType().IsAssignableFrom(control.GetType())) ? (int?)this.GetPriorityValue : null;
		}
		#endregion
		
		#region IControlExtender Members
		/// <summary>
		/// Vytvoří klientský parametr pro předaný control.
		/// </summary>
		/// <param name="parameterPrefix">Název objektu na klientské straně.</param>
		/// <param name="parameter">Parametr předávající řízení extenderu.</param>
		/// <param name="control">Control ke zpracování.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		public void GetInitializeClientSideValueScript(string parameterPrefix, IScriptletParameter parameter, System.Web.UI.Control control, ScriptBuilder scriptBuilder)
		{
			Control substitutedControl = GetSubstitutedControl(control);
			parameter.Scriptlet.ControlExtenderRepository.FindControlExtender(substitutedControl).GetInitializeClientSideValueScript(parameterPrefix, parameter, substitutedControl, scriptBuilder);
		}

		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetAttachEventsScript")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		public void GetAttachEventsScript(string parameterPrefix, IScriptletParameter parameter, System.Web.UI.Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			Control substitutedControl = GetSubstitutedControl(control);
			parameter.Scriptlet.ControlExtenderRepository.FindControlExtender(substitutedControl).GetAttachEventsScript(parameterPrefix, parameter, substitutedControl, scriptletFunctionCallDelegate, scriptBuilder);
		}

		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetDetachEventsScript")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		public void GetDetachEventsScript(string parameterPrefix, IScriptletParameter parameter, System.Web.UI.Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			Control substitutedControl = GetSubstitutedControl(control);
			parameter.Scriptlet.ControlExtenderRepository.FindControlExtender(substitutedControl).GetDetachEventsScript(parameterPrefix, parameter, substitutedControl, scriptletFunctionCallDelegate, scriptBuilder);
		}
		#endregion

		#region GetSubstitutedControl (abstract)
		/// <summary>
		/// Vrací substituovaný control.
		/// </summary>
		protected abstract Control GetSubstitutedControl(Control control);
		#endregion

		#region GetSupportedControlType (abstract)
		/// <summary>
		/// Vrací typ, který je třídou podporován k substituci.
		/// </summary>
		protected abstract Type GetSupportedControlType();
		#endregion
	}
}
