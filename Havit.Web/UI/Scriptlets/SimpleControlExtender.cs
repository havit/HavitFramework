using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Control extender pro jednoduché Controly (WebControls).
    /// Extender tvoøí skript jen tak, že najde pøíslušný element
    /// ve stránce a použije jej jako hodnotu parametru.    
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
		/// Vytvoøí extender pro daný typ s danou prioritou.
		/// </summary>
		/// <param name="controlType">Typ, který bude tato instance umìt øešit.</param>
		/// <param name="priority">Priorita, s jakou jej bude øešit.</param>
		/// <param name="changeEvents">Události, na které je potøeba se navázat pokud má být klientský skript vyvolán v pøípadì zmìny. Null znamená, že pro tento typ controlu nejsou changeEvents podporovány.</param>
		public SimpleControlExtender(Type controlType, int priority, string[] changeEvents)
		{
			this.controlType = controlType;
			this.priority = priority;
			this.changeEvents = changeEvents;
		}
		#endregion
		
		#region GetPriority
		/// <summary>
		/// Vrací priotitu vhodnosti extenderu pro zpracování controlu.
		/// Pokud extender není vhodný pro zpracování controlu, vrací null.
		/// </summary>
		/// <param name="control">Ovìøovaný control.</param>
		/// <returns>Priorita.</returns>
		public virtual int? GetPriority(Control control)
		{
			return (this.controlType.IsAssignableFrom(control.GetType())) ? (int?)this.priority : null;
		}
		#endregion

		#region GetInitializeClientSideValueScript
		/// <summary>
        /// Vytvoøí klientský parametr pro pøedaný control.
        /// </summary>
        /// <param name="parameterPrefix">Název objektu na klientské stranì.</param>
        /// <param name="parameter">Parametr pøedávající øízení extenderu.</param>
        /// <param name="control">Control ke zpracování.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		public void GetInitializeClientSideValueScript(string parameterPrefix, IScriptletParameter parameter, Control control, ScriptBuilder scriptBuilder)
        {
            // vytvoøíme objekt
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
			// pokud se má volat klienský skript pøi zmìnì hodnoty prvku
			if (((ControlParameter)parameter).StartOnChange)
			{
				// ovìøíme, zda jsou nastaveny události (prázdé pole staèí)
				if (changeEvents == null)
				{
					throw new HttpException("Parametr pøikazuje spuštìní pøi zmìnì controlu, u extenderu však není uvedena žádná událost ke které bychom se mìli navázat.");
				}

				// pro všechny událost
				foreach (string eventName in changeEvents)
				{
					// vytvoøíme skript, který danou událost naváže k elementu
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