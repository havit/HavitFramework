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
	/// Scriptlet umožňuje snadnou tvorbu klientských skriptů.
	/// </summary>
	[ControlBuilder(typeof(NoLiteralContolBuilder))]	
	public sealed class Scriptlet : Control, IScriptControl
	{
		private readonly string[] _clientScriptScriptletFunctionParameters = new string[] { "parameters" };
		private readonly string[] _clientScriptGetParametersFunctionParameters = new string[] { };
		private readonly string[] _clientScriptAttachDetachEventsFunctionParameters = new string[] { "data", "delegate", "handler" };

		#region Private fields
		private ClientScript clientScript = null;
		private List<IScriptletParameter> scriptletParameters = new List<IScriptletParameter>();
		#endregion

		/* Parametry Scriptletu *************** */

		#region ControlExtenderRepository
		/// <summary>
		/// Vrací nebo nastavuje repository extenderů pro parametry.
		/// </summary>
		public IControlExtenderRepository ControlExtenderRepository
		{
			get { return controlExtenderRepository; }
			set { controlExtenderRepository = value; }
		}
		private IControlExtenderRepository controlExtenderRepository;
		
		#endregion

		#region ScriptSubstitution
		/// <summary>
		/// Vrací nebo nastavuje substituci použitou pro tvorbu klienského skriptu.
		/// </summary>
		public IScriptSubstitution ScriptSubstitution
		{
			get { return scriptSubstitution; }
			set { scriptSubstitution = value; }
		}
		private IScriptSubstitution scriptSubstitution;
		#endregion

		/* *************** */

		#region Constructor
		/// <summary>
		/// Vytvoří instanci scriptletu a nastaví výchozí hodnoty
		/// <see cref="ControlExtenderRepository">ControlExtenderRepository</see>
		/// (na <see cref="Havit.Web.UI.Scriptlets.ControlExtenderRepository.Default">ControlExtenderRepository.Default</see>)
		/// a <see cref="ScriptSubstitution">ScriptSubstitution</see>
		/// (na <see cref="ScriptSubstitutionRepository.Default">ScriptSubstitutionRepository.Default</see>).
		/// </summary>
		public Scriptlet()
		{
			// vezmeme si výchozí repository
			controlExtenderRepository = Havit.Web.UI.Scriptlets.ControlExtenderRepository.Default;
			scriptSubstitution = ScriptSubstitutionRepository.Default;
		}

		#endregion

		#region AddedControl (override)
		/// <summary>
		/// Zavoláno, když je do kolekce Controls přidán Control.
		/// Zajišťuje, aby nebyl přidán control neimplementující 
		/// <see cref="IScriptletParameter">IScriptletParameter</see>
		/// nebo <see cref="ClientScript">ClientScript</see>.
		/// Zároveň zajistí, aby nebyl přidán více než jeden <see cref="ClientScript">ClientScript</see>.
		/// </summary>
		/// <param name="control">Přidávaný control.</param>
		/// <param name="index">Pozice v kolekci controlů, kam je control přidáván.</param>
		protected override void AddedControl(Control control, int index)
		{
			base.AddedControl(control, index);

			// zajistíme, aby nám do scriptletu nepřišel neznámý control
			if (!(control is ScriptletNestedControl))
			{
				throw new ArgumentException(String.Format("Do Scriptletu je vkládán nepodporovaný control {0}.", control.ID));
			}

			if (control is ClientScript)
			{
				if (clientScript != null)
				{
					throw new ArgumentException("Scriptlet musí obsahovat ClientScript právě jednou.");
				}

				clientScript = (ClientScript)control;
			}

			if (control is IScriptletParameter)
			{
				scriptletParameters.Add((IScriptletParameter)control);
			}
		}
		#endregion

		/* Renderování *************** */

		#region OnPreRender (override)
		/// <summary>
		/// Zajistí tvorbu klienstkého skriptu.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			
			// zajistíme, aby byly k dispozici scripty AJAXu, pokud máme scriptmanager
			ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
			if (scriptManager != null)
			{
				scriptManager.RegisterScriptControl(this);
			}

			CheckControlConditions();
			PrepareAndRegisterClientScript();
		}
		#endregion
		
		#region CheckControlConditions (protected)
		/// <summary>
		/// Ověří, zda jsou správně zadány parametry scriptletu (testuje, zda byl zadán ClientScript).
		/// </summary>
		protected /*virtual*/ void CheckControlConditions()
		{
			if (clientScript == null)
			{
				throw new HttpException("ClientScript nebyl zadán.");
			}
		}
		#endregion
		
		#region PrepareAndRegisterClientScript (private)
		/// <summary>
		/// Sestaví kompletní klientský skript seskládáním funkce, vytvoření objektu 
		/// a jeho parametrů. Zaregistruje skripty do stránky
		/// </summary>
		private void PrepareAndRegisterClientScript()
		{
			if (DesignMode)
			{
				return;
			}

			ScriptBuilder builder = new ScriptBuilder();

			PrepareClientSideScripts(builder);

			if (!builder.IsEmpty)
			{
				// zaregistrujeme jej na konec stránky, aby byly controly již dostupné
				ScriptManager.RegisterStartupScript(
					this.Page,
					typeof(Scriptlet),
					this.UniqueID,
					builder.ToString(),
					true);
			}
		}
		#endregion
		
		#region PrepareClientSideScripts (private)
		/// <summary>
		/// Vrátí klientský skript scriptletu.
		/// </summary>
		/// <param name="builder">Script builder.</param>
		private void PrepareClientSideScripts(ScriptBuilder builder)
		{
			string code;

			code = clientScript.GetClientSideFunctionCode();

			if (String.IsNullOrEmpty(code))
			{
				// pokud je výkonný skript prázdný, nic neregistrujeme - ani attach metody, nemají význam.
				return;
			}

			bool clientSideScriptFunctionReused;
			string clientSideScriptFunctionName = "scriptlet_" + this.ClientID + "_Function";
			PrepareClientSideScripts_WriteFunctionWithReuse(builder, ref clientSideScriptFunctionName, _clientScriptScriptletFunctionParameters, code, "ScriptletFunctionHash", out clientSideScriptFunctionReused);

			code = PrepareClientSideScripts_GetParametersFunctionCode();
			bool clientSideGetParametersFunctionReused;
			string clientSideGetParametersFunctionName = "scriptlet_" + this.ClientID + "_GetParameters";
			PrepareClientSideScripts_WriteFunctionWithReuse(builder, ref clientSideGetParametersFunctionName, _clientScriptGetParametersFunctionParameters, code, "GetParametersHash", out clientSideGetParametersFunctionReused);

			code = PrepareClientSideScripts_GetAttachEventsFunctionCode();
			bool clientSideAttachEventsFunctionReused;
			string clientSideAttachEventsFunctionName = "scriptlet_" + this.ClientID + "_AttachEvents";
			PrepareClientSideScripts_WriteFunctionWithReuse(builder, ref clientSideAttachEventsFunctionName, _clientScriptAttachDetachEventsFunctionParameters, code, "AttachEventsHash", out clientSideAttachEventsFunctionReused);

			string handlerDelegate = String.Format("scriptlet_{0}_HD", this.ClientID);
			string attachFunctionDelegate = String.Format("scriptlet_{0}_AE", this.ClientID);
			string detachFunctionDelegate = String.Format("scriptlet_{0}_DE", this.ClientID);

			if (!AsyncPostBackEnabled)
			{
				builder.AppendLineFormat("var {0} = new Function(\"{1}({2}());\");", handlerDelegate, clientSideScriptFunctionName, clientSideGetParametersFunctionName);
				builder.AppendLineFormat("var {0} = new Function(\"{1}({2}(), {0}, {3});\");", attachFunctionDelegate, clientSideAttachEventsFunctionName, clientSideGetParametersFunctionName, handlerDelegate);
				builder.AppendLine(BrowserHelper.GetAttachEventScript("window", "onload", attachFunctionDelegate));
			}
			else
			{
				code = PrepareClientSideScripts_GetDetachEventsFunctionCode();
				bool clientSideDetachEventsFunctionReused;
				string clientSideDetachEventsFunctionName = "scriptlet_" + this.ClientID + "_DetachEvents";
				PrepareClientSideScripts_WriteFunctionWithReuse(builder, ref clientSideDetachEventsFunctionName, _clientScriptAttachDetachEventsFunctionParameters, code, "DetachEventsHash", out clientSideDetachEventsFunctionReused);

				if (!(IsInAsyncPostBack && clientSideScriptFunctionReused && clientSideGetParametersFunctionReused && clientSideAttachEventsFunctionReused && clientSideDetachEventsFunctionReused))
				{
					builder.AppendLineFormat("var {0} = new Function(\"{1}({2}());\");", handlerDelegate, clientSideScriptFunctionName, clientSideGetParametersFunctionName);
					builder.AppendLineFormat("var {0} = new Function(\"{1}({2}(), {0}, {3});\");", attachFunctionDelegate, clientSideAttachEventsFunctionName, clientSideGetParametersFunctionName, handlerDelegate);
					builder.AppendLineFormat("var {0} = new Function(\"{1}({2}(), {0}, {3});\");", detachFunctionDelegate, clientSideDetachEventsFunctionName, clientSideGetParametersFunctionName, handlerDelegate);
				}

				builder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().add_pageLoading({0});", detachFunctionDelegate);
				// pageLoaded nám zajistí navázání událostí po výměně elementů
				builder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded({0});", attachFunctionDelegate);
			}

			if (clientScript.GetAutoStart())
			{
				builder.AppendLineFormat("{0}();", handlerDelegate);
			}
		}
		#endregion

		#region PrepareClientSideScripts_GetParametersFunctionCode (private)
		/// <summary>
		/// Vrátí kód funkce pro získání parametrů scriptletu.
		/// </summary>
		private string PrepareClientSideScripts_GetParametersFunctionCode()
		{
			ScriptBuilder builder = new ScriptBuilder();
			builder.AppendLine("var result = new Object();");
			foreach (IScriptletParameter scriptletParameter in scriptletParameters)
			{
				scriptletParameter.CheckProperties();
				scriptletParameter.GetInitializeClientSideValueScript("result", this.NamingContainer, builder);
			}
			builder.AppendLine("return result;");
			return builder.ToString();
		}
		#endregion

		#region PrepareClientSideScripts_GetAttachEventsFunctionCode (private)
		/// <summary>
		/// Vrátí kód funkce pro navázání událostí na parametry scriptletu.
		/// </summary>
		private string PrepareClientSideScripts_GetAttachEventsFunctionCode()
		{
			ScriptBuilder attachBuilder = new ScriptBuilder();

			// pokud máme script manager, odpojíme stávající navázání událostí (kvůli callbackům)			
			if (AsyncPostBackEnabled)
			{
				attachBuilder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().remove_pageLoaded(delegate);");
			}

			foreach (IScriptletParameter scriptletParameter in scriptletParameters)
			{
				scriptletParameter.GetAttachEventsScript("data", this.NamingContainer, "handler", attachBuilder);
			}

			return attachBuilder.ToString();
		}
		#endregion

		#region PrepareClientSideScripts_GetDetachEventsFunctionCode (private)
		/// <summary>
		/// Vrátí kód funkce pro odpojení událostí od parametrů scriptletu.
		/// </summary>
		private string PrepareClientSideScripts_GetDetachEventsFunctionCode()
		{
			ScriptBuilder detachBuilder = new ScriptBuilder();
			if (AsyncPostBackEnabled)
			{
				detachBuilder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().remove_pageLoading(delegate);");
			}
			foreach (IScriptletParameter scriptletParameter in scriptletParameters)
			{
				scriptletParameter.GetDetachEventsScript("data", this.NamingContainer, "handler", detachBuilder);
			}

			return detachBuilder.ToString();
		}
		#endregion		

		#region PrepareClientSideScripts_WriteScriptWithReuse (private)
		/// <summary>
		/// Zaregistruje funkci. Před registrací zkouší, zda je funkce v cache či zda je možné provést reuse.
		/// </summary>
		/// <param name="builder">Builder, do kterého je skript zapsán.</param>
		/// <param name="functionName">Název funkce, který bude použit, pokud není metoda v cache.</param>
		/// <param name="functionParameters">Názvy parametrů funkce.</param>
		/// <param name="functionCode">Kód funkce.</param>
		/// <param name="hashIdentifier">Identifikátor hashe pro reuse.</param>
		/// <param name="reused">Vrací informaci, zda došlo k reuse skriptu.</param>
		private void PrepareClientSideScripts_WriteFunctionWithReuse(ScriptBuilder builder, ref string functionName, string[] functionParameters, string functionCode, string hashIdentifier, out bool reused)
		{
			if (String.IsNullOrEmpty(functionCode))
			{
				reused = false;
				return;
			}

			// vezmeme jméno funkce z cache
			string cacheFunctionName = ScriptCacheHelper.GetFunctionNameFromCache(functionParameters, functionCode);
			bool foundInCache = false;
			if (String.IsNullOrEmpty(cacheFunctionName))
			{
				// pokud jsme jej nenašli, použijeme zadané jméno
				cacheFunctionName = functionName;
				ScriptCacheHelper.AddFunctionToCache(cacheFunctionName, functionParameters, functionCode);
			}
			else
			{
				// pokud jsme jej našli, řekneme, jaké jméno jsme použili
				functionName = cacheFunctionName;
				foundInCache = true;
			}

			// WrapClientSideScriptToFunction
			string functionBlock = String.Format("function {0}({1}){3}{{{3}{2}{3}}}{3}", // stručně: function X(paramemetry) { kod } + konce řádek..
				functionName,
				String.Join(", ", functionParameters),
				functionCode.Trim(),
				Environment.NewLine);

			reused = false;
			int currentHashValue = functionBlock.GetHashCode(); // předpokládáme, že pokud se liší skripty, liší se i GetHashCode. Shoda možná, nepravděpodobná. Kdyžtak MD5 ci SHA1.
			if (IsInAsyncPostBack && !String.IsNullOrEmpty(hashIdentifier))
			{
				// pokud jsme v callbacku, můžeme zkusit reuse skriptu
				// tj. nerenderovat jej, protože na klientu už je
				object oldHashValue = ViewState[hashIdentifier];
				if (oldHashValue != null)
				{
					reused = (int)oldHashValue == currentHashValue;
				}
			}

			if (!foundInCache && !reused)
			{
				builder.Append(functionBlock);
			}

			if (AsyncPostBackEnabled)
			{
				ViewState[hashIdentifier] = currentHashValue;
			}
		}
		#endregion

		/* IScriptControl interface *************** */
		
		#region IScriptControl Members

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			return null;
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return null;
		}

		#endregion

		/* ScriptManager *************** */

		#region IsInAsyncPostBack (internal)
		/// <summary>
		/// Vrací true, pokud je zpracováván asynchronní postback (callback).
		/// </summary>
		internal bool IsInAsyncPostBack
		{
			get
			{
				if (_isInAsyncPostBack == null)
				{
					ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
					_isInAsyncPostBack = (scriptManager != null) && scriptManager.IsInAsyncPostBack;
				}
				return _isInAsyncPostBack.Value;
			}
		}
		private bool? _isInAsyncPostBack = null;
		#endregion

		#region IsScriptManager (internal)
		/// <summary>
		/// Vrací true, pokud je k dispozici ScriptManager.
		/// </summary>
		internal bool IsScriptManager
		{
			get
			{
				if (_isScriptManager == null)
				{
					_isScriptManager = ScriptManager.GetCurrent(this.Page) != null;
				}
				return _isScriptManager.Value;
			}
		}
		private bool? _isScriptManager = null;
		#endregion

		#region AsyncPostBackEnabled (internal)
		/// <summary>
		/// Vrací true, pokud může dojít k asynchronnímu postbacku (callbacku).
		/// </summary>
		internal bool AsyncPostBackEnabled
		{
			get
			{
				if (_asyncPostBackEnabled == null)
				{
					ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
					_asyncPostBackEnabled = (scriptManager != null) && scriptManager.EnablePartialRendering && scriptManager.SupportsPartialRendering;
				}
				return _asyncPostBackEnabled.Value;
			}
		}
		private bool? _asyncPostBackEnabled = null;
		#endregion

	}
}
