using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Control extender pro jednoduch� Controly (WebControls).
    /// Extender tvo�� skript jen tak, �e najde p��slu�n� element
    /// ve str�nce a pou�ije jej jako hodnotu parametru.    
    /// </summary>
    public class SimpleControlExtender : IControlExtender
    {
		#region Private fields
		private Type controlType;
		private int priority;
		private string[] changeEvents;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Vytvo�� extender pro dan� typ s danou prioritou.
		/// </summary>
		/// <param name="controlType">Typ, kter� bude tato instance um�t �e�it.</param>
		/// <param name="priority">Priorita, s jakou jej bude �e�it.</param>
		/// <param name="changeEvents">Ud�losti, na kter� je pot�eba se nav�zat pokud m� b�t klientsk� skript vyvol�n v p��pad� zm�ny. Null znamen�, �e pro tento typ controlu nejsou changeEvents podporov�ny.</param>
		public SimpleControlExtender(Type controlType, int priority, string[] changeEvents)
		{
			this.controlType = controlType;
			this.priority = priority;
			this.changeEvents = changeEvents;
		}
		#endregion
		
		#region GetPriority
		/// <summary>
		/// Vrac� priotitu vhodnosti extenderu pro zpracov�n� controlu.
		/// Pokud extender nen� vhodn� pro zpracov�n� controlu, vrac� null.
		/// </summary>
		/// <param name="control">Ov��ovan� control.</param>
		/// <returns>Priorita.</returns>
		public virtual int? GetPriority(Control control)
		{
			return (this.controlType.IsAssignableFrom(control.GetType())) ? (int?)this.priority : null;
		}
		#endregion

		#region GetInitializeClientSideValueScript
		/// <summary>
        /// Vytvo�� klientsk� parametr pro p�edan� control.
        /// </summary>
        /// <param name="parameterPrefix">N�zev objektu na klientsk� stran�.</param>
        /// <param name="parameter">Parametr p�ed�vaj�c� ��zen� extenderu.</param>
        /// <param name="control">Control ke zpracov�n�.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		public void GetInitializeClientSideValueScript(string parameterPrefix, IScriptletParameter parameter, Control control, ScriptBuilder scriptBuilder)
        {
            // vytvo��me objekt
			scriptBuilder.AppendFormat("{0}.{1} = document.getElementById(\"{2}\");\n", parameterPrefix, parameter.Name, control.ClientID);
        }
		#endregion

		#region GetAttachEventsScript
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetAttachEventsScript")]/*' />
		public void GetAttachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			GetEventsScript(BrowserHelper.GetAttachEventScript, parameterPrefix, parameter, control, scriptletFunctionCallDelegate, scriptBuilder);
		}
		
		#endregion

		#region GetDetachEventsScript
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetDetachEventsScript")]/*' />
		public void GetDetachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			GetEventsScript(BrowserHelper.GetDetachEventScript, parameterPrefix, parameter, control, scriptletFunctionCallDelegate, scriptBuilder);
		}
		#endregion
		
		#region GetEventsScript
		private void GetEventsScript(BrowserHelper.GetAttachDetachEventScriptEventHandler getEventScript, string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			// pokud se m� volat kliensk� skript p�i zm�n� hodnoty prvku
			if (((ControlParameter)parameter).StartOnChange)
			{
				// ov���me, zda jsou nastaveny ud�losti (pr�zd� pole sta��)
				if (changeEvents == null)
				{
					throw new HttpException("Parametr p�ikazuje spu�t�n� p�i zm�n� controlu, u extenderu v�ak nen� uvedena ��dn� ud�lost ke kter� bychom se m�li nav�zat.");
				}

				// pro v�echny ud�lost
				foreach (string eventName in changeEvents)
				{
					// vytvo��me skript, kter� danou ud�lost nav�e k elementu
					scriptBuilder.AppendLine(getEventScript.Invoke(
						String.Format("{0}.{1}", parameterPrefix, parameter.Name),
						eventName,
						scriptletFunctionCallDelegate
					));
				}
			}	
		} 
		#endregion
	}
}