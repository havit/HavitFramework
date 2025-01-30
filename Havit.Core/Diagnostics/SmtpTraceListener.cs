﻿using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace Havit.Diagnostics;

/// <summary>
/// TraceListener that sends output via email.
/// Configuration is done through initializeData in the constructor, supported values include:
/// <list type="bullet">
///		<item>
///			<term>subject</term>
///			<description>Subject of the email being sent.</description>
///		</item>
///		<item>
///			<term>to</term>
///			<description>Recipient of the email.</description>
///		</item>
///		<item>
///			<term>from</term>
///			<description>Sender of the email.</description>
///		</item>
///		<item>
///			<term>smtpServer</term>
///			<description>SMTP server for sending the message.</description>
///		</item>
///		<item>
///			<term>smtpPort</term>
///			<description>Port to be used for communication with the SMTP server (if not set, it won't be used).</description>
///		</item>
///		<item>
///			<term>smtpUsername</term>
///			<description>Username for the SMTP server (if not set, it won't be used).</description>
///		</item>
///		<item>
///			<term>smtpPassword</term>
///			<description>Password for the SMTP server.</description>
///		</item>
///		<item>
///			<term>smtpEnableSsl</term>
///			<description>Indicates whether to use SSL when connecting to the SMTP server. The default value is false.</description>
///		</item>
/// </list>
/// </summary>
/// <remarks>
/// Inspired by the implementation of System.Diagnostics.XmlWriterListener.
/// </remarks>
public class SmtpTraceListener : TraceListener
{
	private readonly string _to;
	private readonly string _from;
	private readonly string _subject;
	private readonly string _smtpServer;
	private readonly int? _smtpPort = null;
	private readonly string _smtpUsername;
	private readonly string _smtpPassword;
	private readonly bool? _smtpEnableSsl = false;

	/// <summary>
	/// Constructor that is called when using the TraceListener from the app.config and the value of the initializeData attribute is passed to it.
	/// </summary>
	/// <param name="initializeData">value of the initializeData attribute from app.config</param>
	public SmtpTraceListener(string initializeData)
	{
		if (initializeData == null)
		{
			return; // defaults will be used
		}

		foreach (string arg in initializeData.Split(';'))
		{
			string[] paramValue = arg.Split('=');
			if (paramValue.Length >= 2)
			{
				string parameterName = paramValue[0].Trim().ToLower();
				string parameterValue = paramValue[1].Trim();
				switch (parameterName)
				{
					case "from":
						_from = parameterValue;
						break;
					case "to":
						_to = parameterValue;
						break;
					case "subject":
						_subject = parameterValue;
						break;
					case "smtpserver":
						_smtpServer = parameterValue;
						break;
					case "smtpport":
						_smtpPort = Int32.Parse(parameterValue);
						break;
					case "smtpusername":
						_smtpUsername = parameterValue;
						break;
					case "smtppassword":
						_smtpPassword = parameterValue;
						break;
					case "smtpenablessl":
						_smtpEnableSsl = Boolean.Parse(parameterValue);
						break;
					default:
						throw new ArgumentException(String.Format("Unknown parameter '{0}' in the SmtpTraceListener configuration in initializeData.", paramValue[0]/* not providing parameterName because I want to display it without performing Trim and ToLower */));
				}
			}
		}

		if (String.IsNullOrEmpty(_smtpServer))
		{
			if (!String.IsNullOrEmpty(_smtpUsername) || !String.IsNullOrEmpty(_smtpPassword))
			{
				throw new ArgumentException("Credentials can be set only when smtp server (host) is specified.");
			}

			if (_smtpPort != null)
			{
				throw new ArgumentException("Smtp port can be set only when smtp server (host) is specified.");
			}

			if (_smtpEnableSsl != false)
			{
				throw new ArgumentException("Smtp ssl settings can be set only when smtp server (host) is specified.");
			}
		}
	}

	/// <summary>
	/// Internal implementation of sending an email.
	/// </summary>
	/// <param name="message">trace message</param>
	private void SendMessage(string message)
	{
		if (String.IsNullOrEmpty(_to))
		{
			return;
		}

		try
		{
			MailMessage mailMessage = GetMailMessage(message);
			SmtpClient smtpClient = GetSmtpClient();
			smtpClient.Send(mailMessage);
		}
		catch
		{
			// NOOP - we don't want a malfunctioning trace-mailing to kill the server
#if DEBUG
			// but during debugging, we are interested in it
			throw;
#endif
		}

		// http://www.codeproject.com/KB/trace/smtptracelistenerarticle.aspx
		// In the SMTPTraceListener Write method - I call the Flush method.  This forces the e-mail output to happen right then, and makes the component more stable.
		// With the Flush taken out of the Write method, I was experiencing some inconsistent behavior - i.e. exceptions thrown sometimes but not always...
		// goofy problem perhaps someone knows why?
		this.Flush();
	}

