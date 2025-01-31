using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Management;
using System.Web.Services.Protocols;

namespace Havit.Web.Management;

/// <summary>
/// Pomocná třída pro oznámení události health monitoringem.
/// </summary>
public class WebRequestErrorEventExt : WebRequestErrorEvent
{
	private const string ExceptionText = "(exception)";

	private readonly HttpContext _currentHttpContext;
	private readonly Type _currentApplicationInstanceType;
	private readonly CultureInfo _currentCulture;
	private readonly CultureInfo _currentUiCulture;

	private readonly string _requestInfo_HttpMethod = String.Empty;
	private readonly string _requestInfo_UrlReferrer = String.Empty;
	private readonly string _requestInfo_UserAgent = String.Empty;
	private readonly string _userInfo_IdentityName = String.Empty;
	private readonly string _userInfo_IsAuthenticated = String.Empty;
	private readonly string _userInfo_AuthenticationType = String.Empty;

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
		if (currentHttpContext != null)
		{
			_requestInfo_HttpMethod = GetValueWithExceptionHandling(() => currentHttpContext.Request?.HttpMethod ?? String.Empty);
			_requestInfo_UrlReferrer = GetValueWithExceptionHandling(() => currentHttpContext.Request?.UrlReferrer?.ToString() ?? String.Empty);
			_requestInfo_UserAgent = GetValueWithExceptionHandling(() => currentHttpContext.Request?.UserAgent ?? String.Empty);
			_userInfo_IdentityName = GetValueWithExceptionHandling(() => currentHttpContext.User?.Identity?.Name ?? String.Empty);
			_userInfo_IsAuthenticated = GetValueWithExceptionHandling(() => currentHttpContext.User?.Identity?.IsAuthenticated.ToString() ?? String.Empty);
			_userInfo_AuthenticationType = GetValueWithExceptionHandling(() => currentHttpContext.User?.Identity?.AuthenticationType ?? String.Empty);

			if (currentHttpContext.ApplicationInstance != null)
			{
				this._currentApplicationInstanceType = currentHttpContext.ApplicationInstance.GetType();
			}
		}

		_currentCulture = CultureInfo.CurrentCulture;
		_currentUiCulture = CultureInfo.CurrentUICulture;
	}

	private static Exception UnwrapException(Exception exception)
	{
		if (((exception is SoapException) || (exception is HttpUnhandledException))
			&& (exception.InnerException != null))
		{
			return exception.InnerException;
		}
		return exception;
	}

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
		FormatException(sb, this.ErrorException);

		// Application information
		sb.AppendLine();
		sb.AppendLine("HttpApplication information: ");
		this.FormatApplicationInformation(sb);

		// Event information
		sb.AppendLine();
		sb.AppendLine("Event information: ");
		this.FormatEventInformation(sb);

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

	/// <summary>
	/// Zapíše do StringBuilderu obecné informace o události
	/// </summary>
	private void FormatEventInformation(StringBuilder sb)
	{
		sb.AppendLine("    Event time: " + this.EventTime.ToString(CultureInfo.InstalledUICulture));
		sb.AppendLine("    Event UTC time: " + this.EventTimeUtc.ToString(CultureInfo.InstalledUICulture));
	}

	/// <summary>
	/// Zapíše do StringBuilderu informace o procesu
	/// </summary>
	private void FormatProcessInformation(StringBuilder sb, WebProcessInformation processInformation)
	{
		sb.AppendLine("    Process ID: " + processInformation.ProcessID.ToString(CultureInfo.InstalledUICulture));
		sb.AppendLine("    Process name: " + processInformation.ProcessName);
		sb.AppendLine("    Machine name: " + System.Environment.MachineName);
		sb.AppendLine("    Account name: " + processInformation.AccountName);
	}

	/// <summary>
	/// Zapíše do StringBuilderu informace o výjimce
	/// </summary>
	private void FormatExceptionInformation(StringBuilder sb, Exception exception)
	{
		sb.AppendLine("    Exception type: " + exception.GetType().ToString());
		sb.AppendLine("    Exception message: " + exception.Message);
	}

	/// <summary>
	/// Zapíše do StringBuilderu informace o requestu
	/// </summary>
	private void FormatRequestInformation(StringBuilder sb, WebRequestInformation requestInformation)
	{
		sb.AppendLine("    Request URL: " + requestInformation.RequestUrl);
		sb.AppendLine("    Request path: " + requestInformation.RequestPath);
		sb.AppendLine("    Request verb: " + _requestInfo_HttpMethod);
		sb.AppendLine("    User host address: " + requestInformation.UserHostAddress);
		sb.AppendLine("    User: " + _userInfo_IdentityName);
		sb.AppendLine("    Is authenticated: " + _userInfo_IsAuthenticated);
		sb.AppendLine("    Authentication type: " + _userInfo_AuthenticationType);
		sb.AppendLine("    Referrer: " + _requestInfo_UrlReferrer);
		sb.AppendLine("    User agent: " + _requestInfo_UserAgent);
	}

	/// <summary>
	/// Zapíše informace o aplikaci
	/// </summary>
	private void FormatApplicationInformation(StringBuilder sb)
	{
		if (_currentApplicationInstanceType != null)
		{
			sb.AppendLine("    Assembly: " + _currentApplicationInstanceType.Assembly.GetName().Name);
			sb.AppendLine("    Assembly Version: " + _currentApplicationInstanceType.Assembly.GetName().Version.ToString());
			sb.AppendLine("    Product Version: " + System.Diagnostics.FileVersionInfo.GetVersionInfo(_currentApplicationInstanceType.Assembly.Location).ProductVersion);
		}
	}

	/// <summary>
	/// Zapíše do StringBuilderu informace o threadu
	/// </summary>
	private void FormatThreadInformation(StringBuilder sb, WebThreadInformation threadInformation)
	{
		sb.AppendLine("    Culture: " + _currentCulture.Name);
		sb.AppendLine("    UI Culture: " + _currentUiCulture.Name);
	}

	/// <summary>
	/// Zapíše do StringBuilderu informace o výjimce.
	/// </summary>
	private void FormatException(StringBuilder sb, Exception exception)
	{
		if (exception != null)
		{
			sb.AppendLine(exception.ToString());
		}
	}

	/// <summary>
	/// Získá hodnotu z dané funkce, pokud dojde k výjimce, vrací ExceptionText.
	/// </summary>
	private string GetValueWithExceptionHandling(Func<string> valueFunc)
	{
		try
		{
			return valueFunc();
		}
		catch
		{
			return ExceptionText;
		}
	}
}
