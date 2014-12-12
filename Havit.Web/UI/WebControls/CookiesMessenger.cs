using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Messenger - třída pro sběr zpráv (informačních zpráv, upozorněnní a chybových zpráv) k zobrazení v uživatelském rozhraní.
	/// </summary>
	public class CookieMessenger : Messenger
	{
		#region CookieKey
		private const string CookieKey = "MSG";
		#endregion

		#region Messages
		private List<MessengerMessage> Messages
		{
			get
			{
				List<MessengerMessage> _messages = GetMessagesFromCookie();
				if (_messages == null)
				{
					_messages = new List<MessengerMessage>();
					SaveMessagesToCookie(_messages);
				}
				return _messages;
			}
		}
		#endregion

		#region GetMessagesFromCookie
		private List<MessengerMessage> GetMessagesFromCookie()
		{
			List<MessengerMessage> messages = null;

			HttpCookie msgCookie = GetCurrentContext().Request.Cookies[CookieKey];
			if ((msgCookie != null) && (msgCookie.HasKeys))
			{
				messages = new List<MessengerMessage>();

				for (int i = 0; i < msgCookie.Values.Count; i++)
				{
					string[] restoredMessage = msgCookie.Values[i].Split(new char[] { '|' }, 2);
					MessageType msgType = (MessageType)int.Parse(restoredMessage[0]);
					messages.Add(new MessengerMessage(HttpUtility.UrlDecode(restoredMessage[1]), msgType));
				}
			}
			return messages;
		}
		#endregion

		#region SaveMessagesToCookie
		private void SaveMessagesToCookie(List<MessengerMessage> messages)
		{
			HttpCookie msgCookie = new HttpCookie(CookieKey);
			
			if (messages == null || messages.Count == 0)
			{
				msgCookie.Expires = DateTime.Now.AddDays(-2);
				msgCookie.Value = String.Empty;
			}
			else
			{
				int msgIndex = 0;
				foreach (MessengerMessage message in messages)
				{
					String messegaToStore = String.Format("{0}|{1}", ((int)message.MessageType).ToString(), HttpUtility.UrlEncode(message.Text));
					msgCookie.Values.Add(msgIndex.ToString(), messegaToStore);
					msgIndex++;
				}
			}
			GetCurrentContext().Response.Cookies.Set(msgCookie);
		}
		#endregion

		#region AddMessage
		/// <summary>
		/// Přidá zprávu do messengeru.
		/// </summary>
		/// <param name="message">zpráva</param>
		public override void AddMessage(MessengerMessage message)
		{
			Messages.Add(message);
			List<MessengerMessage> messages = Messages;
			messages.Add(message);
			SaveMessagesToCookie(messages);
		}
		#endregion

		#region GetMessages
		/// <summary>
		/// Zprávy k zobrazení.
		/// </summary>
		public override List<MessengerMessage> GetMessages()
		{
			return Messages;
		}
		#endregion

		#region ClearMessages
		/// <summary>
		/// Vyčistí kolekci zpráv.
		/// </summary>
		public override void ClearMessages()
		{
			List<MessengerMessage> messages = Messages;
			messages.Clear();
			SaveMessagesToCookie(messages);
		}
		#endregion

		#region GetCurrentContext
		private static HttpContext GetCurrentContext()
		{
			HttpContext currentContext = HttpContext.Current;
			if (currentContext == null)
			{
				throw new InvalidOperationException("HttpContext.Current je null.");
			}
			return currentContext;
		}
		#endregion
	}

}
