using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Havit.Web.UI.WebControls
{
    /// <summary>
    /// Messenger - třída pro sběr zpráv (informačních zpráv, upozorněnní a chybových zpráv) k zobrazení v uživatelském rozhraní.
    /// </summary>
    public class Messenger
    {
        #region Messages (private)
        /// <summary>
        /// Zprávy k zobrazení.
        /// </summary>
        public List<MessengerMessage> Messages
        {
            get
            {
                HttpContext currentContext = HttpContext.Current;
                if (currentContext == null)
                {
                    throw new InvalidOperationException("HttpContext.Current je null.");
                }

                List<MessengerMessage> result = (List<MessengerMessage>)currentContext.Session["Messenger_Messages"];
                if (result == null)
                {
                    result = new List<MessengerMessage>();
                    currentContext.Session["Messenger_Messages"] = result;
                }

                return result;
            }
        }
        #endregion

        #region AddMessage
        /// <summary>
        /// Přidá zprávu do messengeru.
        /// </summary>
		/// <param name="message">zpráva</param>
        public void AddMessage(MessengerMessage message)
        {
            Messages.Add(message);
        }

        /// <summary>
        /// Přidá zprávu do messengeru.
        /// </summary>
        /// <param name="messageType">typ zprávy (information, error, ...)</param>
        /// <param name="text">text zprávy</param>
        public void AddMessage(MessageType messageType, string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Parametr nesmí být null ani String.Empty", "text");
            }
            AddMessage(new MessengerMessage(text, messageType));
        }

        /// <summary>
        /// Přidá zprávu pomocí String.Format();
        /// </summary>
        /// <param name="messageType">typ zprávy (information, error, ...)</param>
        /// <param name="format">formátovací řetězec pro String.Format()</param>
        /// <param name="args">argumenty pro String.Format()</param>
        public void AddMessage(MessageType messageType, string format, params object[] args)
        {
            AddMessage(messageType, String.Format(format, args));
        }

        /// <summary>
        /// Přidá zprávu typu Information pomocí String.Format();
        /// </summary>
        /// <param name="format">formátovací řetězec pro String.Format()</param>
        /// <param name="args">argumenty pro String.Format()</param>
        public void AddMessage(string format, params object[] args)
        {
            AddMessage(MessageType.Information, String.Format(format, args));
        }

        /// <summary>
        /// Prosté přidání zprávy typu Information.
        /// </summary>
        /// <param name="message">zpráva</param>
        public void AddMessage(string message)
        {
            AddMessage(MessageType.Information, message);
        }
        #endregion

        #region AddGlobalResourceMessage
        /// <summary>
        /// Přidá zprávu z App_GlobalResources.
        /// </summary>
		/// <param name="messageType">Typ zprávy.</param>
		/// <param name="classKey">název global-resource souboru</param>
        /// <param name="resourceKey">klíč resourcu</param>
        public void AddGlobalResourceMessage(MessageType messageType, string classKey, string resourceKey)
        {
            AddMessage(messageType, (string)HttpContext.GetGlobalResourceObject(classKey, resourceKey));
        }

        /// <summary>
        /// Přidá zprávu z App_GlobalResources typu Information.
        /// </summary>
        /// <param name="classKey">název global-resource souboru</param>
        /// <param name="resourceKey">klíč resourcu</param>
        public void AddGlobalResourceMessage(string classKey, string resourceKey)
        {
            AddGlobalResourceMessage(MessageType.Information, classKey, resourceKey);
        }
        #endregion

        #region Clear
        /// <summary>
        /// Vyčistí kolekci zpráv.
        /// </summary>
        public void Clear()
        {
            Messages.Clear();
        }
        #endregion

        #region Default
        /// <summary>
        /// Výchozí instance messengera.
        /// </summary>
        public static Messenger Default
        {
            get
            {
                return _default;
            }
        }
        private static Messenger _default = new Messenger(); 
        #endregion

    }
}
