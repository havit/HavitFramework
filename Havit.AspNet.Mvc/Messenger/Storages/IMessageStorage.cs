using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.AspNet.Mvc.Messenger.Storages
{
	/// <summary>
	/// Defines storage for messages.
	/// </summary>
	public interface IMessageStorage
	{
		/// <summary>
		/// Adds the message into storage.
		/// </summary>
		/// <param name="message">The message.</param>
		void AddMessage(Message message);

		/// <summary>
		/// Gets the messages.
		/// </summary>
		List<Message> GetMessages();

		/// <summary>
		/// Clears the messages.
		/// </summary>
		void ClearMessages();
	}
}
