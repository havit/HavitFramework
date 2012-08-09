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
    [ToolboxData("<{0}:Scriptlet runat=\"server\"><{0}:ClientScript runat=\"server\"></{0}:ClientScript></{0}:Scriptlet>")]
    [ControlBuilder(typeof(NoLiteralContolBuilder))]
    public class Scriptlet : Control
    {
		/// <summary>
		/// Vytvoøí instanci scriptletu a nastaví vıchozí hodnoty (ControlExtenderRepository a ScriptSubstitution).
		/// </summary>
		public Scriptlet()
		{
            // vezmeme si vıchozí repository
			controlExtenderRepository = Havit.Web.UI.Scriptlets.ControlExtenderRepository.Default;
            scriptSubstitution = ScriptSubstitutionRepository.Default;
		}

        /// <summary>
        /// Vrací název klienského objektu, kterı je parametrem volání klienské metody 
        /// generované v ClientScriptu.
        /// </summary>
        protected virtual string ClientSideObjectIdentifier
        {
            get
            {
                return "scriptletobject" + this.ClientID;
            }
        }

        /// <summary>
        /// Název funkce vygenerované ClientScriptem. Dostupné a po vygenerování skriptu.
        /// Pokud není funkce generována opakovanì (v repeateru, apod.) vrací název sdílené
        /// funkce.
        /// </summary>
        protected virtual string ClientSideFunctionName
        {
            get { return clientScript.ClientSideFunctionName; }
        }

        /// <summary>
        /// Vrací klientskı skript pro volání klientské funkce s klientskım parametrem.
        /// </summary>
        public string ClientSideFunctionCall
        {
            get
            {
                return String.Format("{0}({1});", ClientSideFunctionName, ClientSideObjectIdentifier);
            }
        }

		ClientScript clientScript = null;
        List<IScriptletParameter> scriptletParameters = new List<IScriptletParameter>();

		private IControlExtenderRepository controlExtenderRepository;

        /// <summary>
        /// Vrací nebo nastavuje repository extenderù pro parametry.
        /// </summary>
		public IControlExtenderRepository ControlExtenderRepository
		{
			get { return controlExtenderRepository; }
			set { controlExtenderRepository = value; }
		}

        private IScriptSubstitution scriptSubstitution;
        /// <summary>
        /// Vrací nebo nastavuje substituci pouitou pro tvorbu klienského skriptu.
        /// </summary>
        public IScriptSubstitution ScriptSubstitution
        {
            get { return scriptSubstitution; }
            set { scriptSubstitution = value; }
        }

        /// <summary>
        /// Zajistí tvorbu klienstkého skriptu.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!DesignMode)
            {
                // vytvoøíme klientskı skript
                string clientScript = PrepareClientScript();

                // zaregistrujeme jej na konec stránky, aby byly controly ji dostupné
                Page.ClientScript.RegisterStartupScript(
                    typeof(Scriptlet),
                    this.ClientID,
                    clientScript,
                    true
                );
            }
        }

        /// <summary>
        /// Sestaví kompletní klientskı skript seskládáním funkce, vytvoøení objektu 
        /// a jeho parametrù.
        /// </summary>
        /// <returns>Kompletní klientskı skript.</returns>
        protected virtual string PrepareClientScript()
        {
			ScriptBuilder builder = new ScriptBuilder();

            CreateClientSideFunction(builder);
            CreateClientSideObject(builder);
			CreateClientSideParameters(builder);

            return builder.ToString();
        }

		/// <summary>
		/// Zajistí, aby se na scriptletu nepouilo klasické renderování.
		/// Místo renderování se registrují klientské skripty v metodì OnPreRender.
		/// </summary>
		/// <param name="writer"></param>
        public override void RenderControl(HtmlTextWriter writer)
		{
			// nebudeme renderovat nic z vnitøku controlu
		}

        /// <summary>
        /// Vytvoøí klientskou funkci z objektu typu ClientScript.
        /// </summary>
        /// <param name="builder">Script builder.</param>
        protected virtual void CreateClientSideFunction(ScriptBuilder builder)
        {
            if (clientScript != null)
                clientScript.CreateClientSideScript(builder);
            else
                throw new ArgumentException("ClientScript nebyl zadán.");
        }

        /// <summary>
        /// Vytvoøí skript pro objekt na klientské stranì.
        /// </summary>
        /// <param name="builder">Script builder.</param>
        protected virtual void CreateClientSideObject(ScriptBuilder builder)
        {
			builder.AppendFormat("var {0} = new Object();\n", ClientSideObjectIdentifier);
        }

        /// <summary>
        /// Vytvoøí parametry klintského objektu.
        /// </summary>
        /// <param name="builder">Script builder.</param>
        protected virtual void CreateClientSideParameters(ScriptBuilder builder)
		{
            foreach (IScriptletParameter scriptletParameter in scriptletParameters)
			{
                scriptletParameter.CheckProperties();
                scriptletParameter.CreateParameter(ClientSideObjectIdentifier, this.NamingContainer, builder);
			}
        }

		/// <summary>
		/// Zavoláno, kdy je do kolekce Controls pøidán Control.
		/// Zajišuje, aby nebyl pøidán control neimplementující 
		/// IScriptletParameter nebo ClientScript.
		/// Zároveò zajistí, aby nebyl pøidán více ne jeden ClientScript.
		/// </summary>
		/// <param name="control">Pøidávanı control.</param>
		/// <param name="index">Pozice v kolekci controlù, kam je control pøidáván.</param>
		protected override void AddedControl(Control control, int index)
        {
            base.AddedControl(control, index);

            // zajistíme, aby nám do scriptletu nepøišel neznámı control
            if (!(control is ScriptletNestedControl))
                throw new ArgumentException(String.Format("Do Scriptletu je vkládán nepodporovanı control {0}.", control.ID));

            if (control is ClientScript)
            {
                if (clientScript != null)
                    throw new ArgumentException("Scriptlet musí obsahovat ClientScript právì jednou.");

                clientScript = (ClientScript)control;
            }

            if (control is IScriptletParameter)
                scriptletParameters.Add((IScriptletParameter)control);
        }
    }
}
