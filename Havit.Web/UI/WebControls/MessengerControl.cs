﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace Havit.Web.UI.WebControls
{
    /// <summary>
    /// Zobrazuje zprávy Messengera.
    /// </summary>
    public class MessengerControl : Literal
    {
        #region Messenger
        /// <summary>
        /// Messenger použitý pro zprávy k zobrazení.
        /// Není-li nastaven, vrací Messenger.Default.
        /// </summary>
        public Messenger Messenger
        {
            get
            {
                return _messenger == null ? Messenger.Default : _messenger;                
            }
            set
            {
                _messenger = value;
            }
        }
        private Messenger _messenger; 
        #endregion

		#region ShowMessageBox
		/// <summary>
		/// Indikuje, zda se budou zprávy messengeru zobrazovat v message boxu (alert).
		/// </summary>
		public bool ShowMessageBox
		{
			get { return (bool)(ViewState["ShowMessageBox"] ?? false); }
			set { ViewState["ShowMessageBox"] = value; }
		} 
		#endregion

		#region ShowSummary
		/// <summary>
		/// Indikuje, zda se budou zprávy messengeru renderovat do stránky.
		/// </summary>
		public bool ShowSummary
		{
			get { return (bool)(ViewState["ShowSummary"] ?? true); }
			set { ViewState["ShowSummary"] = value; }
		} 
		#endregion

        #region OnPreRender
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

			if (this.ShowSummary)
			{
				this.Text = this.GetSummaryHtml();
			}
			if (this.ShowMessageBox)
			{
				string messageBoxText = this.GetMessageBoxText();
				if (!String.IsNullOrEmpty(messageBoxText))
				{
					//string script = String.Format("alert('{0}');", messageBoxText.Replace("'", "\\'"));
					string script = String.Format("window.setTimeout(function() {{ alert('{0}'); }}, 10);", messageBoxText.Replace("'", "\\'"));
					System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(MessengerControl), "Summary", script, true);
				}
			}

            this.Messenger.Clear();
        }
        #endregion

		#region GetSummaryHtml, AddMessageHtml
		/// <summary>
        /// Vrátí obsah messengeru (HTML kód) připravený k vyrenderování do stránky.
        /// </summary>
		protected virtual string GetSummaryHtml()
        {
            if (Messenger.Messages.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<div class=\"tmessenger\">");
                foreach (MessengerMessage message in Messenger.Messages)
                {
					AddMessageSummaryHtml(message, sb);
                }
                sb.AppendLine("</div>");

                return sb.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Vrátí text zprávy messengeru (HTML kód) připravený k vyrenderování do stránky.
        /// </summary>
        protected virtual void AddMessageSummaryHtml(MessengerMessage message, StringBuilder sb)
        {
            Debug.Assert(message != null);
            Debug.Assert(sb != null);

            string messageCssClass;
            switch (message.MessageType)
            {
                case MessageType.Information:
                    messageCssClass = "tmessageinformation";
                    break;
                case MessageType.Warning:
                    messageCssClass = "tmessagewarning";
                    break;
                case MessageType.Error:
                    messageCssClass = "tmessageerror";
                    break;
                default:
                    throw new InvalidOperationException("Neznámý typ zprávy.");
            }

            sb.Append("<div class=\"");
            sb.Append(messageCssClass);
            sb.Append("\">");

            sb.Append("<span class=\"tmessagetext\">");
            sb.Append(message.Text.Replace("\n", "<br/>\n"));
            sb.Append("</span>");

            sb.Append("</div>");
        }
        #endregion

		#region GetMessageBoxText, AddMessageAlertText
		/// <summary>
		/// Vrátí obsah messengeru (text) připravený k zobrazení v message boxu (alert).
		/// </summary>
		protected virtual string GetMessageBoxText()
		{
			if (Messenger.Messages.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				foreach (MessengerMessage message in Messenger.Messages)
				{
					AddMessageBoxText(message, sb);
				}
				return sb.ToString();
			}
			return String.Empty;
		}

		/// <summary>
		/// Vrátí text zprávy messengeru připravený k zobrazení v message boxu (alert).
		/// </summary>
		protected virtual void AddMessageBoxText(MessengerMessage message, StringBuilder sb)
		{
			sb.Append(message.Text);
			sb.Append("\\n");
		}
		#endregion
	}
}
