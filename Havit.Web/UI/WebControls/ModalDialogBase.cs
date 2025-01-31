﻿using System.ComponentModel;
using System.Web.UI;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Základní infrastruktura pro modální dialog. Umí zobrazovat a skrývat dialog, neřeší však implementační záležitosti.
/// </summary>
[ParseChildren(true)]
[PersistChildren(false)]
public abstract class ModalDialogBase : Control
{
	private bool _dialogCurrentlyHiding = false;

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

	/// <summary>
	/// OnInit.
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		EnsureChildControls();
		this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
	}

	/// <summary>
	/// Inicializuje podstrom controlů.
	/// </summary>
	protected override void CreateChildControls()
	{
		this.Controls.Clear();
		Control contentContainer = GetContentContainer();
		if (!contentContainer.HasControls() && (_contentTemplate != null))
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

	/// <summary>
	/// Vrací control/kontejner, do kterého je instanciována šablona obsahu.
	/// </summary>
	protected abstract Control GetContentContainer();

	/// <summary>
	/// Vrací control/kontejner, který reprezentuje dialog jako celek. Tento control je ovládán klientskými skripty pro zobrazení a schování obsahu.
	/// </summary>
	protected abstract Control GetDialogContainer();

	/// <summary>
	/// Zobrazí dialog.
	/// </summary>
	public void Show()
	{
		if (!DialogVisible)
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			OnDialogShowing(cancelEventArgs);
			if (!cancelEventArgs.Cancel)
			{
				DialogVisible = true;
				DialogPanelClientIDMemento = GetDialogContainer().ClientID;
				OnDialogShown(EventArgs.Empty);
			}
		}
	}

	/// <summary>
	/// Skryje dialog.
	/// </summary>
	public void Hide()
	{
		if (DialogVisible)
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			OnDialogHiding(cancelEventArgs);
			if (!cancelEventArgs.Cancel)
			{
				DialogVisible = false;
				_dialogCurrentlyHiding = true;
				OnDialogHidden(EventArgs.Empty);
			}
		}
	}

	/// <summary>
	/// Obsluhuje událost před zobrazením dialogu.
	/// </summary>
	protected virtual void OnDialogShowing(CancelEventArgs eventArgs)
	{
		if (DialogShowing != null)
		{
			DialogShowing(this, eventArgs);
		}
	}

	/// <summary>
	/// Obsluhuje událost před zobrazením dialogu.
	/// </summary>
	protected virtual void OnDialogHiding(CancelEventArgs eventArgs)
	{
		if (DialogHiding != null)
		{
			DialogHiding(this, eventArgs);
		}
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

	/// <summary>
	/// Vrátí skript pro zobrazení dialogu na klientské straně.
	/// </summary>
	protected abstract string GetShowScript();

	/// <summary>
	/// Vrátí skript pro skrytí dialogu na klientské straně.
	/// </summary>
	protected abstract string GetHideScript();

	/// <summary>
	/// Zaregistruje skript, který zobrazí dialog na klientské straně.
	/// </summary>
	protected void RegisterShowScript()
	{
		ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery");

		string showScript = GetShowScript();
		if (!String.IsNullOrEmpty(showScript))
		{
			string script = String.Format("$(document).ready(function() {{ {0} }});", showScript);
			ScriptManager.RegisterStartupScript(this.Page, typeof(BasicModalDialog), this.ClientID, script, true);
		}
	}

	/// <summary>
	/// Zaregistruje skript, který skryje dialog na klientské straně.
	/// </summary>
	protected void RegisterHideScript()
	{
		ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery");
		string script = String.Format(
			"$(document).ready(function() {{ {0} }});",
			GetHideScript());
		ScriptManager.RegisterStartupScript(this.Page, typeof(BasicModalDialog), this.ClientID, script, true);
	}

	/// <summary>
	/// Událost oznamující začátek zobrazení dialogu.
	/// </summary>
	public event CancelEventHandler DialogShowing;

	/// <summary>
	/// Událost oznamující zobrazení dialogu.
	/// </summary>
	public event EventHandler DialogShown;

	/// <summary>
	/// Událost oznamující začátek skrytí dialogu.
	/// </summary>
	public event CancelEventHandler DialogHiding;

	/// <summary>
	/// Událost oznamující skrytí dialogu.
	/// </summary>
	public event EventHandler DialogHidden;

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

	private void Page_PreRenderComplete(object sender, EventArgs e)
	{
		// Dialog nemá být vidět a právě jej schováváme.
		// To se může stát, že control již není ve stránce (např. byl v repeateru, který byl rebindován) 
		// nebo je nadřazený element schovaný a pak se nevyvolá OnPreRender,
		// proto zkusíme control schovat v každém případě (Page.PreRenderComplete)

		if (!DialogVisible && _dialogCurrentlyHiding)
		{
			RegisterHideScriptFromPreRenderComplete();
			RegisterHideScript();
		}
	}

	/// <summary>
	/// Zajistí schování dialogu
	/// </summary>
	protected abstract void RegisterHideScriptFromPreRenderComplete();
}
