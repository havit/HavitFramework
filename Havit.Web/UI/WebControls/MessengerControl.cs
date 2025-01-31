using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Zobrazuje zprávy Messengera.
/// </summary>
public class MessengerControl : Literal
{
	/// <summary>
	/// Messenger použitý pro zprávy k zobrazení.
	/// Není-li nastaven, vrací Messenger.Default.
	/// </summary>
	public Messenger Messenger
	{
		get
		{
			return _messenger == null ? Messenger.Default : _messenger;
		}
		set
		{
			_messenger = value;
		}
	}
	private Messenger _messenger;

	/// <summary>
	/// Indikuje, zda se budou zprávy messengeru zobrazovat (bez ohledu na způsob) a čistit z fronty zpráv.
	/// </summary>
	public bool Enabled
	{
		get { return (bool)(ViewState["Enabled"] ?? true); }
		set { ViewState["Enabled"] = value; }
	}

	/// <summary>
	/// Indikuje, zda se budou zprávy messengeru zobrazovat v message boxu (alert).
	/// </summary>
	public bool ShowMessageBox
	{
		get { return (bool)(ViewState["ShowMessageBox"] ?? false); }
		set { ViewState["ShowMessageBox"] = value; }
	}

	/// <summary>
	/// Indikuje, zda se budou zprávy messengeru renderovat do stránky.
	/// </summary>
	public bool ShowSummary
	{
		get { return (bool)(ViewState["ShowSummary"] ?? true); }
		set { ViewState["ShowSummary"] = value; }
	}

	/// <summary>
	/// Indikuje, zda se budou zprávy messengeru renderovat pro toastr (https://github.com/CodeSeven/toastr).
	/// </summary>
	public bool ShowToastr
	{
		get { return (bool)(ViewState["ShowToastr"] ?? false); }
		set { ViewState["ShowToastr"] = value; }
	}

	/// <summary>
	/// OnInit.
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		this.Page.PreRenderComplete += Page_PreRenderComplete;
	}

	/// <summary>
	/// Registrace knihoven jquery a toastru, pokud je použit toastr (PreRenderComplete je pozdě).
	/// </summary>
	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		if (Enabled && ShowToastr)
		{
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery");
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "toastr");
		}
	}

	/// <summary>
	/// Vyrenderuje html a/nebo script messengeru.
	/// </summary>
	private void Page_PreRenderComplete(object sender, EventArgs e)
	{
		if (this.Enabled)
		{
			// Pokud dojde k redirectu s endRequest=false (voláno takto mj. z FormsAuthentication.RedirectFromLoginPage) před voláním této metody,
			// potom i v takovém případě proběhne tento OnPreRender, který vyčistí zprávy messengeru. Následně však dojde k přesměrování namísto zobrazení obsahu,
			// takže uživatel zprávy nevidí.
			// Touto podmínkou zajistíme, aby MessengerControl nezpracovával zprávy, pokud je známo, že následně dojde k přesměrování (namísto zobrazení obsahu stránky).
			if (!this.Page.Response.IsRequestBeingRedirected)
			{
				if (this.ShowSummary)
				{
					this.Text = this.GetSummaryHtml();
				}

				if (this.ShowMessageBox)
				{
					string messageBoxText = this.GetMessageBoxText();
					if (!String.IsNullOrEmpty(messageBoxText))
					{
						//string script = String.Format("alert('{0}');", messageBoxText.Replace("'", "\\'"));
						string script = String.Format("window.setTimeout(function() {{ alert('{0}'); }}, 10);", messageBoxText.Replace("'", "\\'"));
						System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, typeof(MessengerControl), "MessageBox", script, true);
					}
				}

				if (this.ShowToastr)
				{
					string toastrScript = this.GetToastrScript();
					if (!String.IsNullOrEmpty(toastrScript))
					{
						System.Web.UI.ScriptManager.RegisterStartupScript(this.Page, typeof(MessengerControl), "Toastr", toastrScript, true);
					}
				}

				this.Messenger.Clear();
			}
		}
	}

	/// <summary>
	/// Vrátí obsah messengeru (HTML kód) připravený k vyrenderování do stránky.
	/// </summary>
	protected virtual string GetSummaryHtml()
	{
		if (Messenger.GetMessages().Count > 0)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<div class=\"tmessenger\">");
			foreach (MessengerMessage message in Messenger.GetMessages())
			{
				AddMessageSummaryHtml(message, sb);
			}
			sb.AppendLine("</div>");

			return sb.ToString();
		}
		else
		{
			return String.Empty;
		}
	}

	/// <summary>
	/// Vrátí text zprávy messengeru (HTML kód) připravený k vyrenderování do stránky.
	/// </summary>
	protected virtual void AddMessageSummaryHtml(MessengerMessage message, StringBuilder sb)
	{
		Debug.Assert(message != null);
		Debug.Assert(sb != null);

		string messageCssClass;
		switch (message.MessageType)
		{
			case MessageType.Information:
				messageCssClass = "tmessageinformation";
				break;
			case MessageType.Warning:
				messageCssClass = "tmessagewarning";
				break;
			case MessageType.Error:
				messageCssClass = "tmessageerror";
				break;
			default:
				throw new InvalidOperationException("Neznámý typ zprávy.");
		}

		sb.Append("<div class=\"");
		sb.Append(messageCssClass);
		sb.Append("\">");

		sb.Append("<span class=\"tmessagetext\">");
		sb.Append(message.Text.TrimEnd().Replace("\n", "<br/>\n").Replace("\r", ""));
		sb.Append("</span>");

		sb.Append("</div>");
	}

	/// <summary>
	/// Vrátí obsah messengeru (text) připravený k zobrazení v message boxu (alert).
	/// </summary>
	protected virtual string GetMessageBoxText()
	{
		if (Messenger.GetMessages().Count > 0)
		{
			StringBuilder sb = new StringBuilder();
			foreach (MessengerMessage message in Messenger.GetMessages())
			{
				AddMessageBoxText(message, sb);
			}
			return sb.ToString();
		}
		return String.Empty;
	}

	/// <summary>
	/// Vrátí text zprávy messengeru připravený k zobrazení v message boxu (alert).
	/// </summary>
	protected virtual void AddMessageBoxText(MessengerMessage message, StringBuilder sb)
	{
		sb.Append(message.Text.TrimEnd().Replace("\n", "\\n").Replace("\r", ""));
		sb.Append("\\n");
	}

	/// <summary>
	/// Vrací script pro vyrenderování obsahu pro toastr.
	/// </summary>
	protected virtual string GetToastrScript()
	{
		if (Messenger.GetMessages().Count > 0)
		{
			StringBuilder sb = new StringBuilder();
			foreach (MessengerMessage message in Messenger.GetMessages())
			{
				AddToastrScript(message, sb);
			}
			return sb.ToString();
		}
		return String.Empty;
	}

	private void AddToastrScript(MessengerMessage message, StringBuilder sb)
	{
		string toasterMessageType;
		switch (message.MessageType)
		{
			case MessageType.Information:
				toasterMessageType = "success"; // enum má sice hodnotu information, ale používáme ve smyslu success
				break;

			case MessageType.Error:
				toasterMessageType = "error";
				break;

			case MessageType.Warning:
				toasterMessageType = "warning";
				break;

			default:
				throw new ApplicationException("Neznámá hodnota MessageType.");
		}

		string toasterMessage = message.Text.TrimEnd().Replace("'", "\\'").Replace("\n", "<br />").Replace("\r", "");
		sb.AppendFormat("toastr.{0}('{1}');", toasterMessageType, toasterMessage);
	}
}
