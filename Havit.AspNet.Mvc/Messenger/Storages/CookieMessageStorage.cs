using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Havit.AspNet.Mvc.Messenger.Storages
{
	/// <summary>
	/// Storage for messages provided by cookies.
	/// </summary>
	public class CookieMessageStorage : IMessageStorage
	{
		internal const string StorageKey = "CookieMessageStorage";
		private readonly HttpContextBase httpContext;

		/// <summary>
		/// Gets the messages stored in cookie.
		/// </summary>
		private List<Message> Messages
		{
			get
			{
				List<Message> messages = new List<Message>();

				HttpCookie cookie = this.httpContext.Request.Cookies[StorageKey];
				if (cookie != null && cookie.HasKeys)
				{
					for (int i = 0; i < cookie.Values.Count; i++)
					{
						string[] restoredMessage = cookie.Values[i].Split(new char[] { '|' }, 2);
						MessageType messageType = (MessageType)int.Parse(restoredMessage[0]);
						string messageText = HttpUtility.UrlDecode(restoredMessage[1]);
						messages.Add(new Message { MessageType = messageType, Text = messageText });
					}
				}
				return messages;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CookieMessageStorage"/> class.
		/// </summary>
		public CookieMessageStorage(HttpContextBase httpContext)
		{
			this.httpContext = httpContext;
		}

		/// <summary>
		/// Adds the message into storage.
		/// </summary>
		/// <param name="message">The message.</param>
		public void AddMessage(Message message)
		{
			List<Message> messages = Messages;
			messages.Add(message);
			SaveMessagesToCookie(messages);
		}

		/// <summary>
		/// Gets the messages.
		/// </summary>
		public List<Message> GetMessages()
		{
			return new List<Message>(Messages);
		}

		/// <summary>
		/// Clears the messages.
		/// </summary>
		public void ClearMessages()
		{
			SaveMessagesToCookie(new List<Message>());
		}

		private void SaveMessagesToCookie(List<Message> messages)
		{
			HttpCookie messageCookie = new HttpCookie(StorageKey);
			//přístup přes klientský skript netřeba, tj. pro security HttpOnly
			messageCookie.HttpOnly = true;
			messageCookie.SameSite = SameSiteMode.Strict; // není důvod, proč by měly cookies přinášet hodnotu při příchodu odjinud

			if ((messages == null) || (messages.Count == 0))
			{
				messageCookie.Expires = DateTime.Now.AddYears(-1);
				messageCookie.Value = String.Empty;
			}
			else
			{
				int messageIndex = 0;
				foreach (Message message in messages)
				{
					string messageToStore = String.Format("{0}|{1}", ((int)message.MessageType).ToString(), HttpUtility.UrlEncode(message.Text));
					messageCookie.Values.Add(messageIndex.ToString(), messageToStore);
					messageIndex++;
				}
			}
			this.httpContext.Response.Cookies.Set(messageCookie);
		}
	}
}