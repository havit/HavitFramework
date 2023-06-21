using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Havit.AspNet.Mvc.Messenger;

/// <summary>
/// Defines messenger for showing messages.
/// </summary>
public interface IMessenger
{
	/// <summary>
	/// Adds the message.
	/// </summary>
	/// <param name="text">The text of message.</param>
	/// <param name="messageType">Type of the message.</param>
	void AddMessage(string text, MessageType messageType = MessageType.Success);
}
