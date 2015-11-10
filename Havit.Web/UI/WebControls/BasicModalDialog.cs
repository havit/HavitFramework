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
	public class BasicModalDialog : ModalDialogBase
	{
		#region Private fields
		private readonly Panel _dialogPanel;
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

		#region GetContentContainer
		/// <summary>
		/// Vrací control/kontejner, do kterého je instanciována šablona obsahu.
		/// </summary>
		protected override Control GetContentContainer()
		{
			return _dialogPanel;
		}
		#endregion

		#region GetDialogContainer
		/// <summary>
		/// Vrací control/kontejner, který reprezentuje dialog jako celek. Tento control je ovládán klientskými skripty pro zobrazení a schování obsahu.
		/// </summary>
		protected override Control GetDialogContainer()
		{
			return _dialogPanel;
		}
		#endregion

		#region GetShowScript, GetHideScript
		/// <summary>
		/// Vrátí skript pro zobrazení dialogu na klientské straně.
		/// </summary>
		protected override string GetShowScript()
		{
			string script = String.Format(
				"havitSetDialogSize('{0}', '{1}', '{2}', '{3}', '{4}'); havitShowDialog('{0}');",
				GetDialogContainer().ClientID,
				Width.ToString(),
                Height.ToString(),
                MarginLeft.ToString(),
                MarginTop.ToString());
			return script;
		}

		/// <summary>
		/// Vrátí skript pro skrytí dialogu na klientské straně.
		/// </summary>
		protected override string GetHideScript()
		{
			return String.Format("havitHideDialog('{0}');", DialogPanelClientIDMemento ?? GetDialogContainer().ClientID);
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// OnPreRender.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			// šahnutím na hodnotu property se ID vygeneruje a VYRENDERUJE!
			// My jej musíme vyrenderovat vždy, protože jinak nefungují správně klientské skripty.
			string tmp = _dialogPanel.ClientID;
			
			CheckDialogSize();

			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery");

			_dialogPanel.Style[HtmlTextWriterStyle.Width] = Width.ToString();
			_dialogPanel.Style[HtmlTextWriterStyle.Height] = Height.ToString();
			_dialogPanel.Style[HtmlTextWriterStyle.MarginLeft] = MarginLeft.ToString();
			_dialogPanel.Style[HtmlTextWriterStyle.MarginTop] = MarginTop.ToString();

			base.OnPreRender(e);
		}
		#endregion

		#region RegisterHideScriptFromPreRenderComplete
		/// <summary>
		/// Zajistí schování dialogu.
		/// </summary>
		protected override void RegisterHideScriptFromPreRenderComplete()
		{
			ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);

			// pokud jsme v callbacku, vyrenderujeme skript schovávající dialog
			if (scriptManager != null && scriptManager.IsInAsyncPostBack)
			{
				RegisterHideScript();
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

			if (this.Width.Type != UnitType.Pixel)
			{
				throw new InvalidOperationException("Vlastnost Width není v pixelech.");
			}

			if (this.Height.Type != UnitType.Pixel)
			{
				throw new InvalidOperationException("Vlastnost Height není v pixelech.");
			}

			if (this.Width.Value != Math.Floor(this.Width.Value))
			{
				throw new InvalidOperationException("Vlastnost Width nesmí obsahovat desetinné číslo.");
			}

			if (this.Height.Value != Math.Floor(this.Height.Value))
			{
				throw new InvalidOperationException("Vlastnost Height nesmí obsahovat desetinné číslo.");
			}
		}
		#endregion
	}
}