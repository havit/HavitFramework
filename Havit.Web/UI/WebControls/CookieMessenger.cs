using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Messenger - třída pro sběr zpráv (informačních zpráv, upozorněnní a chybových zpráv) k zobrazení v uživatelském rozhraní.
/// </summary>
internal class CookieMessenger : Messenger
{
	private const string CookieKey = "Messenger";

	private List<MessengerMessage> GetMessagesFromCookie()
	{
		List<MessengerMessage> messages = new List<MessengerMessage>();

		HttpCookie msgCookie = GetCurrentContext().Request.Cookies[CookieKey];
		if ((msgCookie != null) && msgCookie.HasKeys)
		{
			for (int i = 0; i < msgCookie.Values.Count; i++)
			{
				string[] restoredMessage = msgCookie.Values[i].Split(new char[] { '|' }, 2);
				MessageType msgType = (MessageType)int.Parse(restoredMessage[0]);
				messages.Add(new MessengerMessage(HttpUtility.UrlDecode(restoredMessage[1]), msgType));
			}
		}
		return messages;
	}

	private void SaveMessagesToCookie(List<MessengerMessage> messages)
	{
		HttpContext currentContext = GetCurrentContext();
		HttpCookie msgCookie = new HttpCookie(CookieKey)
		{
			HttpOnly = true, // přístup přes klientský skript netřeba, tj. pro security HttpOnly
			SameSite = SameSiteMode.Strict, // není důvod, proč by měly cookies přinášet hodnotu při příchodu odjinud
			Secure = currentContext.Request.IsSecureConnection // běží-li aplikace přes HTTPS, nechť jsou i tyto cookie nastaveny jako secure
		};

		if (messages == null || !messages.Any())
		{
			msgCookie.Expires = DateTime.Now.AddYears(-1);
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
		currentContext.Response.Cookies.Set(msgCookie);
	}

	/// <summary>
	/// Přidá zprávu do messengeru.
	/// </summary>
	/// <param name="message">zpráva</param>
	public override void AddMessage(MessengerMessage message)
	{
		List<MessengerMessage> messages = GetMessagesFromCookie();
		messages.Add(message);
		SaveMessagesToCookie(messages);
	}

	/// <summary>
	/// Zprávy k zobrazení.
	/// </summary>
	public override List<MessengerMessage> GetMessages()
	{
		return GetMessagesFromCookie();
	}

	/// <summary>
	/// Vyčistí kolekci zpráv.
	/// </summary>
	public override void Clear()
	{
		SaveMessagesToCookie(new List<MessengerMessage>());
	}

	private static HttpContext GetCurrentContext()
	{
		HttpContext currentContext = HttpContext.Current;
		if (currentContext == null)
		{
			throw new InvalidOperationException("HttpContext.Current je null.");
		}
		return currentContext;
	}
}
