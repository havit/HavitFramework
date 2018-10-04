using Havit.AspNet.Mvc.Messenger.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Havit.Web;

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
		/// Renders messages. Performs replace of new line to HTML breakline and carriage return to empty string. Encodes message to HTML entities.
		/// </summary>
		public MvcHtmlString Render()
		{
			StringBuilder sb = new StringBuilder();
			List<Message> messages = messageStorage.GetMessages();
			foreach (Message message in messages)
			{
				string toasterMessage = message.Text.TrimEnd().Replace("\n", "<br />").Replace("\r", "");
				string encodedMessage = HttpUtilityExt.HtmlEncode(toasterMessage);
				sb.AppendFormat("toastr.{0}(\"{1}\");", message.MessageType.ToString().ToLower(), encodedMessage);
				sb.AppendLine();
			}

			TagBuilder scriptTagBuilder = new TagBuilder("script");
			scriptTagBuilder.Attributes["type"] = "text/javascript";
			scriptTagBuilder.InnerHtml = sb.ToString();

			return new MvcHtmlString(scriptTagBuilder.ToString());
		}
	}
}