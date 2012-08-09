using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Control extender pro jednoduché Controly (WebControls).
    /// Extender tvoøí skript jen tak, že najde pøíslušný element
    /// ve stránce a použije jej jako hodnotu parametru.    
    /// </summary>
    public class SimpleControlExtender : IControlExtender
    {
        private Type controlType;
        private int priority;
        private string[] changeEvents;        

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

        /// <summary>
        /// Vytvoøí klientský parametr pro pøedaný control.
        /// </summary>
        /// <param name="parameterPrefix">Název objektu na klientské stranì.</param>
        /// <param name="parameter">Parametr pøedávající øízení extenderu.</param>
        /// <param name="control">Control ke zpracování.</param>
		/// <param name="scriptBuilder">Script builder.</param>
        public void CreateParameter(string parameterPrefix, IScriptletParameter parameter, Control control, ScriptBuilder scriptBuilder)
        {
            // vytvoøíme objekt
            scriptBuilder.AppendFormat("{0}.{1} = document.getElementById(\"{2}\");\n", parameterPrefix, parameter.Name, control.ClientID);

            // pokud se má volat klienský skript pøi zmìnì hodnoty prvku
            if (((ControlParameter)parameter).StartOnChange)
            {
                // ovìøíme, zda jsou nastaveny události (prázdé pole staèí)
                if (changeEvents == null)
                    throw new ArgumentException("Parametr pøikazuje spuštìní pøi zmìnì controlu, u extenderu však není uvedena žádná událost ke které bychom se mìli navázat.");

                // pro všechny událost
                foreach (string eventName in changeEvents)
                {
                    // vytvoøíme skript, který danou událost naváže k elementu
                    scriptBuilder.Append(BrowserHelper.GetAttachEvent(
                        String.Format("{0}.{1}", parameterPrefix, parameter.Name),
                        eventName,
                        parameter.Scriptlet.ClientSideFunctionCall
                    ));
                    scriptBuilder.Append("\n");
                }
            }
        }
    }
}