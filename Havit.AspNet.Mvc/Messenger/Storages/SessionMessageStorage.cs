using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Havit.AspNet.Mvc.Messenger.Storages
{
	/// <summary>
	/// TempData message store.
	/// </summary>
	public class SessionMessageStorage : IMessageStorage
	{
		internal const string StorageKey = "TempDataMessageStorage";
		private readonly HttpContextBase httpContext;

		/// <summary>
		/// Gets the messages from storage (TempData of Session).
		/// </summary>
		private List<Message> Messages
		{
			get
			{
				List<Message> messages = (List<Message>)httpContext.Session[StorageKey];
				if (messages == null)
				{
					messages = new List<Message>();
					httpContext.Session[StorageKey] = messages;
				}
				return messages;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SessionMessageStorage"/> class.
		/// </summary>
		public SessionMessageStorage(HttpContextBase httpContext)
		{
			this.httpContext = httpContext;
		}

		/// <summary>
		/// Adds the message into storage.
		/// </summary>
		/// <param name="message">The message.</param>
		public void AddMessage(Message message)
		{
			Messages.Add(message);
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
			Messages.Clear();
		}
	}
}