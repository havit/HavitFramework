using Havit.AspNet.Mvc.Messenger.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Havit.AspNet.Mvc.Messenger
{
	/// <summary>
	/// Toastr messenger.
	/// </summary>
	public class ToastrMessenger : IMessenger
	{
		private readonly IMessageStorage messageStorage;

		/// <summary>
		/// Initializes a new instance of the <see cref="ToastrMessenger"/> class.
		/// </summary>
		/// <param name="messageStorage">The message storage.</param>
		public ToastrMessenger(IMessageStorage messageStorage)
		{
			this.messageStorage = messageStorage;
		}

		/// <summary>
		/// Adds the message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="messageType">Type of the message.</param>
		public void AddMessage(string message, MessageType messageType)
		{
			messageStorage.AddMessage(new Message { Text = message, MessageType = messageType });
		}
	}
}