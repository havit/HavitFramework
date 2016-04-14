using Havit.AspNet.Mvc.Messenger.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Havit.AspNet.Mvc.Messenger.Renderers
{
	/// <summary>
	/// Renderer for messages in Toastr.
	/// </summary>
	public class ToastrMessageRenderer : IMessageRenderer
	{
		private readonly IMessageStorage messageStorage;

		/// <summary>
		/// Initializes a new instance of the <see cref="ToastrMessageRenderer"/> class.
		/// </summary>
		/// <param name="messageStorage">The message storage.</param>
		public ToastrMessageRenderer(IMessageStorage messageStorage)
		{
			this.messageStorage = messageStorage;
		}

		/// <summary>
		/// Renders messages.
		/// </summary>
		public MvcHtmlString Render()
		{
			StringBuilder sb = new StringBuilder();
			foreach (Message message in messageStorage.GetMessages())
			{
				string toasterMessage = message.Text.TrimEnd().Replace("'", "\\'").Replace("\n", "<br />").Replace("\r", "");
				sb.AppendFormat("toastr.{0}(\"{1}\");", message.MessageType.ToString().ToLower(), toasterMessage);
				sb.AppendLine();
			}
			return new MvcHtmlString(String.Format("<script type=\"text/javascript\">\n{0}</script>", sb.ToString()));
		}
	}
}