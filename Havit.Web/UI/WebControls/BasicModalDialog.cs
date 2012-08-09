using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Základní třída pro modální dialog. Umí pozicovat, zobrazovat a skrývat dialog.
	/// </summary>
	[ParseChildren(true)]
	[PersistChildren(false)]
	public class BasicModalDialog: Control
	{
		#region Private fields
		private Panel _dialogPanel;
		private bool _dialogCurrentlyHiding = false;
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
				base.ChildControlsCreated = false;
			}
		}
		private ITemplate _contentTemplate;
		#endregion		

		#region ContentTemplateContainer
		/// <summary>
		/// Instancovaný obsah dialogu.
		/// </summary>
		public Control ContentTemplateContainer
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

		#region Width, Height, MarginLeft, MarginTop
		/// <summary>
		/// Šířka dialogu v pixelech.
		/// </summary>
		public Unit Width
		{
			get
			{
				return (Unit)(ViewState["Width"] ?? Unit.Empty);
			}
			set
			{
				ViewState["Width"] = value;
			}
		}

		/// <summary>
		/// Výška dialogu v pixelech.
		/// </summary>
		public Unit Height
		{
			get
			{
				return (Unit)(ViewState["Height"] ?? Unit.Empty);
			}
			set
			{
				ViewState["Height"] = value;
			}
		}
		
		/// <summary>
		/// Určeno pro centrování dialogu: Posun dialogu doleva vůči středu. 
		/// </summary>
		private Unit MarginLeft
		{
			get
			{
				return new Unit(-1 * Width.Value / 2, Width.Type);
			}
		}

		/// <summary>
		/// Určeno pro centrování dialogu: Posun dialogu nahoru vůči středu. 
		/// </summary>
		private Unit MarginTop
		{
			get
			{
				return new Unit(-1 * Height.Value / 2, Height.Type);
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

		#region Constructors
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public BasicModalDialog()
		{
			_dialogPanel = new Panel();
			_dialogPanel.CssClass = "webdialog";
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
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Inicializuje podstrom controlů.
		/// </summary>
		protected override void CreateChildControls()
		{
			this.Controls.Clear();
			if (this._contentTemplate != null)
			{
				_contentTemplate.InstantiateIn(GetContentContainer());
			}
			this.Controls.Add(_dialogPanel);
		}
		#endregion	

		#region GetContentContainer
		/// <summary>
		/// Vrací kontejner, do kterého je instanciována šablona.
		/// </summary>
		/// <returns></returns>
		protected virtual Control GetContentContainer()
		{
			return _dialogPanel;
		}
		#endregion		

		#region Show, Hide, OnDialogShown, OnDialogHidden
		/// <summary>
		/// Zobrazí dialog.
		/// </summary>
		public void Show()
		{
			DialogVisible = true;
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
		/// <param name="eventArgs"></param>
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
		public string GetShowScript()
		{
			string script = String.Format(
				"havitSetDialogSize('{0}', '{1}', '{2}', '{3}', '{4}'); havitShowDialog('{0}');",
				_dialogPanel.ClientID,
				Width.ToString(),
				Height.ToString(),
				MarginLeft.ToString(),
				MarginTop.ToString());
			return script;
		}

		/// <summary>
		/// Vrátí skript pro skrytí dialogu na klientské straně.
		/// </summary>
		public string GetHideScript()
		{
			return String.Format("havitHideDialog('{0}');", _dialogPanel.ClientID);
		}

		/// <summary>
		/// Zaregistruje skript, který zobrazí dialog na klientské straně.
		/// </summary>
		private void RegisterShowScript()
		{
			string script = String.Format(
				"window.setTimeout(new Function(\"{0}\"), 0);",
				GetShowScript());
			ScriptManager.RegisterStartupScript(this.Page, typeof(BasicModalDialog), this.ClientID, script, true);
		}

		/// <summary>
		/// Zaregistruje skript, který skryje dialog na klientské straně.
		/// </summary>
		private void RegisterHideScript()
		{
			string script = String.Format(
				"window.setTimeout(new Function(\"{0}\"), 0);",
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

			CheckDialogSize();

			_dialogPanel.Style[HtmlTextWriterStyle.Width] = Width.ToString();
			_dialogPanel.Style[HtmlTextWriterStyle.Height] = Height.ToString();
			_dialogPanel.Style[HtmlTextWriterStyle.MarginLeft] = MarginLeft.ToString();
			_dialogPanel.Style[HtmlTextWriterStyle.MarginTop] = MarginTop.ToString();

			if (DialogVisible)
			{
				RegisterShowScript();
			}
			else
			{
				if (_dialogCurrentlyHiding)
				{
					ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);

					// pokud jsme v callbacku, vyrenderujeme skript schovávající dialog
					if (scriptManager != null && scriptManager.IsInAsyncPostBack)
					{
						RegisterHideScript();
					}
				}
			}
		}
		#endregion

		#region CheckDialogSize
		/// <summary>
		/// Ověří správné nastavení vlastností controlu.
		/// Ověřuje nastavení vlastností Width a Height, 
		/// pokud nejsou kladné (výchozí = 0) je vyvolána výjimka InvalidOperationException.
		/// </summary>
		protected virtual void CheckDialogSize()
		{
			if (this.Width == Unit.Empty)
			{
				throw new InvalidOperationException("Není nastavena vlastnost Width.");
			}

			if (this.Height == Unit.Empty)
			{
				throw new InvalidOperationException("Není nastavena vlastnost Height.");
			}
		}
		#endregion
	}
}
