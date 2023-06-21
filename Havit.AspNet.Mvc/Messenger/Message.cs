using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.AspNet.Mvc.Messenger;

/// <summary>
/// Defines messages what is shown by messenger.
/// </summary>
public class Message
{
	/// <summary>
	/// Gets or sets the text of message.
	/// </summary>
	public string Text { get; set; }

	/// <summary>
	/// Gets or sets the type of the message.
	/// </summary>
	public MessageType MessageType { get; set; }
}
