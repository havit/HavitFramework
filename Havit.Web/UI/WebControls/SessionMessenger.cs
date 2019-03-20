using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Messenger - třída pro sběr zpráv (informačních zpráv, upozorněnní a chybových zpráv) k zobrazení v uživatelském rozhraní.
	/// </summary>
	internal class SessionMessenger : Messenger
	{
		private const string SessionKey = "Messenger_Messages";

		private List<MessengerMessage> Messages
		{
			get
			{
				List<MessengerMessage> messages = (List<MessengerMessage>)GetCurrentContext().Session[SessionKey];			
				if (messages == null)
				{
					messages = new List<MessengerMessage>();
					GetCurrentContext().Session[SessionKey] = messages;
				}
				return messages;
			}
		}

		/// <summary>
		/// Přidá zprávu do messengeru.
		/// </summary>
		/// <param name="message">zpráva</param>
		public override void AddMessage(MessengerMessage message)
		{
			Messages.Add(message);
		}

		/// <summary>
		/// Zprávy k zobrazení.
		/// </summary>
		public override List<MessengerMessage> GetMessages()
		{
			return new List<MessengerMessage>(Messages);
		}

		/// <summary>
		/// Vyčistí kolekci zpráv.
		/// </summary>
		public override void Clear()
		{
			Messages.Clear();
		}

		private static HttpContext GetCurrentContext()
		{
			HttpContext currentContext = HttpContext.Current;
			if (currentContext == null)
			{
				throw new InvalidOperationException("HttpContext.Current je null.");
			}
			if (currentContext.Session == null)
			{
				throw new InvalidOperationException("HttpContext.Current.Session je null.");
			}
			return currentContext;
		}
	}
}