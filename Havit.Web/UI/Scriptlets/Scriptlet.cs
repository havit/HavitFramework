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
		#region Private fields
		private ClientScript clientScript = null;
		private List<IScriptletParameter> scriptletParameters = new List<IScriptletParameter>();
		#endregion
		
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

		#region Constructor
		/// <summary>
		/// Vytvo¯Ì instanci scriptletu a nastavÌ v˝chozÌ hodnoty
		/// <see cref="ControlExtenderRepository">ControlExtenderRepository</see>
		/// (na <see cref="ControlExtenderRepository.Default">ControlExtenderRepository.Default</see>)
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

		#region AddedControl
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

		#region OnPreRender
		/// <summary>
		/// ZajistÌ tvorbu klienstkÈho skriptu.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			
			ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
			if (scriptManager != null)
			{
				scriptManager.RegisterScriptControl(this);
			}
			CheckControlConditions();
			PrepareAndRegisterClientScript();
		}
		#endregion
		
		#region CheckControlConditions
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
		
		#region PrepareAndRegisterClientScript
		/// <summary>
		/// SestavÌ kompletnÌ klientsk˝ skript seskl·d·nÌm funkce, vytvo¯enÌ objektu 
		/// a jeho parametr˘. Zaregistruje skripty do str·nky
		/// </summary>
		/// <returns>KompletnÌ klientsk˝ skript.</returns>
		protected virtual void PrepareAndRegisterClientScript()
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
		
		#region PrepareClientSideScripts
		/// <summary>
		/// Vr·tÌ klientsk˝ skript scriptletu.
		/// </summary>
		/// <param name="builder">Script builder.</param>
		protected virtual void PrepareClientSideScripts(ScriptBuilder mainBuilder)
		{
			string code;

			code = clientScript.GetClientSideFunctionCode();
			bool clientSideScriptFunctionReused;
			string clientSideScriptFunctionName = "scriptlet_" + this.ClientID + "_Function";
			clientSideScriptFunctionName = PrepareClientSideScripts_WriteScriptWithReuse(mainBuilder, clientSideScriptFunctionName, new string[] { "parameters" }, code, "ScriptletFunctionHash", out clientSideScriptFunctionReused);

			code = PrepareClientSideScripts_GetParametersFunctionCode();
			bool clientSideGetParametersFunctionReused;
			string clientSideGetParametersFunctionName = "scriptlet_" + this.ClientID + "_GetParameters";
			clientSideGetParametersFunctionName = PrepareClientSideScripts_WriteScriptWithReuse(mainBuilder, clientSideGetParametersFunctionName, null, code, "GetParametersHash", out clientSideGetParametersFunctionReused);

			code = PrepareClientSideScripts_GetAttachEventsFunctionCode();
			bool clientSideAttachEventsFunctionReused;
			string clientSideAttachEventsFunctionName = "scriptlet_" + this.ClientID + "_AttachEvents";
			clientSideAttachEventsFunctionName = PrepareClientSideScripts_WriteScriptWithReuse(mainBuilder, clientSideAttachEventsFunctionName, new string[] { "data", "delegatex", "handler" }, code, "AttachEventsHash", out clientSideAttachEventsFunctionReused);

			string handlerDelegate = String.Format("scriptlet_{0}_HD", this.ClientID);
			string attachFunctionDelegate = String.Format("scriptlet_{0}_AE", this.ClientID);
			string detachFunctionDelegate = String.Format("scriptlet_{0}_DE", this.ClientID);

			if (!IsScriptManager)
			{
				mainBuilder.AppendLineFormat("var {0} = new Function(\"{1}({2}());\");", handlerDelegate, clientSideScriptFunctionName, clientSideGetParametersFunctionName);
				mainBuilder.AppendLineFormat("var {0} = new Function(\"{1}({2}(), {0}, {3});\");", attachFunctionDelegate, clientSideAttachEventsFunctionName, clientSideGetParametersFunctionName, handlerDelegate);
				mainBuilder.AppendLine(BrowserHelper.GetAttachEventScript("window", "onload", attachFunctionDelegate));
			}
			else
			{
				code = PrepareClientSideScripts_GetDetachEventsFunctionCode();
				bool clientSideDetachEventsFunctionReused;
				string clientSideDetachEventsFunctionName = "scriptlet_" + this.ClientID + "_DetachEvents";
				clientSideDetachEventsFunctionName = PrepareClientSideScripts_WriteScriptWithReuse(mainBuilder, clientSideDetachEventsFunctionName, new string[] { "data", "delegatex", "handler" }, code, "DetachEventsHash", out clientSideDetachEventsFunctionReused);

				if (!(IsInAsyncPostBack && clientSideScriptFunctionReused && clientSideGetParametersFunctionReused && clientSideAttachEventsFunctionReused && clientSideDetachEventsFunctionReused))
				{
					mainBuilder.AppendLineFormat("var {0} = new Function(\"{1}({2}());\");", handlerDelegate, clientSideScriptFunctionName, clientSideGetParametersFunctionName);
					mainBuilder.AppendLineFormat("var {0} = new Function(\"{1}({2}(), {0}, {3});\");", attachFunctionDelegate, clientSideAttachEventsFunctionName, clientSideGetParametersFunctionName, handlerDelegate);
					mainBuilder.AppendLineFormat("var {0} = new Function(\"{1}({2}(), {0}, {3});\");", detachFunctionDelegate, clientSideDetachEventsFunctionName, clientSideGetParametersFunctionName, handlerDelegate);
				}

				mainBuilder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().add_pageLoading({0});", detachFunctionDelegate);
				// pageLoaded n·m zajistÌ nav·z·nÌ ud·lostÌ po v˝mÏnÏ element˘
				mainBuilder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded({0});", attachFunctionDelegate);
			}

			if (clientScript.GetAutoStart())
			{
				mainBuilder.AppendLineFormat("{0}();", handlerDelegate);
			}
		}
		#endregion




		// vytvo¯Ìme funkci pro zÌsk·nÌ objektu nesoucÌ parametry
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

		private string PrepareClientSideScripts_GetAttachEventsFunctionCode()
		{
			ScriptBuilder attachBuilder = new ScriptBuilder();

			// pokud m·me script manager, odpojÌme st·vajÌcÌ nav·z·nÌ ud·lostÌ (kv˘li callback˘m)			
			if (IsScriptManager)
			{
				attachBuilder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().remove_pageLoaded(delegatex);");
			}

			foreach (IScriptletParameter scriptletParameter in scriptletParameters)
			{
				scriptletParameter.GetAttachEventsScript("data", this.NamingContainer, "handler", attachBuilder);
			}
			
			return attachBuilder.ToString();
		}

		private string PrepareClientSideScripts_GetDetachEventsFunctionCode()
		{
			ScriptBuilder detachBuilder = new ScriptBuilder();
			if (IsScriptManager)
			{
				detachBuilder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().remove_pageLoading(delegatex);");
			}
			foreach (IScriptletParameter scriptletParameter in scriptletParameters)
			{
				scriptletParameter.GetDetachEventsScript("data", this.NamingContainer, "handler", detachBuilder);
			}

			return detachBuilder.ToString();
		}
		
		private string PrepareClientSideScripts_WriteScriptWithReuse(ScriptBuilder mainBuilder, string functionName, string[] functionParameter, string code, string hashIdentifier, out bool reused)
		{

			if (String.IsNullOrEmpty(code))
			{
				reused = false;
				return null;
			}

			// vezmeme jmÈno funkce z cache
			string name = ScriptCacheHelper.GetFunctionNameFromCache(functionParameter, code);
			bool foundInCache = false;
			if (String.IsNullOrEmpty(name))
			{
				// pokud jsme jej nenaöli, pouûijeme zadanÈ jmÈno
				name = functionName;
				ScriptCacheHelper.AddFunctionToCache(name, functionParameter, code);
			}
			else
			{
				foundInCache = true;
			}

			string functionBlock = ClientScript.WrapClientSideScriptToFunction(name, functionParameter, code);
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
				mainBuilder.Append(functionBlock);
			}
			
			if (!reused && IsScriptManager)
			{
				ViewState[hashIdentifier] = hash;
			}


			return name;
		}

#warning comment
		public bool IsInAsyncPostBack
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


		public bool IsScriptManager
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
	}
}
