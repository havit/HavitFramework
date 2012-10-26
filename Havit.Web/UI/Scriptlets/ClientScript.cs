using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.Scriptlets
{
	/// <summary>
	/// Představuje klientský skript - funkci, která je vyvolána při změně vstupních
	/// parametrů <see cref="Scriptlet">Scriptletu</see> nebo při načtění stránky či callbacku (pokud je povoleno).
	/// </summary>
	//[DefaultProperty("Script")]
	[ParseChildren(true, "Script")]
	public class ClientScript : ScriptletNestedControl
	{

		/* Parametry ClientScriptu *************** */

		#region Script
		/// <summary>
		/// Klientský skript. Okolo skriptu je vytvořena obálka a případně je spuštěn po načtení stránky či callbacku.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public string Script
		{
			get { return (string)ViewState["Script"] ?? String.Empty; }
			set { ViewState["Script"] = value; }
		}
		#endregion

		#region StartOnLoad
		/// <summary>
		/// Udává, zda má po načtení stránky dojít k automatickému spuštění skriptu. Výchozí hodnota je <c>false</c>.
		/// </summary>				
		/// <remarks>
		/// Nemá vliv na zpracování stránky při asynchronním postbacku (callbacku). K tomu slouží vlastnost <see cref="StartOnAjaxCallback">StartOnAjaxCallback</see>.
		/// </remarks>
		public bool StartOnLoad
		{
			get { return (bool)(ViewState["StartOnLoad"] ?? false); }
			set { ViewState["StartOnLoad"] = value; }
		}
		#endregion

		#region StartOnAjaxCallback
		/// <summary>
		/// Udává, zda má po dojít k automatickému spuštění skriptu po asynchronním postbacku (ajax callback). Výchozí hodnota je <b>false</b>.
		/// </summary>
		public bool StartOnAjaxCallback
		{
			get { return (bool)(ViewState["StartOnAjaxCallback"] ?? false); }
			set { ViewState["StartOnAjaxCallback"] = value; }
		}		
		#endregion		

		/*  *************** */

		#region GetAutoStart
		/// <summary>
		/// Vrací true, pokud má být renderován skript pro automatické spuštění funkce scriptletu.
		/// </summary>
		public bool GetAutoStart()
		{
			if (this.Scriptlet.IsInAsyncPostBack)
			{
				return StartOnAjaxCallback;
			}
			else
			{
				return StartOnLoad;
			}
		}
		#endregion

		#region GetClientSideFunctionCode
		/// <summary>
		/// Vrátí kód pro hlavní funkci skriptletu.
		/// </summary>
		public virtual string GetClientSideFunctionCode()
		{
			return GetSubstitutedScript();
		}
		#endregion
		
		/* SUBSTITUCE *************** */

		#region GetSubstitutedScript (protected)
		/// <summary>
		/// Vrátí připravený klientský skript. Provede nad skriptem substituce.
		/// </summary>
		/// <returns>Klientský skript připravený k vytvoření obálky a registraci.</returns>
		protected virtual string GetSubstitutedScript()
		{
			ClientScriptSubstituingEventArgs eventArgs = new ClientScriptSubstituingEventArgs(Script);
			OnClientScriptSubstituing(eventArgs);
			return Scriptlet.ScriptSubstitution.Substitute(eventArgs.ClientScript);
		}
		#endregion

		#region ClientScriptSubstituingEventArgs (protected)
		/// <summary>
		/// Obslouží událost ClientScriptSubstituing.
		/// </summary>
		protected virtual void OnClientScriptSubstituing(ClientScriptSubstituingEventArgs eventArgs)
		{
			if (_clientScriptSubstituing != null)
			{
				_clientScriptSubstituing.Invoke(this, eventArgs);
			}
		}
		#endregion

		#region ClientScriptSubstituing
		/// <summary>
		/// Událost pro provedení substituce v klietském skriptu.
		/// </summary>
		public event ClientScriptSubstituingEventHandler ClientScriptSubstituing
		{
			add
			{
				_clientScriptSubstituing += value;
			}
			remove
			{
				_clientScriptSubstituing -= value;
			}
		}
		private ClientScriptSubstituingEventHandler _clientScriptSubstituing;
		
		#endregion

	}
}
