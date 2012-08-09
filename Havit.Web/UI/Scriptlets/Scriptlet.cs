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
	/// Scriptlet umoòuje snadnou tvorbu klientskıch skriptù.
	/// </summary>
	[ControlBuilder(typeof(NoLiteralContolBuilder))]	
	public class Scriptlet : Control
	{
		#region Private fields
		private ClientScript clientScript = null;
		private List<IScriptletParameter> scriptletParameters = new List<IScriptletParameter>();
		#endregion

		#region ClientSideGetDataObjectFunctionName
		/// <summary>
		/// Vrací skrit volání klienské metody vracející objekt nesoucí parametry scriptletu.
		/// </summary>
		protected virtual string ClientSideGetDataObjectFunctionName
		{
			get
			{
				return "scriptletGetDataObject_" + this.ClientID;
			}
		}		
		#endregion

		#region ClientSideGetDataObjectFunctionCall
		/// <summary>
		/// Vrací skrit volání klienské metody vracející objekt nesoucí parametry scriptletu.
		/// </summary>
		protected virtual string ClientSideGetDataObjectFunctionCall
		{
			get
			{
				return ClientSideGetDataObjectFunctionName + "()";
			}
		}
		#endregion

		#region ClientSideScriptFunctionName
		///<summary>
		///Název funkce vygenerované ClientScriptem. Dostupné a po vygenerování skriptu.
		///Pokud není funkce generována opakovanì (v repeateru, apod.) vrací název sdílené
		///funkce.
		///</summary>
		protected virtual string ClientSideScriptFunctionName
		{
			get { return clientScript.ClientSideScriptFunctionName; }
		}
		#endregion

		#region ClientSideScriptFunctionCall
		/// <summary>
		/// Vrací klientskı skript pro volání klientské funkce s klientskım parametrem.
		/// </summary>
		public string ClientSideScriptFunctionCall
		{
			get
			{
				return String.Format("{0}({1});", ClientSideScriptFunctionName, ClientSideGetDataObjectFunctionCall);
			}
		}
		#endregion
		
		#region ControlExtenderRepository
		/// <summary>
		/// Vrací nebo nastavuje repository extenderù pro parametry.
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
		/// Vrací nebo nastavuje substituci pouitou pro tvorbu klienského skriptu.
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
		/// Vytvoøí instanci scriptletu a nastaví vıchozí hodnoty
		/// <see cref="ControlExtenderRepository">ControlExtenderRepository</see>
		/// (na <see cref="ControlExtenderRepository.Default">ControlExtenderRepository.Default</see>)
		/// a <see cref="ScriptSubstitution">ScriptSubstitution</see>
		/// (na <see cref="ScriptSubstitutionRepository.Default">ScriptSubstitutionRepository.Default</see>).
		/// </summary>
		public Scriptlet()
		{
			// vezmeme si vıchozí repository
			controlExtenderRepository = Havit.Web.UI.Scriptlets.ControlExtenderRepository.Default;
			scriptSubstitution = ScriptSubstitutionRepository.Default;
		}

		#endregion

		#region AddedControl
		/// <summary>
		/// Zavoláno, kdy je do kolekce Controls pøidán Control.
		/// Zajišuje, aby nebyl pøidán control neimplementující 
		/// <see cref="IScriptletParameter">IScriptletParameter</see>
		/// nebo <see cref="ClientScript">ClientScript</see>.
		/// Zároveò zajistí, aby nebyl pøidán více ne jeden <see cref="ClientScript">ClientScript</see>.
		/// </summary>
		/// <param name="control">Pøidávanı control.</param>
		/// <param name="index">Pozice v kolekci controlù, kam je control pøidáván.</param>
		protected override void AddedControl(Control control, int index)
		{
			base.AddedControl(control, index);

			// zajistíme, aby nám do scriptletu nepøišel neznámı control
			if (!(control is ScriptletNestedControl))
			{
				throw new ArgumentException(String.Format("Do Scriptletu je vkládán nepodporovanı control {0}.", control.ID));
			}

			if (control is ClientScript)
			{
				if (clientScript != null)
				{
					throw new ArgumentException("Scriptlet musí obsahovat ClientScript právì jednou.");
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
		/// Zajistí tvorbu klienstkého skriptu.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			CheckControlConditions();
			PrepareAndRegisterClientScript();
		}
		#endregion
		
		#region CheckControlConditions
		/// <summary>
		/// Ovìøí, zda jsou správnì zadány parametry scriptletu (testuje, zda byl zadán ClientScript).
		/// </summary>
		protected virtual void CheckControlConditions()
		{
			if (clientScript == null)
			{
				throw new HttpException("ClientScript nebyl zadán.");
			}
		}
		#endregion
		
		#region RenderControl
		///// <summary>
		///// Zajistí tvorbu klienstkého skriptu.
		///// </summary>
		//protected override void Render(HtmlTextWriter writer)
		//{
		//    base.Render(writer);

		//    ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
		//    if ((scriptManager != null) && scriptManager.IsInAsyncPostBack)
		//    {
		//        PrepareAndRegisterClientScript();
		//    }
		//}

		/// <summary>
		/// Zajistí, aby se na scriptletu nepouilo klasické renderování.
		/// Místo renderování se registrují klientské skripty v metodì OnPreRender.
		/// </summary>
		/// <param name="writer"></param>
		public override void RenderControl(HtmlTextWriter writer)
		{
			// nebudeme renderovat nic z vnitøku controlu
		}		
		#endregion

		#region PrepareAndRegisterClientScript
		/// <summary>
		/// Sestaví kompletní klientskı skript seskládáním funkce, vytvoøení objektu 
		/// a jeho parametrù. Zaregistruje skripty do stránky
		/// </summary>
		/// <returns>Kompletní klientskı skript.</returns>
		protected virtual void PrepareAndRegisterClientScript()
		{
			if (DesignMode)
			{
				return;
			}

			ScriptBuilder builder = new ScriptBuilder();

			PrepareClientSideScripts(builder);

			// zaregistrujeme jej na konec stránky, aby byly controly ji dostupné
			ScriptManager.RegisterStartupScript(
				this,
				typeof(Scriptlet),
				this.UniqueID,
				builder.ToString(),
				true
			);
		}
		#endregion
		
		#region PrepareClientSideScripts
		/// <summary>
		/// Vrátí klientskı skript scriptletu.
		/// </summary>
		/// <param name="builder">Script builder.</param>
		protected virtual void PrepareClientSideScripts(ScriptBuilder builder)
		{
			ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);

			// nejdøíve musíme vytvoøit funkci, abychom získali jméno obálky
			// (navíc mùe dojít k reuse scriptu)
			
			// získáme funkci reprezentující ClientScript	
			clientScript.GetClientSideScriptFunction(builder);

			// nyní ji se mùeme zeptat na jméno klientské funkce se skriptem
			string attachEventsFunctionName = ClientSideScriptFunctionName + "_AttachEvents";
			string detachEventsFunctionName = ClientSideScriptFunctionName + "_DetachEvents";

			// vytvoøíme funkci pro získání objektu nesoucí parametry
			builder.AppendLineFormat("function {0}()", ClientSideGetDataObjectFunctionName);
			builder.AppendLine("{");
			builder.AppendLine("var result = new Object();");
			foreach (IScriptletParameter scriptletParameter in scriptletParameters)
			{
				scriptletParameter.CheckProperties();
				scriptletParameter.GetInitializeClientSideValueScript("result", this.NamingContainer, builder);
			}
			builder.AppendLine("return result;");
			builder.AppendLine("}");

			builder.AppendLineFormat("function {0}()", attachEventsFunctionName);
			builder.AppendLine("{");

			// pokud pouíváme klientské události ASP.NET AJAXu, potøebujeme se odpojit od pageLoaded
			builder.AppendLine("if (!((typeof(Sys) == 'undefined') || (typeof(Sys.WebForms) == 'undefined')))");
			builder.AppendLine("{");
			builder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().remove_pageLoaded({0});", attachEventsFunctionName);
			builder.AppendLine("}");

			builder.AppendLineFormat("var data = {0};", ClientSideGetDataObjectFunctionCall);
			foreach (IScriptletParameter scriptletParameter in scriptletParameters)
			{
				scriptletParameter.GetAttachEventsScript("data", this.NamingContainer, builder);
			}
			
			builder.AppendLine("}");

			// pro neAJAXové stránky scriptlet nepotøebuje odpojovat události
			if (scriptManager != null)
			{
				builder.AppendLineFormat("function {0}()", detachEventsFunctionName);
				builder.AppendLine("{");

				// pokud pouíváme klientské události ASP.NET AJAXu, potøebujeme se odpojit od pageLoadingu
				builder.AppendLine("if (!((typeof(Sys) == 'undefined') || (typeof(Sys.WebForms) == 'undefined')))");
				builder.AppendLine("{");
				builder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().remove_pageLoading({0});", detachEventsFunctionName);
				builder.AppendLine("}");
				
				builder.AppendLineFormat("var data = {0};", ClientSideGetDataObjectFunctionCall);
				foreach (IScriptletParameter scriptletParameter in scriptletParameters)
				{
					scriptletParameter.GetDetachEventsScript("data", this.NamingContainer, builder);				
				}
				builder.AppendLine("}");
			}
		
			clientScript.GetClientSideStartupScript(builder);
						
			if (scriptManager == null)
			{
				builder.AppendLine(BrowserHelper.GetAttachEventScript("window", "onload", attachEventsFunctionName));
			}
			else
			{
				builder.AppendLine("if ((typeof(Sys) == 'undefined') || (typeof(Sys.WebForms) == 'undefined'))");
				builder.AppendLine("{");
				builder.AppendLine(BrowserHelper.GetAttachEventScript("window", "onload", attachEventsFunctionName));
				builder.AppendLine("}");
				builder.AppendLine("else");
				builder.AppendLine("{");
				//builder.AppendLine("if (typeof(document.scriptletEvents" + this.UniqueID + "Registered) == 'undefined')");
				//builder.AppendLine("{");				
				// pageLoading nám zajistí odebrání událostí ještì pøed vımìnou elementù v dokumentu
				builder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().add_pageLoading({0});", detachEventsFunctionName);
				// pageLoaded nám zajistí navázání událostí po vımìnì elementù
				builder.AppendLineFormat("Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded({0});", attachEventsFunctionName);
				builder.AppendLine("document.scriptletEvents" + this.UniqueID + "Registered = true;");				
				builder.AppendLine("}");
				//builder.AppendLine("}");
			}
		}
		#endregion
	}
}
