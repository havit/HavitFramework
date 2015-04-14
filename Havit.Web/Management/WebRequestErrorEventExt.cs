using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Management;
using System.Web.Services.Protocols;

namespace Havit.Web.Management
{
	/// <summary>
	/// Pomocná třída pro oznámení události health monitoringem.
	/// </summary>
	public class WebRequestErrorEventExt : WebRequestErrorEvent
	{
		#region Constants (private)
		private const string ExceptionText = "(exception)";
		#endregion

		#region Private fields
		private HttpContext _currentHttpContext;
		private Type _currentApplicationInstanceType;
		private CultureInfo _currentCulture;
		private CultureInfo _currentUiCulture;
		#endregion

		#region Constructors
		/// <summary>
		/// Konstruktor.
		/// </summary>
		[Obsolete]		
		public WebRequestErrorEventExt(string message, object eventSource, Exception exception) : this(message, eventSource, exception, null)
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public WebRequestErrorEventExt(string message, object eventSource, Exception exception, HttpContext currentHttpContext)
			: base(message, eventSource, WebEventCodes.WebExtendedBase + 999, UnwrapException(exception))
		{
			this._currentHttpContext = currentHttpContext;
			if ((currentHttpContext != null) && (currentHttpContext.ApplicationInstance != null))
			{
				this._currentApplicationInstanceType = currentHttpContext.ApplicationInstance.GetType();
			}
			_currentCulture = CultureInfo.CurrentCulture;
			_currentUiCulture = CultureInfo.CurrentUICulture;
		}
		#endregion

		#region UnwrapException
		private static Exception UnwrapException(Exception exception)
		{
			if (((exception is SoapException) || (exception is HttpUnhandledException))
				&& (exception.InnerException != null))
			{
				return exception.InnerException;
			}
			return exception;
		}
		#endregion

		#region ToString
		/// <summary>
		/// Vrátí log události
		/// </summary>
		public override string ToString(bool includeAppInfo, bool includeCustomEventDetails)
		{
			StringBuilder sb = new StringBuilder();

			// Exception information
			sb.AppendLine("Exception information: ");
			FormatExceptionInformation(sb, this.ErrorException);

			// Request information
			sb.AppendLine();
			sb.AppendLine("Request information: ");
			FormatRequestInformation(sb, this.RequestInformation);

			// Exception
			sb.AppendLine();
			sb.AppendLine("Exception: ");
			FormatExceptionInformation(sb);

			// Application information
			sb.AppendLine();
			sb.AppendLine("Application information: ");
			this.FormatApplicationInformation(sb);

			// Event information
			sb.AppendLine();
			sb.AppendLine("Event information: ");
			this.FormatEventInformation(sb, includeAppInfo);

			// Process information
			sb.AppendLine();
			sb.AppendLine("Process information: ");
			FormatProcessInformation(sb, this.ProcessInformation);

			// Thread information
			sb.AppendLine();
			sb.AppendLine("Thread information: ");
			FormatThreadInformation(sb, this.ThreadInformation);
			
			return sb.ToString();
		}

		#endregion

		#region FormatToString
		/// <summary>
		/// Zapíše do StringBuilderu obecné informace o události
		/// </summary>
		private void FormatEventInformation(StringBuilder sb, bool includeAppInfo)
		{
			//sb.AppendLine("    Event code: " + this.EventCode.ToString(CultureInfo.InstalledUICulture));
			//sb.AppendLine("    Event message: " + this.Message);
			//sb.AppendLine("    Event time: " + this.EventTime.ToString(CultureInfo.InstalledUICulture));
			//sb.AppendLine("    Event time(UTC): " + this.EventTimeUtc.ToString(CultureInfo.InstalledUICulture));
			//sb.AppendLine("    Event ID: " + this.EventID.ToString("N", CultureInfo.InstalledUICulture));
			//sb.Append(" Event sequence: " + this.EventSequence.ToString(CultureInfo.InstalledUICulture));
			//sb.Append(" Event occurence: " + this.EventOccurrence.ToString(CultureInfo.InstalledUICulture));
			//sb.AppendLine(" Event detail code: " + this.EventDetailCode.ToString(CultureInfo.InstalledUICulture));

			//sb.Append("    Event ID: " + this.EventID.ToString("N", CultureInfo.InstalledUICulture));
			//sb.Append(", Event sequence: " + this.EventSequence.ToString(CultureInfo.InstalledUICulture));
			//sb.AppendLine(", Event occurence: " + this.EventOccurrence.ToString(CultureInfo.InstalledUICulture));
			sb.AppendLine("    Event time: " + this.EventTime.ToString(CultureInfo.InstalledUICulture));
			sb.AppendLine("    Event UTC time: " + this.EventTimeUtc.ToString(CultureInfo.InstalledUICulture));
		} 
		#endregion

