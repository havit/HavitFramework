using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Havit.Web.UI.WebControls
{
    /// <summary>
    /// Zpráva do messengera.
    /// </summary>
    [Serializable]
    public class MessengerMessage
    {
	    /// <summary>
        /// Text zprávy.
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }
        private string _text;

        /// <summary>
        /// Typ zprávy.
        /// </summary>
        public MessageType MessageType
        {
            get
            {
                return _messageType;
            }
            set
            {
                _messageType = value;
            }
        }
        private MessageType _messageType;

        /// <summary>
        /// Vytvoří instanci zprávy a nastaví typ zprávy na Information.
        /// </summary>
        public MessengerMessage()
        {
            this._messageType = MessageType.Information;
            this._text = String.Empty;
        }

        /// <summary>
        /// Vytvoří instanci zprávy.
        /// </summary>
        /// <param name="text">text zprávy</param>
        /// <param name="messageType">typ zprávy</param>
        public MessengerMessage(string text, MessageType messageType)
        {
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(text), nameof(text));

			this._messageType = messageType;
            this._text = text;
        }
    }

}