	/// <summary>
	/// Returns a fully configured instance of SmtpClient for sending an email.
	/// </summary>
	protected internal virtual SmtpClient GetSmtpClient()
	{
		SmtpClient smtpClient = new SmtpClient();
		if (!String.IsNullOrEmpty(_smtpServer))
		{
			smtpClient.Host = _smtpServer;

			smtpClient.Credentials = (!String.IsNullOrEmpty(_smtpUsername))
				? new NetworkCredential(_smtpUsername, _smtpPassword)
				: new NetworkCredential("", "");

			smtpClient.Port = _smtpPort.GetValueOrDefault(25);
			smtpClient.EnableSsl = _smtpEnableSsl.GetValueOrDefault(false);

			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
		}

		return smtpClient;
	}

	/// <summary>
	/// Returns a fully configured MailMessage ready for sending.
	/// </summary>
	protected virtual MailMessage GetMailMessage(string message)
	{
		MailMessage mailMessage = new MailMessage();
		mailMessage.BodyTransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;

		if (!String.IsNullOrEmpty(_to))
		{
			foreach (string toAddress in _to.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
			{
				mailMessage.To.Add(toAddress.Trim());
			}
		}

		if (!String.IsNullOrEmpty(_from))
		{
			mailMessage.From = new MailAddress(_from);
		}

		mailMessage.Subject = _subject;
		mailMessage.Body = message;
		return mailMessage;
	}

	/// <summary>
	/// The main internal implementation for composing the email.
	/// </summary>
	/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
	/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
	/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
	/// <param name="id">A numeric identifier for the event.</param>
	/// <param name="data">An array of objects to emit as data. If it is a string, it contains the actual message text.</param>
	private void SendTrace(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
	{
		StringBuilder message = new StringBuilder();
		DateTime now = DateTime.Now;

		foreach (object item in data)
		{
			if (item != null)
			{
				message.AppendLine(item.ToString());
				message.AppendLine();
			}
		}

		message.AppendLine("Event information:");
		message.AppendLine("    Event time: " + now.ToLocalTime().ToString(CultureInfo.InstalledUICulture));
		message.AppendLine("    Event UTC time: " + now.ToUniversalTime().ToString(CultureInfo.InstalledUICulture));
		message.AppendLine();

		message.AppendLine("Thread information:");
		message.AppendLine("    Culture: " + System.Threading.Thread.CurrentThread.CurrentCulture.Name);
		message.AppendLine("    UI Culture: " + System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
		message.AppendLine("    Thread ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
		message.AppendLine();

		// ApplicationInsights
#if NET6_0_OR_GREATER
		if (Activity.Current is not null)
		{
			message.AppendLine("Activity information:");
			message.AppendLine("    ID: " + Activity.Current.Id);
			message.AppendLine("    Root ID: " + Activity.Current.RootId + " (ApplicationInsights Operation Id)");
			message.AppendLine("    Parent ID: " + Activity.Current.ParentId);
			message.AppendLine("    Operation Name: " + Activity.Current.OperationName);
			message.AppendLine("    Kind: " + Activity.Current.Kind);
		}
#endif

		message.AppendLine("Event information:");
		message.AppendLine("    Event time: " + now.ToLocalTime().ToString(CultureInfo.InstalledUICulture));
		message.AppendLine("    Event UTC time: " + now.ToUniversalTime().ToString(CultureInfo.InstalledUICulture));
		message.AppendLine();

		// pro konzolovky, ve webových aplikacích vrací null
		// příklad: "TracingTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
		Assembly assembly = Assembly.GetEntryAssembly();

		// JK: IMHO zbytečný kód.
		// V běžném requestu máme health monitoring, netřeba se obvykle zabývat SMTP trace listenerem.
		// Nepokrýváme asynchronní kód, tasky, thready - ty nemají HttpContext.Current.
		// V konzolovce, atp., máme assembly.

		if (assembly == null)
		{
			// Implementace vychází z https://stackoverflow.com/a/6754205/4202832
			// Abychom nemuseli být závislí (dependency) na System.Web, nepoužijeme závislost na System.Web a proto musíme typ dohledat dynamicky.
			// A pracovat s ním reflexí.

			// Implementujeme "HttpContext.Current != null" reflexí
			Type httpContextType = Type.GetType("System.Web.HttpContext, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", throwOnError: false);
			if (httpContextType != null)
			{
				PropertyInfo httpContextCurrentMember = httpContextType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
				object httpContextCurrent = httpContextCurrentMember.GetValue(null /* static */);

				// pro requesty webových aplikací, v asynchronním tasku/threadu vrací HttpContext.Current null
				if (httpContextCurrent != null)
				{
					// příklad: "App_global.asax.agdxj0ym, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
					// v precompiled aplikaci bude, co čekáme

					// Ve druhé části implementujeme "assembly = HttpContext.Current.ApplicationInstance.GetType().Assembly;".
					object applicationInstance = httpContextType.GetProperty("ApplicationInstance", BindingFlags.Public | BindingFlags.Instance).GetValue(httpContextCurrent);
					assembly = applicationInstance.GetType().Assembly;
				}
			}
		}

		// pro asynchronní tasky/thready webových aplikací nevíme, jak získat assembly

		if (assembly != null)
		{
			message.AppendLine("Application info:");
			message.AppendLine("    Assembly: " + assembly.GetName().Name);
			message.AppendLine("    Assembly Version: " + assembly.GetName().Version);
			message.AppendLine("    Assembly File Version: " + FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion);
			message.AppendLine();
		}

		message.AppendLine("Environment:");
		message.AppendLine("    Command line: " + Environment.CommandLine);
		message.AppendLine("    Current directory: " + Environment.CurrentDirectory);
		message.AppendLine("    Machine name: " + Environment.MachineName);
		message.AppendLine("    User domain name: " + Environment.UserDomainName);
		message.AppendLine("    .NET Framework: " + Environment.Version.ToString());
		message.AppendLine("    OS Version: " + Environment.OSVersion.ToString());

		SendMessage(message.ToString());
	}

	/// <summary>
	/// Writes trace information, an array of data objects and event information to the listener specific output.
	/// </summary>
	/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
	/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
	/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
	/// <param name="id">A numeric identifier for the event.</param>
	/// <param name="data">An array of objects to emit as data.</param>
	/// <PermissionSet>
	/// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
	/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
	/// </PermissionSet>
	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
	{
		if ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
		{
			SendTrace(eventCache, source, eventType, id, data);
		}
	}

	/// <summary>
	/// Writes trace information, an array of data objects and event information to the listener specific output.
	/// </summary>
	/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
	/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
	/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
	/// <param name="id">A numeric identifier for the event.</param>
	/// <param name="data">An array of objects to emit as data.</param>
	/// <PermissionSet>
	/// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
	/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
	/// </PermissionSet>
	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
	{
		if ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
		{
			SendTrace(eventCache, source, eventType, id, data);
		}
	}

	/// <summary>
	/// Writes trace information, a message, and event information to the listener specific output.
	/// </summary>
	/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
	/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
	/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
	/// <param name="id">A numeric identifier for the event.</param>
	/// <param name="message">A message to write.</param>
	/// <PermissionSet>
	/// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
	/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
	/// </PermissionSet>
	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
	{
		if ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
		{
			SendTrace(eventCache, source, eventType, id, message);
		}
	}

	/// <summary>
	/// Writes trace information, a formatted array of objects and event information to the listener specific output.
	/// </summary>
	/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
	/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
	/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
	/// <param name="id">A numeric identifier for the event.</param>
	/// <param name="format">A format string that contains zero or more format items, which correspond to objects in the <paramref name="args"/> array.</param>
	/// <param name="args">An object array containing zero or more objects to format.</param>
	/// <PermissionSet>
	/// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
	/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
	/// </PermissionSet>
	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
	{
		if ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
		{
			SendTrace(eventCache, source, eventType, id, String.Format(CultureInfo.InvariantCulture, format, args));
		}
	}

	/// <summary>
	/// Writes trace information, a message, a related activity identity and event information to the listener specific output.
	/// </summary>
	/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
	/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
	/// <param name="id">A numeric identifier for the event.</param>
	/// <param name="message">A message to write.</param>
	/// <param name="relatedActivityId">A <see cref="T:System.Guid"/>  object identifying a related activity.</param>
	public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
	{
		SendTrace(eventCache, source, TraceEventType.Transfer, id, String.Format("{0} : {1}", message, relatedActivityId));
	}

	/// <summary>
	/// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
	/// </summary>
	/// <param name="message">A message to write.</param>
	public override void Write(string message)
	{
		TraceEvent(null, "Write", TraceEventType.Information, 0, message);
	}

	/// <summary>
	/// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
	/// </summary>
	/// <param name="message">A message to write.</param>
	public override void WriteLine(string message)
	{
		this.Write(message);
	}
}
