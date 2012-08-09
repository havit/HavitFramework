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
	/// Parametr obsažený ve <see cref="Scriptlet">Scripletu</see>.
	/// </summary>
	public interface IScriptletParameter
	{
		#region Scriptlet
		/// <summary>
		/// Zpøístupòuje scriptlet, ve kterém je parametr obsažen.
		/// </summary>
		Scriptlet Scriptlet { get; }
		#endregion

		#region Name
		/// <summary>
		/// Název parametru, pod kterým bude identifikován v klientském skriptu.
		/// </summary>
		string Name { get; }
		#endregion

		#region CheckProperties
		/// <summary>
		/// Zkontroluje nastavení parametru. Je-li nìjaké nastavení chybnì,
		/// má být vyhozena výjimka.
		/// </summary>
		void CheckProperties();
		#endregion

		#region GetInitializeClientSideValueScript
		/// <summary>
		/// Vrátí skript pro inicializaci hodnoty parametru na klientské stranì.
		/// </summary>
		/// <param name="parameterPrefix">Prefix pro název parametru. Parametry mohou být vnoøené (napø. TextBox v Repeateru).</param>
		/// <param name="parentControl">Rodièovský prvek, pro který je parametr renderován.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		void GetInitializeClientSideValueScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder);
		#endregion

		#region GetAttachEventsScript
		/// <summary>
		/// Vrátí skript pro navázání událostí k objektu na klientské stranì.
		/// </summary>
		/// <param name="parameterPrefix">Prefix pro název parametru. Parametry mohou být vnoøené (napø. TextBox v Repeateru).</param>
		/// <param name="parentControl">Rodièovský prvek, pro který je parametr renderován.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		void GetAttachEventsScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder);
		#endregion
		
		#region GetDetachEventsScript
		/// <summary>
		/// Vrátí skript pro odpojení událostí od objektu na klientské stranì.
		/// </summary>
		/// <param name="parameterPrefix">Prefix pro název parametru. Parametry mohou být vnoøené (napø. TextBox v Repeateru).</param>
		/// <param name="parentControl">Rodièovský prvek, pro který je parametr renderován.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		void GetDetachEventsScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder);
		#endregion
	}
}
