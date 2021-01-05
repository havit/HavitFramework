using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.Mail;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Configuration;

namespace Havit.Diagnostics
{
	/// <summary>
	/// TraceListener, který výstup posílá mailem.
	/// Konfigurace se provádí přes initializeData v kontruktoru, podporovány jsou hodnoty:
	/// <list type="bullet">
	///		<item>
	///			<term>subject</term>
	///			<description>Předmět zasílaného emailu.</description>
	///		</item>
	///		<item>
	///			<term>to</term>
	///			<description>Adresát emailu.</description>
	///		</item>
	///		<item>
	///			<term>from</term>
	///			<description>Odesílatel emailu.</description>
	///		</item>
	///		<item>
	///			<term>smtpServer</term>
	///			<description>Smtp server k odeslání zprávy.</description>
	///		</item>
	///		<item>
	///			<term>smtpPort</term>
	///			<description>Port, který se má použít pro komunikaci se Smtp serverem (není-li nastaveno, nepoužije se).</description>
	///		</item>
	///		<item>
	///			<term>smtpUsername</term>
	///			<description>Username smtp serveru (není-li nastaveno, nepoužije se).</description>
	///		</item>
	///		<item>
	///			<term>smtpPassword</term>
	///			<description>Heslo smtp serveru.</description>
	///		</item>
	///		<item>
	///			<term>smtpEnableSsl</term>
	///			<description>Indikuje, zda se má použít SSL při připojení k smtp serveru. Výchozí hodnota je false.</description>
	///		</item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// Inspirováno implementaci System.Diagnostics.XmlWriterListener.
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
		/// Constructor, který je volán při použití TraceListerneru z app.configu a předává se do něj hodnota atributu initializeData.
		/// </summary>
		/// <param name="initializeData">hodnota atributu initializeData z app.config</param>
		public SmtpTraceListener(string initializeData)
		{
			if (initializeData == null)
			{
				return; // použijí se defaulty				
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
							throw new ConfigurationErrorsException(String.Format("Neznámý parametr '{0}' konfigurace SmtpTraceListeneru v initializeData.", paramValue[0]/* nedávám paramenterName, protože jej chci zobrazit bez provedeného Trim a ToLower */));
					}
				}
			}

			if (String.IsNullOrEmpty(_smtpServer))
			{
				if (!String.IsNullOrEmpty(_smtpUsername) || !String.IsNullOrEmpty(_smtpPassword))
				{
					throw new ConfigurationErrorsException("Credentials can be set only when smtp server (host) is specified.");
				}

				if (_smtpPort != null)
				{
					throw new ConfigurationErrorsException("Smtp port can be set only when smtp server (host) is specified.");
				}

				if (_smtpEnableSsl != false)
				{
					throw new ConfigurationErrorsException("Smtp ssl settings can be set only when smtp server (host) is specified.");
				}
			}
		}

		/// <summary>
		/// Interní implementace odesílání mailu.
		/// </summary>
		/// <param name="message">zpráva z trace</param>
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
				// NOOP - nechceme, aby nám nefunkční trace-mailing zabil server
#if DEBUG
				// při debugování nás to ale zajímá
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
		/// Vrací kompletně nakonfigurovanou instanci SmtpClient pro odeslání emailu.
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
		/// Vrací kompletně nakonfigurovanou MailMessage k odeslání.
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
		/// Hlavní interní implementace sestavení mailu.
		/// </summary>
		/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
		/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">An array of objects to emit as data. Pokud je string, obsahuje přímo text zprávy.</param>
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

			// pro konzolovky, ve webových aplikacích vrací null
			// příklad: "TracingTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
			Assembly assembly = Assembly.GetEntryAssembly();
#if NET472
			// pro requesty webových aplikací, v asynchronním tasku/threadu vrací HttpContext.Current null
			if ((assembly == null) && (HttpContext.Current != null))
			{
				// příklad: "App_global.asax.agdxj0ym, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
				// v precompiled aplikaci bude, co čekáme
				assembly = HttpContext.Current.ApplicationInstance.GetType().Assembly;
			}
#endif

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
}
