using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI.ClientScripts;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Základní třída pro modální dialog. Umí pozicovat, zobrazovat a skrývat dialog.
	/// </summary>
	[ParseChildren(true)]
	[PersistChildren(false)]
	public abstract class ModalDialogBase : Control
	{

		protected abstract Control GetDialogContainer();


		#region Private fields
		private bool _dialogCurrentlyHiding = false;
		#endregion

		#region DialogPanelClientIDMemento
		/// <summary>
		/// Paměť pro _dialogPanel.ClientID.
		/// Využívá metoda RegisterHideScript (volána z Page_PreRenderComplete) pro registraci scriptu pro schování dialogu na klientské straně.
		/// Slouží pro řešení situace, kdy potřebujeme ze stránky vyhodit control, protože již ve stránce neexistuje (byl vyhozen databindingem, atp.).
		/// </summary>
		protected string DialogPanelClientIDMemento
		{
			get
			{
				return (string)ViewState["DialogPanelClientIDMemento"];
			}
			set
			{
				ViewState["DialogPanelClientIDMemento"] = value;
			}
		}
		#endregion

		#region ContentTemplate
		/// <summary>
		/// Šablona obsahu dialogu. Instancovaný obsah šablony je v ContentTemplateContainer (instancováno v průběhu OnInit).
		/// </summary>
		[TemplateInstance(TemplateInstance.Single)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ITemplate ContentTemplate
		{
			get
			{
				return _contentTemplate;
			}
			set
			{
				_contentTemplate = value;
				ChildControlsCreated = false;
			}
		}
		private ITemplate _contentTemplate;
		#endregion		

		#region ContentTemplateContainer
		/// <summary>
		/// Instancovaný obsah dialogu.
		/// </summary>
		internal Control ContentTemplateContainer
		{
			get
			{
				EnsureChildControls();
				return GetContentContainer();
			}
		}		
		#endregion

		#region Controls
		/// <summary>
		/// Kolekce controlů.
		/// Přístup k property zajistí inicializaci podstromu controlů (EnsureChildControls).
		/// </summary>
		public override ControlCollection Controls
		{
			get
			{
				EnsureChildControls();
				return base.Controls;
			}
		}
		#endregion		

		#region DialogVisible
		/// <summary>
		/// Udává, zda je dialog viditelný.
		/// </summary>
		protected internal bool DialogVisible
		{
			get
			{
				return (bool)(ViewState["DialogVisible"] ?? false);
			}
			private set
			{
				ViewState["DialogVisible"] = value;
			}
		}
		#endregion

		#region OnInit
		/// <summary>
		/// OnInit.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EnsureChildControls();
			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Inicializuje podstrom controlů.
		/// </summary>
		protected override void CreateChildControls()
		{
			this.Controls.Clear();
			Control contentContainer = GetContentContainer();
			if (this._contentTemplate != null)
			{
				_contentTemplate.InstantiateIn(contentContainer);
			}
			Control dialogContainer = GetDialogContainer();
			if (dialogContainer != this)
			{
				this.Controls.Add(dialogContainer);
			}
			dialogContainer.ID = this.ID + "__DC";
		}
		#endregion	

		#region GetContentContainer
		/// <summary>
		/// Vrací kontejner, do kterého je instanciována šablona.
		/// </summary>
		protected abstract Control GetContentContainer();
		#endregion		

		#region Show, Hide, OnDialogShown, OnDialogHidden
		/// <summary>
		/// Zobrazí dialog.
		/// </summary>
		public void Show()
		{
			DialogVisible = true;
			DialogPanelClientIDMemento = GetDialogContainer().ClientID;
			OnDialogShown(EventArgs.Empty);
		}

		/// <summary>
		/// Skryje dialog.
		/// </summary>
		public void Hide()
		{
			DialogVisible = false;
			_dialogCurrentlyHiding = true;
			OnDialogHidden(EventArgs.Empty);
		}

		/// <summary>
		/// Obsluhuje událost zobrazení dialogu.
		/// </summary>
		protected virtual void OnDialogShown(EventArgs eventArgs)
		{
			if (DialogShown != null)
			{
				DialogShown(this, eventArgs);
			}
		}

		/// <summary>
		/// Obsluhuje událost skrytí dialogu.
		/// </summary>
		protected virtual void OnDialogHidden(EventArgs eventArgs)
		{
			if (DialogHidden != null)
			{
				DialogHidden(this, eventArgs);
			}
		}
		#endregion

		#region GetShowScript, GetHideScript, RegisterShowScript, RegisterHideScript
		/// <summary>
		/// Vrátí skript pro zobrazení dialogu na klientské straně.
		/// </summary>
		public abstract string GetShowScript();

		/// <summary>
		/// Vrátí skript pro skrytí dialogu na klientské straně.
		/// </summary>
		public abstract string GetHideScript();

		/// <summary>
		/// Zaregistruje skript, který zobrazí dialog na klientské straně.
		/// </summary>
		private void RegisterShowScript()
		{
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery");
			string script = String.Format(
				"$(document).ready(function() {{ {0} }});",
				GetShowScript());
			ScriptManager.RegisterStartupScript(this.Page, typeof(BasicModalDialog), this.ClientID, script, true);
		}

		/// <summary>
		/// Zaregistruje skript, který skryje dialog na klientské straně.
		/// </summary>
		private void RegisterHideScript()
		{
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery");
			string script = String.Format(
				"$(document).ready(function() {{ {0} }});",
				GetHideScript());
			ScriptManager.RegisterStartupScript(this.Page, typeof(BasicModalDialog), this.ClientID, script, true);
		}
		#endregion

		#region DialogShown, DialogHidden
		/// <summary>
		/// Událost oznamující zobrazení dialogu.
		/// </summary>
		public event EventHandler DialogShown;

		/// <summary>
		/// Událost oznamujíxí skrytí dialogu.
		/// </summary>
		public event EventHandler DialogHidden;
		#endregion

		#region OnPreRender
		/// <summary>
		/// OnPreRender.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			EnsureChildControls();

			if (DialogVisible)
			{
				RegisterShowScript();
			}
		}
		#endregion

		#region Page_PreRenderComplete
		private void Page_PreRenderComplete(object sender, EventArgs e)
		{
			// Dialog nemá být vidět a právě jej schováváme.
			// To se může stát, že control již není ve stránce (např. byl v repeateru, který byl rebindován) 
			// nebo je nadřazený element schovaný a pak se nevyvolá OnPreRender,
			// proto zkusíme control schovat v každém případě (Page.PreRenderComplete)
						
			if (!DialogVisible && _dialogCurrentlyHiding)
			{
				ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);

				// pokud jsme v callbacku, vyrenderujeme skript schovávající dialog
				if (scriptManager != null && scriptManager.IsInAsyncPostBack)
				{
					RegisterHideScript();
				}
			}
		}
		#endregion
	}
}
