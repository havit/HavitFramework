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
	/// Control extender, který umí pracovat s Repeaterem.
	/// </summary>
    public class RepeaterControlExtender: IControlExtender
    {
		private int priority;

		/// <summary>
		/// Vytvoøí extender s danou prioritou.
		/// </summary>
		/// <param name="priority">Priorita extenderu.</param>
		public RepeaterControlExtender(int priority)
		{
			this.priority = priority;
		}
		
		/// <summary>
		/// Vrátí prioritu extenderu pro daný control.
		/// Pokud je control Repeaterem, vrátí prioritu zadanou v konstruktoru,
		/// jinak vrací null.
		/// </summary>
		/// <param name="control">Control, pro který se ovìøuje priorita.</param>
		/// <returns>Priorita.</returns>
        public int? GetPriority(Control control)
        {
            return (control is Repeater) ? (int?)priority : null;                
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
            scriptBuilder.AppendFormat("{0}.{1} = new Array();\n", parameterPrefix, parameter.Name);
            Repeater repeater = (Repeater)control;

            int index = 0;
            foreach (RepeaterItem item in repeater.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {

					string newParameterPrefix = String.Format("{0}.{1}[{2}]", parameterPrefix, parameter.Name, index);
					scriptBuilder.AppendFormat("{0} = new Object();\n", newParameterPrefix);
					
					foreach (Control nestedControl in ((Control)parameter).Controls)
                    {
                        ParameterBase nestedParameter = nestedControl as ParameterBase;
                        if (nestedParameter == null)
                            continue;
                        nestedParameter.CreateParameter(newParameterPrefix, item, scriptBuilder);
                    }
                    index++;
                }
            }
        }
    }
 
}