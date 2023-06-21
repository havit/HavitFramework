using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Havit.AspNet.Mvc.Messenger.Renderers;

/// <summary>
/// Defines renderer for rendering messages.
/// </summary>
public interface IMessageRenderer
{
	/// <summary>
	/// Renders messages.
	/// </summary>
	MvcHtmlString Render();
}