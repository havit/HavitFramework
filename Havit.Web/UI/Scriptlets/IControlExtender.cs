using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Control extender je zodpovìdný za vytoøení klienského skriptu pro 
    /// pøedaný control. Díky této obecnosti je možné pøidávat vlastní control
	/// extendery (èi mìnit existující) a tím upravit chování
	/// <see cref="Scriptlet">Scriptletu</see> pro další controly.
    /// </summary>
	public interface IControlExtender
	{
		#region GetPriority
		/// <summary>
		/// Vrací prioritu, s jakou je extender vhodný pro zpracování controlu.
		/// Pokud je extender nevhodný pro zpracování, vrácí se <c>null</c>.
		/// </summary>
		/// <param name="control">Control, který bude zpracováván.</param>
		/// <returns>Priorita extenderu.</returns>
		int? GetPriority(Control control);
		#endregion

		#region GetInitializeClientSideValueScript
		/// <summary>
		/// Vrátí skript pro inicializaci parametru hodnotou objektu na klientské stranì.
		/// </summary>
		/// <param name="parameterPrefix">Název objektu na klientské stranì.</param>
		/// <param name="parameter">Parametr pøedávající øízení extenderu.</param>
		/// <param name="control">Control ke zpracování.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		void GetInitializeClientSideValueScript(string parameterPrefix, IScriptletParameter parameter, Control control, ScriptBuilder scriptBuilder);		
		#endregion

		#region GetAttachEventsScript
		/// <summary>
		/// Vrátí skript pro navázání událostí k objektu na klientské stranì.
		/// </summary>
		/// <param name="parameterPrefix">Název objektu na klientské stranì.</param>
		/// <param name="parameter">Parametr pøedávající øízení extenderu.</param>
		/// <param name="control">Control ke zpracování.</param>
		/// <param name="scriptletFunctionCallDelegate">Delegát volání funkce scriptletu.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		void GetAttachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder);		
		#endregion

		#region GetDetachEventsScript
		/// <summary>
		/// Vrátí skript pro odpojení událostí od objektu na klientské stranì.
		/// </summary>
		/// <param name="parameterPrefix">Název objektu na klientské stranì.</param>
		/// <param name="parameter">Parametr pøedávající øízení extenderu.</param>
		/// <param name="control">Control ke zpracování.</param>
		/// <param name="scriptletFunctionCallDelegate">Delegát volání funkce scriptletu.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		void GetDetachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder); 
		#endregion
	}
}
