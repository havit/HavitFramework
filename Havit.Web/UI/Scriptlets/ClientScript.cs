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
		
		#region ClientSideScriptFunctionName
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

		#region GetClientSideScriptFunction
		/// <summary>
		/// Metoda vytvoøí funkci okolo klienského skriptu a zaregistruje ji do stránky.
		/// Je zajištìno, aby nebyla funkce se shodným obsahem registrována víckrát
		/// (napø. pøi použití controlu v repeateru).
		/// </summary>
		/// <param name="scriptBuilder">ScriptBuilder, do kterého je tvoøen klientský skript.</param>
		public virtual void GetClientSideScriptFunction(ScriptBuilder scriptBuilder)
		{
			// naèteme klientský script
			string clientScript = GetSubstitutedScript();
			// podíváme se, zda již byl registrován
			string functionName = GetFunctionNameFromCache(clientScript);

			if (functionName == null)
			{
				// pokud tento skript ještì nebyl registrován...				
				CreateFunctionName(null);
				// uložíme jej do cache
				AddFunctionToCache(clientSideScriptFunctionName, clientScript);
				// a zaregistrujeme
				scriptBuilder.Append(WrapClientSideScriptToFunction(clientScript));
			}
			else
			{
				// pokud tento skript již byl registrován,
				// použijeme tento registrovaný skript
				clientSideScriptFunctionName = functionName;
			}
		}
		#endregion
		
		#region CreateFunctionName
		/// <summary>
		/// Vytvoøí název klientské funkce. Pokud je parametr reuseFunction prázdný,
		/// vytvoøí nový název, jinak použije hodnotu tohoto parametru.
		/// </summary>
		/// <param name="reuseFunctionName"></param>
		protected virtual void CreateFunctionName(string reuseFunctionName)
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

		#region GetClientSideStartupScript
		/// <summary>
		/// Registruje spuštìní klientského skriptu pøi naštìní stránky.
		/// </summary>
		/// <param name="scriptBuilder">ScriptBuilder, do kterého je tvoøen klientský skript.</param>
		public virtual void GetClientSideStartupScript(ScriptBuilder scriptBuilder)
		{
			bool startOnLoad;

			ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
			if ((scriptManager != null) && scriptManager.IsInAsyncPostBack)
			{
				startOnLoad = this.StartOnAjaxCallback;
			}
			else
			{
				startOnLoad = this.StartOnLoad;
			}

			if (startOnLoad)
			{
				scriptBuilder.AppendLine(Scriptlet.ClientSideScriptFunctionCall);
			}
		}
		
		#endregion
		
		#region GetSubstitutedScript
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

		#region ClientScriptSubstituingEventArgs
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

		#region Functions cache
		
		#region FunctionCache
		/// <summary>
		/// Cache pro klientské skripty. Klíèem je skript, hodnotou je název funkce,
		/// ve které je skript registrován.
		/// Cache je uložena v HttpContextu.
		/// </summary>
		protected virtual Dictionary<string, string> FunctionCache
		{
			get
			{
				Dictionary<string, string> result = (Dictionary<string, string>)HttpContext.Current.Items[typeof(ClientScript)];

				if (result == null)
				{
					// pokud cache ještì není, vytvoøíme ji a vrátíme
					// žádné zámky (lock { ... }) nejsou potøeba, jsme stále v jednom HttpContextu
					result = new Dictionary<string, string>();
					HttpContext.Current.Items[typeof(ClientScript)] = result;
				}

				return result;
			}
		}		
		#endregion

		#region AddFunctionToCache
		/// <summary>
		/// Pøidá klientský skript do cache.
		/// </summary>
		/// <param name="functionName">Název funkce, ve které je skript registrován.</param>
		/// <param name="clientScript">Klientský skript.</param>
		protected virtual void AddFunctionToCache(string functionName, string clientScript)
		{
			FunctionCache.Add(clientScript, functionName);
		}		
		#endregion

		#region GetFunctionNameFromCache
		/// <summary>
		/// Nalezne název funkce, ve které je klientský skript registrován.
		/// </summary>
		/// <param name="clientScript">Klientský skript.</param>
		/// <returns>Nalezne název funkce, ve které je klientský skript 
		/// registrován. Pokud skript není registrován, vrátí null.</returns>
		protected virtual string GetFunctionNameFromCache(string clientScript)
		{
			string result;
			if (FunctionCache.TryGetValue(clientScript, out result))
			{
				return result;
			}
			else
			{
				return null;
			}
		}
		#endregion
		
		#region WrapClientSideScriptToFunction
		/// <summary>
		/// Zabalí klientský skript do funkce.
		/// </summary>
		/// <param name="clientScript">Název funkce, která se tvoøí.</param>
		/// <returns>Klientský skript jako funkce pøipravená k registraci do stránky.</returns>
		protected string WrapClientSideScriptToFunction(string clientScript)
		{
			return String.Format("function {0}(parameters){2}{{{2}{1}{2}}}{2}", clientSideScriptFunctionName, clientScript, Environment.NewLine);
		}
		#endregion
		
		#endregion
	}
}
