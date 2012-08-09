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
	/// Pøedstavuje klientský skript - funkci, která je vyvolána pøi zmìnì vstupních
	/// parametrù <see cref="Scriptlet">Scriptletu</see> nebo pøi naètìní stránky èi callbacku (pokud je povoleno).
	/// </summary>
	//[DefaultProperty("Script")]
	[ParseChildren(true, "Script")]
	public class ClientScript : ScriptletNestedControl
	{

		/* PARAMETY CLIENTSCRIPTU *************** */

		#region Script
		/// <summary>
		/// Klientský skript. Okolo skriptu je vytvoøena obálka a pøípadnì je spuštìn po naètení stránky èi callbacku.
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
		/// Udává, zda má po naètení stránky dojít k automatickému spuštìní skriptu. Výchozí hodnota je <c>false</c>.
		/// </summary>				
		/// <remarks>
		/// Nemá vliv na zpracování stránky pøi asynchronním postbacku (callbacku). K tomu slouží vlastnost <see cref="StartOnAjaxCallback"/>StartOnAjaxCallback</see>.
		/// </remarks>
		public bool StartOnLoad
		{
			get { return (bool)(ViewState["StartOnLoad"] ?? false); }
			set { ViewState["StartOnLoad"] = value; }
		}
		#endregion

		#region StartOnAjaxCallback
		/// <summary>
		/// Udává, zda má po dojít k automatickému spuštìní skriptu po asynchronním postbacku (ajax callback). Výchozí hodnota je <b>false</b>.
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
		/// Vrací true, pokud má být renderován skript pro automatické spuštìní funkce scriptletu.
		/// </summary>
		/// <returns></returns>
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

		/* RENDEROVÁNÍ *************** */
/*
		#region ClientSideScriptFunctionName (internal)
		/// <summary>
		/// Název funkce, pod jakou je klientský skript zaregistrován.
		/// Hodnota je dostupná až po zavolání metody RegisterClientScript.
		/// </summary>
		internal string ClientSideScriptFunctionName
		{
			get
			{
				if (clientSideScriptFunctionName == null)
				{
					throw new InvalidOperationException("Èíst vlastnost ClientSideFunctionName ještì není dovoleno.");
				}

				return clientSideScriptFunctionName;
			}
		}
		private string clientSideScriptFunctionName;
		#endregion
*/
		#region GetClientSideScriptFunction
		/// <summary>
		/// Metoda vytvoøí funkci okolo klienského skriptu a zaregistruje ji do stránky.
		/// Je zajištìno, aby nebyla funkce se shodným obsahem registrována víckrát
		/// (napø. pøi použití controlu v repeateru).
		/// </summary>
		/// <param name="scriptBuilder">ScriptBuilder, do kterého je tvoøen klientský skript.</param>
		public virtual string GetClientSideFunctionCode()
		{
			// naèteme klientský script
			return GetSubstitutedScript();
//            // podíváme se, zda již byl registrován
//            string functionName = ScriptCacheHelper.GetFunctionNameFromCache(_clientScriptParameters, code);

//            if (functionName == null)
//            {
//                // pokud tento skript ještì nebyl registrován...				
//                CreateFunctionName(null);
//                // uložíme jej do cache
//                ScriptCacheHelper.AddFunctionToCache(clientSideScriptFunctionName, _clientScriptParameters, code);
//                // a zaregistrujeme
//                return WrapClientSideScriptToFunction(clientSideScriptFunctionName, _clientScriptParameters, code);
//            }
//            else
//            {
//                // pokud tento skript již byl registrován,
//                // použijeme tento registrovaný skript
//                CreateFunctionName(functionName);
////				clientSideScriptFunctionName = functionName;
//                return null;
//            }
		}
		#endregion
	/*
		#region CreateFunctionName (private)
		/// <summary>
		/// Vytvoøí název klientské funkce. Pokud je parametr reuseFunction prázdný,
		/// vytvoøí nový název, jinak použije hodnotu tohoto parametru.
		/// </summary>
		/// <param name="reuseFunctionName"></param>
		private void CreateFunctionName(string reuseFunctionName)
		{
			if (!String.IsNullOrEmpty(reuseFunctionName))
			{
				clientSideScriptFunctionName = reuseFunctionName;
			}
			else
			{
				clientSideScriptFunctionName = "scriptletFunction_" + this.Scriptlet.ClientID;
			}
		}		
		#endregion
		*/
		private static string[] _clientScriptParameters = new string[] { "parameters" };
		
		/* SUBSTITUCE *************** */

		#region GetSubstitutedScript (protected)
		/// <summary>
		/// Vrátí pøipravený klientský skript. Provede nad skriptem substituce.
		/// </summary>
		/// <returns>Klientský skript pøipravený k vytvoøení obálky a registraci.</returns>
		protected virtual string GetSubstitutedScript()
		{
			ClientScriptSubstituingEventArgs eventArgs = new ClientScriptSubstituingEventArgs(Script);
			OnClientScriptSubstituing(eventArgs);
			return Scriptlet.ScriptSubstitution.Substitute(eventArgs.ClientScript);
		}
		#endregion

		#region ClientScriptSubstituingEventArgs (protected)
		protected virtual void OnClientScriptSubstituing(ClientScriptSubstituingEventArgs eventArgs)
		{
			if (_clientScriptSubstituing != null)
			{
				_clientScriptSubstituing.Invoke(this, eventArgs);
			}
		}
		#endregion

		#region ClientScriptSubstituing
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

		/* STATIC  *************** */

		#region WrapClientSideScriptToFunction (static)
		/// <summary>
		/// Zabalí klientský skript do funkce.
		/// </summary>
		/// <returns>Klientský skript jako funkce pøipravená k registraci do stránky.</returns>
		public static string WrapClientSideScriptToFunction(string functionName, string[] parameters, string clientScript)
		{
			return String.Format("function {0}({1}){3}{{{3}{2}{3}}}{3}",
				functionName,
				parameters == null ? String.Empty : String.Join(", ", parameters),
				clientScript.Trim(),
				Environment.NewLine);
		}
		#endregion

	}
}
