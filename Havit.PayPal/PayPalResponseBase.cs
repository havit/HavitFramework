using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Havit.PayPal
{
	/// <summary>
	/// Abstraktní třída pro strong-type reprezentaci odpovědí z PayPal.
	/// </summary>
	public abstract class PayPalResponseBase
	{
		/// <summary>
		/// Token PayPal transkakce.
		/// </summary>
		public string Token
		{
			get { return _token; }
			protected set { _token = value; }
		}
		private string _token;

		/// <summary>
		/// Čas a datum odpovědi.
		/// </summary>
		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}
		private DateTime _timestamp;

		/// <summary>
		/// Verze API.
		/// </summary>
		public string Version
		{
			get { return _version; }
			set { _version = value; }
		}
		private string _version;

		/// <summary>
		/// Označení buildu.
		/// </summary>
		public string Build
		{
			get { return _build; }
			set { _build = value; }
		}
		private string _build;

		/// <summary>
		/// Kód chyby.
		/// </summary>
		public string ErrorCode
		{
			get { return _errorCode; }
			set { _errorCode = value; }
		}
		private string _errorCode;

		/// <summary>
		/// Krátka zpráva chyby.
		/// </summary>
		public string ErrorShortMessage
		{
			get { return _errorShortMessage; }
			set { _errorShortMessage = value; }
		}
		private string _errorShortMessage;

		/// <summary>
		/// Dlouhá zpráva chyby.
		/// </summary>
		public string ErrorLongMessage
		{
			get { return _errorLongMessage; }
			set { _errorLongMessage = value; }
		}
		private string _errorLongMessage;

		/// <summary>
		/// Kód severity.
		/// </summary>
		public string SeverityCode
		{
			get { return _severityCode; }
			set { _severityCode = value; }
		}
		private string _severityCode;

		/// <summary>
		/// Odpověď popisující úspěšnost volání API Success/SuccessWithWarning/PartialSuccess/Failure/FailureWithWarning/Warning.
		/// </summary>
		public string Acknowledgment
		{
			get { return _acknowledgement; }
			protected set { _acknowledgement = value; }
		}
		private string _acknowledgement;

		/// <summary>
		/// Debugging token.
		/// </summary>
		public string CorrelationID
		{
			get { return _correlationID; }
			protected set { _correlationID = value; }
		}
		private string _correlationID;

		/// <summary>
		/// Indikuje, zda-li je odpověď úspěšná (Success/SuccessWithWarning).
		/// </summary>
		public bool IsSuccess
		{
			get { return _isSuccess; }
			protected set { _isSuccess = value; }
		}
		private bool _isSuccess;

		/// <summary>
		/// Initializes a new instance of the PayPalRawResponse class.
		/// </summary>
		/// <param name="rawResponseData">The response data, raw.</param>
		protected PayPalResponseBase(NameValueCollection rawResponseData)
		{
			ParseResponseData(rawResponseData);
		}

		/// <summary>
		/// Rozparsuje data do strong-type properties.
		/// </summary>
		/// <param name="rawResponseData">data požadavku</param>
		protected virtual void ParseResponseData(NameValueCollection rawResponseData)
		{
			this.Token = rawResponseData["TOKEN"];
			this.CorrelationID = rawResponseData["CORRELATIONID"];
			this.Build = rawResponseData["BUILD"];
			this.Version = rawResponseData["VERSION"];

			if (!String.IsNullOrEmpty(rawResponseData["TIMESTAMP"]))
			{
				DateTime timeStamp;
				if (DateTime.TryParse(rawResponseData["TIMESTAMP"], out timeStamp))
				{
					this.Timestamp = timeStamp;
				}
			}

			this.IsSuccess = false;

			if (!String.IsNullOrEmpty(rawResponseData["ACK"]))
			{
				this.Acknowledgment = rawResponseData["ACK"];
				
				if ((this.Acknowledgment == "Success") || (this.Acknowledgment == "SuccessWithWarning"))
				{
					this.IsSuccess = true;
				}				
			}

			if (!String.IsNullOrEmpty(rawResponseData["ERRORCODE0"]))
			{
				this.ErrorCode = rawResponseData["ERRORCODE0"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["L_SHORTMESSAGE0"]))
			{
				this.ErrorShortMessage = rawResponseData["L_SHORTMESSAGE0"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["L_LONGMESSAGE0"]))
			{
				this.ErrorLongMessage = rawResponseData["L_LONGMESSAGE0"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["L_SEVERITYCODE0"]))
			{
				this.SeverityCode = rawResponseData["L_SEVERITYCODE0"];			
			}			
			
		}
	}
}
