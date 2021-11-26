using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Havit.AspNetCore.ExceptionMonitoring.Formatters
{
	/// <summary>
	/// Formatter výjimky s popisem prostředí http serveru.
	/// </summary>
    public class HttpRequestExceptionFormatter : IExceptionFormatter
    {
        private readonly IHttpContextAccessor httpContextAccessor;

		/// <summary>
		/// Konstruktor.
		/// </summary>
        public HttpRequestExceptionFormatter(IHttpContextAccessor httpContextAccessor = null)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

		/// <summary>
		/// Vrátí text reprezentující lidsky čitelné informace o výjimce (a http requestu a http contextu).
		/// </summary>
        public string FormatException(Exception exception)
        {
            StringBuilder sb = new StringBuilder();

            AppendExceptionInformation(sb, exception);

            if (httpContextAccessor != null)
            {
                HttpContext httpContext = httpContextAccessor.HttpContext;
                AppendRequestInformation(sb, httpContext);
            }

            AppendExceptionDetails(sb, exception);
            AppendExceptionDemistified(sb, exception);
            AppendApplicationInformation(sb);
            AppendEventInformation(sb);

            return sb.ToString();
        }

        private void AppendExceptionInformation(StringBuilder sb, Exception exception)
        {
            sb.AppendLine("Exception information");
            AppendValueLine(sb, "Exception type", () => exception.GetType().ToString());
            AppendValueLine(sb, "Exception message", () => exception.Message);
            sb.AppendLine();
        }

        private void AppendRequestInformation(StringBuilder sb, HttpContext context)
        {
			sb.AppendLine("Request information");
			if (context == null)
			{				
				sb.AppendLine("No request information available.");
			}
			else
			{
				AppendValueLine(sb, "Request URL", () => context.Request.Path);
				AppendValueLine(sb, "Request verb", () => context.Request.Method);
				AppendValueLine(sb, "User host address", () => context.Connection.RemoteIpAddress.ToString());
				AppendValueLine(sb, "Username", () => context.User.Identity.Name);
				AppendValueLine(sb, "IsAuthenticated", () => context.User.Identity.IsAuthenticated.ToString());
				AppendValueLine(sb, "AuthenticationType", () => context.User.Identity.AuthenticationType);
				AppendValueLine(sb, "Client URI", () => context.Request.Headers["hx-client-uri"], skipIfNullOrEmpty: true);
				AppendValueLine(sb, "Referer", () => context.Request.Headers["Referer"]);
				AppendValueLine(sb, "User agent", () => context.Request.Headers["User-Agent"]);
			}
            //AppendValueLine(sb, "Culture", () => context.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name);
            //AppendValueLine(sb, "UI Culture", () => context.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name);
            sb.AppendLine();
        }

        private void AppendExceptionDetails(StringBuilder sb, Exception exception)
        {
            if (exception != null)
            {
                sb.AppendLine("Exception details");
                sb.AppendLine(exception.ToString());
                sb.AppendLine();
            }
        }

        private void AppendExceptionDemistified(StringBuilder sb, Exception exception)
        {
            if (exception != null)
            {
                string demistifiedException;

                try
                {
                    demistifiedException = exception.Demystify().ToString();
                }
                catch
                {
                    return;
                }

                sb.AppendLine("Exception details (with Ben.Demystifier)");
                sb.AppendLine(demistifiedException);
                sb.AppendLine();
            }
        }

        private void AppendApplicationInformation(StringBuilder sb)
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            if (assembly != null)
            {
                sb.AppendLine("Application information");
                AppendValueLine(sb, "Assembly", () => assembly.GetName().Name);
                AppendValueLine(sb, "Assembly Version", () => assembly.GetName().Version.ToString());
                AppendValueLine(sb, "Product Version", () => System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion);
                sb.AppendLine();
            }
        }

        private void AppendEventInformation(StringBuilder sb)
        {
            DateTimeOffset now = DateTimeOffset.Now;

            sb.AppendLine("Event information");
            AppendValueLine(sb, "Event time", () => now.ToString(CultureInfo.InstalledUICulture));
            AppendValueLine(sb, "Event UTC time", () => now.ToUniversalTime().ToString(CultureInfo.InstalledUICulture));
            sb.AppendLine();
        }

        private void AppendValueLine(StringBuilder sb, string key, Func<string> getValueFunc, bool skipIfNullOrEmpty = false)
        {
            string value;
            try
            {
                value = getValueFunc.Invoke();
            }
            catch
            {
                value = "(exception)";
            }
			if (skipIfNullOrEmpty && String.IsNullOrEmpty(value))
			{
				return;
			}
            sb.AppendLine("    " + key + ": " + value);
        }
    }
}