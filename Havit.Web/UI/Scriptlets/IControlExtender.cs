using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Control extender je zodpovìdný za vytoøení klienského skriptu pro 
    /// pøedaný control. Díky této obecnosti je možné pøidávat vlastní control
    /// extendery (èi mìnit existující) a tím upravit chování Scriptletu pro
    /// další controly.
    /// </summary>
	public interface IControlExtender
	{
        /// <summary>
        /// Vrací prioritu, s jakou je extender chodný pro zpracování controlu.
        /// Pokud je extender nevhodný pro zpracování, vrácí se null.
        /// </summary>
        /// <param name="control">Control, který bude zpracováván.</param>
        /// <returns>Priorita extenderu.</returns>
        int? GetPriority(Control control);

        /// <summary>
        /// Vytvoøí klientský parametr pro pøedaný control.
        /// </summary>
        /// <param name="parameterPrefix">Název objektu na klientské stranì.</param>
        /// <param name="parameter">Parametr pøedávající øízení extenderu.</param>
        /// <param name="control">Control ke zpracování.</param>
        /// <param name="builder">Script builder.</param>
        void CreateParameter(string parameterPrefix, IScriptletParameter parameter, Control control, ScriptBuilder builder);
    }
}
