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
	/// Scriptlet umoûÚuje snadnou tvorbu klientsk˝ch skript˘.
	/// </summary>
	[ControlBuilder(typeof(NoLiteralContolBuilder))]	
	public class Scriptlet : Control, IScriptControl
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
		/// VracÌ nebo nastavuje repository extender˘ pro parametry.
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
		/// VracÌ nebo nastavuje substituci pouûitou pro tvorbu klienskÈho skriptu.
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
		/// Vytvo¯Ì instanci scriptletu a nastavÌ v˝chozÌ hodnoty
		/// <see cref="ControlExtenderRepository">ControlExtenderRepository</see>
		/// (na <see cref="Havit.Web.UI.Scriptlets.ControlExtenderRepository.Default">ControlExtenderRepository.Default</see>)
		/// a <see cref="ScriptSubstitution">ScriptSubstitution</see>
		/// (na <see cref="ScriptSubstitutionRepository.Default">ScriptSubstitutionRepository.Default</see>).
		/// </summary>
		public Scriptlet()
		{
			// vezmeme si v˝chozÌ repository
			controlExtenderRepository = Havit.Web.UI.Scriptlets.ControlExtenderRepository.Default;
			scriptSubstitution = ScriptSubstitutionRepository.Default;
		}

		#endregion

		#region AddedControl (override)
		/// <summary>
		/// Zavol·no, kdyû je do kolekce Controls p¯id·n Control.
		/// Zajiöùuje, aby nebyl p¯id·n control neimplementujÌcÌ 
		/// <see cref="IScriptletParameter">IScriptletParameter</see>
		/// nebo <see cref="ClientScript">ClientScript</see>.
		/// Z·roveÚ zajistÌ, aby nebyl p¯id·n vÌce neû jeden <see cref="ClientScript">ClientScript</see>.
		/// </summary>
		/// <param name="control">P¯id·van˝ control.</param>
		/// <param name="index">Pozice v kolekci control˘, kam je control p¯id·v·n.</param>
		protected override void AddedControl(Control control, int index)
		{
			base.AddedControl(control, index);

			// zajistÌme, aby n·m do scriptletu nep¯iöel nezn·m˝ control
			if (!(control is ScriptletNestedControl))
			{
				throw new ArgumentException(String.Format("Do Scriptletu je vkl·d·n nepodporovan˝ control {0}.", control.ID));
			}

			if (control is ClientScript)
			{
				if (clientScript != null)
				{
					throw new ArgumentException("Scriptlet musÌ obsahovat ClientScript pr·vÏ jednou.");
				}

				clientScript = (ClientScript)control;
			}

			if (control is IScriptletParameter)
			{
				scriptletParameters.Add((IScriptletParameter)control);
			}
		}
		#endregion

		/* Renderov·nÌ *************** */

		#region OnPreRender (override)
		/// <summary>
		/// ZajistÌ tvorbu klienstkÈho skriptu.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			
			// zajistÌme, aby byly k dispozici scripty AJAXu, pokud m·me scriptmanager
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
		/// OvÏ¯Ì, zda jsou spr·vnÏ zad·ny parametry scriptletu (testuje, zda byl zad·n ClientScript).
		/// </summary>
		protected virtual void CheckControlConditions()
		{
			if (clientScript == null)
			{
				throw new HttpException("ClientScript nebyl zad·n.");
			}
		}
		#endregion
		
		#region PrepareAndRegisterClientScript (private)
		/// <summary>
		/// SestavÌ kompletnÌ klientsk˝ skript seskl·d·nÌm funkce, vytvo¯enÌ objektu 
		/// a jeho parametr˘. Zaregistruje skripty do str·nky
		/// </summary>
		/// <returns>KompletnÌ klientsk˝ skript.</returns>
		private void PrepareAndRegisterClientScript()
		{
			if (DesignMode)
			{
				return;
			}

			ScriptBuilder builder = new ScriptBuilder();

			PrepareClientSideScripts(builder);

			// zaregistrujeme jej na konec str·nky, aby byly controly jiû dostupnÈ
			ScriptManager.RegisterStartupScript(
				this.Page,
				typeof(Scriptlet),
				this.UniqueID,
				builder.ToString(),
				true
			);
		}
		#endregion
		
		#region PrepareClientSideScripts (private)
		/// <summary>
		/// Vr·tÌ klientsk˝ skript scriptletu.
		/// </summary>
		/// <param name="builder">Script builder.</param>
		private void PrepareClientSideScripts(ScriptBuilder builder)
		{
			string code;

			code = clientScript.GetClientSideFunctionCode();
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
				// pageLoaded n·m zajistÌ nav·z·nÌ ud·lostÌ po v˝mÏnÏ element˘
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
		/// Vr·tÌ kÛd funkce pro zÌsk·nÌ parametr˘ scriptletu.
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
		/// Vr·tÌ kÛd funkce pro nav·z·nÌ ud·lostÌ na parametry scriptletu.
		/// </summary>
		private string PrepareClientSideScripts_GetAttachEventsFunctionCode()
		{
			ScriptBuilder attachBuilder = new ScriptBuilder();

			// pokud m·me script manager, odpojÌme st·vajÌcÌ nav·z·nÌ ud·lostÌ (kv˘li callback˘m)			
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
		/// Vr·tÌ kÛd funkce pro odpojenÌ ud·lostÌ od parametr˘ scriptletu.
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
		/// Zaregistruje funkci. P¯ed registracÌ zkouöÌ, zda je funkce v cache Ëi zda je moûnÈ provÈst reuse.
		/// </summary>
		/// <param name="builder">Builder, do kterÈho je skript zaps·n.</param>
		/// <param name="functionName">N·zev funkce, kter˝ bude pouûit, pokud nenÌ metoda v cache.</param>
		/// <param name="functionParameters">N·zvy parametr˘ funkce.</param>
		/// <param name="functionCode">KÛd funkce.</param>
		/// <param name="hashIdentifier">Identifik·tor hashe pro reuse.</param>
		/// <param name="reused">VracÌ informaci, zda doölo k reuse skriptu.</param>
		private void PrepareClientSideScripts_WriteFunctionWithReuse(ScriptBuilder builder, ref string functionName, string[] functionParameters, string functionCode, string hashIdentifier, out bool reused)
		{
			if (String.IsNullOrEmpty(functionCode))
			{
				reused = false;
				return;
			}

			// vezmeme jmÈno funkce z cache
			string cacheFunctionName = ScriptCacheHelper.GetFunctionNameFromCache(functionParameters, functionCode);
			bool foundInCache = false;
			if (String.IsNullOrEmpty(cacheFunctionName))
			{
				// pokud jsme jej nenaöli, pouûijeme zadanÈ jmÈno
				cacheFunctionName = functionName;
				ScriptCacheHelper.AddFunctionToCache(cacheFunctionName, functionParameters, functionCode);
			}
			else
			{
				// pokud jsme jej naöli, ¯ekneme, jakÈ jmÈno jsme pouûili
				functionName = cacheFunctionName;
				foundInCache = true;
			}

			// WrapClientSideScriptToFunction
			string functionBlock = String.Format("function {0}({1}){3}{{{3}{2}{3}}}{3}", // struËnÏ: function X(paramemetry) { kod } + konce ¯·dek..
				functionName,
				String.Join(", ", functionParameters),
				functionCode.Trim(),
				Environment.NewLine);

			reused = false;
			int hash = functionBlock.GetHashCode(); // p¯edpokl·d·me, ûe pokud se liöÌ skripty, liöÌ se i GetHashCode. Shoda moûn·, nepravdÏpodobn·. Kdyûtak MD5 ci SHA1.
			if (IsInAsyncPostBack && !String.IsNullOrEmpty(hashIdentifier))
			{
				// pokud jsme v callbacku, m˘ûeme zkusit reuse skriptu
				// tj. nerenderovat jej, protoûe na klientu uû je
				reused = (int)ViewState[hashIdentifier] == hash;
			}

			if (!foundInCache && !reused)
			{
				builder.Append(functionBlock);
			}

			if (!reused && AsyncPostBackEnabled)
			{
				ViewState[hashIdentifier] = hash;
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
		/// VracÌ true, pokud je zpracov·v·n asynchronnÌ postback (callback).
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
		/// VracÌ true, pokud je k dispozici ScriptManager.
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
		/// VracÌ true, pokud m˘ûe dojÌt k asynchronnÌmu postbacku (callbacku).
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
