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
	/// parametrù Scriptletu nebo pøi naètìní stránky (pokud je povoleno).
	/// </summary>
    //[DefaultProperty("Script")]
    [ParseChildren(true, "Script")]
	public class ClientScript : ScriptletNestedControl
    {
	
		/// <summary>
		/// Klientský skript. Okolo skriptu je vytvoøena obálka a pøípadnì je spuštìn pøi naètení stránky.
		/// </summary>		
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string Script
        {
            get { return (string)ViewState["Script"] ?? String.Empty; }
			set { ViewState["Script"] = value; }
        }

		/// <summary>
		/// Udává, zda má po naètení stránky dojít k automatickému spuštìní skriptu. Výchozí hodnota je <b>false</b>.
		/// </summary>
		public bool StartOnLoad
		{
			get { return (bool)(ViewState["StartOnLoad"] ?? false); }
			set { ViewState["StartOnLoad"] = value; }
		}
	
		private string clientSideFunctionName;

		/// <summary>
		/// Název funkce, pod jakou je klientský skript zaregistrován.
		/// Hodnota je dostupná až po zavolání metody RegisterClientScript.
		/// </summary>
		public string ClientSideFunctionName
		{
			get { return clientSideFunctionName; }
		}

		/// <summary>
		/// Metoda vytvoøí funkci okolo klienského skriptu a zaregistruje ji do stránky.
		/// Je zajištìno, aby nebyla funkce se shodným obsahem registrována víckrát
		/// (napø. pøi použití controlu v repeateru).
		/// </summary>
		/// <param name="scriptBuilder">ScriptBuilder, do kterého je tvoøen klientský skript.</param>
		public virtual void CreateClientSideScript(ScriptBuilder scriptBuilder)
        {
			// naèteme klientský script
			string clientScript = DoSubstitutions();
			// podíváme se, zda již byl registrován
			string functionName = GetFunctionNameFromCache(clientScript);

			if (functionName == null)
			{
				// pokud tento skript ještì nebyl registrován...				
				CreateFunctionName(null);
				// uložíme jej do cache
				AddFunctionToCache(clientSideFunctionName, clientScript);
				// a zaregistrujeme
				scriptBuilder.Append(WrapClientSideScriptToFunction(clientScript));
			}
			else
				// pokud tento skript již byl registrován,
				// použijeme tento registrovaný skript
				clientSideFunctionName = functionName;

            // zaregistrujeme spusteni pri startu (je-li nastaveno);
            CreateClientSideStartOnLoadEvent(scriptBuilder);
        }

        /// <summary>
        /// Vytvoøí název klientské funkce. Pokud je parametr reuseFunction prázdný,
        /// vytvoøí nový název, jinak použije hodnotu tohoto parametru.
        /// </summary>
        /// <param name="reuseFunctionName"></param>
        protected virtual void CreateFunctionName(string reuseFunctionName)
        {
            if (String.IsNullOrEmpty(reuseFunctionName))
                clientSideFunctionName = "scriptlet" + this.Parent.ClientID;
            else
                clientSideFunctionName = reuseFunctionName;
        }

		/// <summary>
		/// Registruje spuštìní klientského skriptu pøi naštìní stránky.
		/// </summary>
		/// <param name="scriptBuilder">ScriptBuilder, do kterého je tvoøen klientský skript.</param>
		protected virtual void CreateClientSideStartOnLoadEvent(ScriptBuilder scriptBuilder)
		{
            if (StartOnLoad)
            {
                scriptBuilder.AppendFormat(BrowserHelper.GetAttachEvent("window", "onload", Scriptlet.ClientSideFunctionCall));
                scriptBuilder.Append("\n");
            }
		}

		/// <summary>
		/// Vrátí pøipravený klientský skript. Provede nad skriptem substituce.
		/// </summary>
		/// <returns>Klientský skript pøipravený k vytvoøení obálky a registraci.</returns>
		protected virtual string DoSubstitutions()
		{
			return Scriptlet.ScriptSubstitution.Substitute(Script);
		}

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

		/// <summary>
		/// Pøidá klientský skript do cache.
		/// </summary>
		/// <param name="functionName">Název funkce, ve které je skript registrován.</param>
		/// <param name="clientScript">Klientský skript.</param>
		protected virtual void AddFunctionToCache(string functionName, string clientScript)
		{
			FunctionCache.Add(clientScript, functionName);
		}

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
				return result;
			else
				return null;
		}

		/// <summary>
		/// Zabalí klientský skript do funkce.
		/// </summary>
		/// <param name="clientScript">Název funkce, která se tvoøí.</param>
		/// <returns>Klientský skript jako funkce pøipravená k registraci do stránky.</returns>
		protected string WrapClientSideScriptToFunction(string clientScript)
		{
			return String.Format("function {0}(parameters) {{\n{1}\n}}\n", clientSideFunctionName, clientScript);
		}
    }
}
