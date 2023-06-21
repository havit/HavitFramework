using Havit.AspNet.Mvc.Messenger;
using Havit.AspNet.Mvc.Messenger.Renderers;
using Havit.AspNet.Mvc.Messenger.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Havit.AspNet.Mvc.Html;

/// <summary>
/// Extension metods for Messenger.
/// </summary>
public static class MessengerExtensions
{
	/// <summary>
	/// Renders HTML for messenger.
	/// </summary>
	public static MvcHtmlString Messenger(this HtmlHelper helper)
	{
		IMessageStorage messageStorage = new SessionMessageStorage(helper.ViewContext.HttpContext);
		IMessageRenderer messageRenderer = new ToastrMessageRenderer(messageStorage);
		MvcHtmlString result = messageRenderer.Render();
		messageStorage.ClearMessages();
		return result;
	}
}