		#region FormatProcessInformation
		/// <summary>
		/// Zapíše do StringBuilderu informace o procesu
		/// </summary>
		private void FormatProcessInformation(StringBuilder sb, WebProcessInformation processInformation)
		{
			sb.AppendLine("    Process ID: " + processInformation.ProcessID.ToString(CultureInfo.InstalledUICulture));
			sb.AppendLine("    Process name: " + processInformation.ProcessName);
			sb.AppendLine("    Account name: " + processInformation.AccountName);
		} 
		#endregion

		#region FormatExceptionInformation
		/// <summary>
		/// Zapíše do StringBuilderu informace o výjimce
		/// </summary>
		private void FormatExceptionInformation(StringBuilder sb, Exception exception)
		{
			sb.AppendLine("    Exception type: " + exception.GetType().ToString());
			sb.AppendLine("    Exception message: " + exception.Message);
		}
		#endregion

		#region FormatRequestInformation
		/// <summary>
		/// Zapíše do StringBuilderu informace o requestu
		/// </summary>
		private void FormatRequestInformation(StringBuilder sb, WebRequestInformation requestInformation)
		{
			sb.AppendLine("    Request URL: " + requestInformation.RequestUrl);
			sb.AppendLine("    Request path: " + requestInformation.RequestPath);
			sb.AppendLine("    User host address: " + requestInformation.UserHostAddress);

			string userName;
			try
			{
				userName = requestInformation.Principal.Identity.Name;
			}
			catch
			{
				userName = ExceptionText;
			}
			sb.AppendLine("    User: " + userName);

			string isAuthenticated;
			try
			{
				isAuthenticated = requestInformation.Principal.Identity.IsAuthenticated.ToString();
			}
			catch
			{
				isAuthenticated = ExceptionText;
			}
			sb.AppendLine("    Is authenticated: " + isAuthenticated);

			string authenticationType;
			try
			{
				authenticationType = requestInformation.Principal.Identity.AuthenticationType;
			}
			catch
			{
				authenticationType = ExceptionText;
			}
			sb.AppendLine("    Authentication type: " + authenticationType);

			if (_currentHttpContext != null)
			{
				string urlReferrer;

				try
				{
					urlReferrer = _currentHttpContext.Request.UrlReferrer.ToString();
				}
				catch
				{
					urlReferrer = ExceptionText;
				}
				sb.AppendLine("    Referrer: " + urlReferrer);

				string userAgent;
				try
				{
					userAgent = _currentHttpContext.Request.UserAgent;
				}
				catch
				{
					userAgent = ExceptionText;
				}
				sb.AppendLine("    User agent: " + userAgent);				
			}
		}
		#endregion

		#region FormatApplicationInformation
		/// <summary>
		/// Zapíše informace o aplikaci
		/// </summary>
		private void FormatApplicationInformation(StringBuilder sb)
		{
			if (_currentApplicationInstanceType != null)
			{
				sb.AppendLine("    HttpApplication Assembly: " + _currentApplicationInstanceType.Assembly.FullName);
			}
		}
		#endregion

		#region FormatThreadInformation
		/// <summary>
		/// Zapíše do StringBuilderu informace o threadu
		/// </summary>
		private void FormatThreadInformation(StringBuilder sb, WebThreadInformation threadInformation)
		{
			sb.AppendLine("    Thread ID: " + threadInformation.ThreadID);
			sb.AppendLine("    Thread account name: " + threadInformation.ThreadAccountName);
			sb.AppendLine("    Is impersonating: " + threadInformation.IsImpersonating);
			sb.AppendLine("    Culture: " + _currentCulture.Name);
			sb.AppendLine("    UI Culture: " + _currentUiCulture.Name);
		}
		#endregion

		#region FormatExceptionInformation
		/// <summary>
		/// Zapíše do StringBuilderu informace o výjimce.
		/// </summary>
		private void FormatExceptionInformation(StringBuilder sb)
		{
			if (ErrorException != null)
			{
				sb.AppendLine(this.ErrorException.ToString());
			}
		}
		#endregion
	}
}
