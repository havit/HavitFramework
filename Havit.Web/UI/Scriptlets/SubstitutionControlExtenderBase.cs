using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets
{
	/// <summary>
	/// ControlExtender urèený ke tvorbì potomkù, kteøí provádìjí substituci za jiný control.
	/// Napø. pokud je potøeba substituovat UserControl za nìkterý vnoøený control.
	/// </summary>
	public abstract class SubstitutionControlExtenderBase: IControlExtender
	{
		#region GetPriority
		/// <summary>
		/// Vrací priotitu vhodnosti extenderu pro zpracování controlu.
		/// Pokud extender není vhodný pro zpracování controlu, vrací null.
		/// </summary>
		/// <param name="control">Ovìøovaný control.</param>
		/// <returns>Priorita.</returns>
		public int? GetPriority(Control control)
		{
			return (this.GetSupportedControlType().IsAssignableFrom(control.GetType())) ? (int?)this.GetPriorityValue: null;
		}
		#endregion
		
		#region IControlExtender Members
		/// <summary>
		/// Vytvoøí klientský parametr pro pøedaný control.
		/// </summary>
		/// <param name="parameterPrefix">Název objektu na klientské stranì.</param>
		/// <param name="parameter">Parametr pøedávající øízení extenderu.</param>
		/// <param name="control">Control ke zpracování.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		public void GetInitializeClientSideValueScript(string parameterPrefix, IScriptletParameter parameter, System.Web.UI.Control control, ScriptBuilder scriptBuilder)
		{
			Control substitutedControl = GetSubstitutedControl(control);
			parameter.Scriptlet.ControlExtenderRepository.FindControlExtender(substitutedControl).GetInitializeClientSideValueScript(parameterPrefix, parameter, substitutedControl, scriptBuilder);
		}

		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetAttachEventsScript")]/*' />
		public void GetAttachEventsScript(string parameterPrefix, IScriptletParameter parameter, System.Web.UI.Control control, ScriptBuilder scriptBuilder)
		{
			Control substitutedControl = GetSubstitutedControl(control);
			parameter.Scriptlet.ControlExtenderRepository.FindControlExtender(substitutedControl).GetAttachEventsScript(parameterPrefix, parameter, substitutedControl, scriptBuilder);
		}
		
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetDetachEventsScript")]/*' />
		public void GetDetachEventsScript(string parameterPrefix, IScriptletParameter parameter, System.Web.UI.Control control, ScriptBuilder scriptBuilder)
		{
			Control substitutedControl = GetSubstitutedControl(control);
			parameter.Scriptlet.ControlExtenderRepository.FindControlExtender(substitutedControl).GetDetachEventsScript(parameterPrefix, parameter, substitutedControl, scriptBuilder);
		}
		#endregion

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

		#region GetSubstitutedControl (abstract)
		/// <summary>
		/// Vrací substituovaný control.
		/// </summary>
		protected abstract Control GetSubstitutedControl(Control control);
		#endregion

		#region GetSupportedControlType (abstract)
		/// <summary>
		/// Vrací typ, který je tøídou podporován k substituci.
		/// </summary>
		protected abstract Type GetSupportedControlType();
		#endregion
	}
}
