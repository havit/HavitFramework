using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI.WebControls
{
    /// <summary>
    /// Zpráva do messengera.
    /// </summary>
    [Serializable]
    public class MessengerMessage
    {
        #region Text
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
        #endregion

        #region MessageType
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
        #endregion

        #region Constructors
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
            if (String.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Parametr nesmí být null ani String.Empty", "messageType");
            }
            this._messageType = messageType;
            this._text = text;
        }
        #endregion
    }

}
