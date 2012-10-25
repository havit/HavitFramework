using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.Mail;
using System.Globalization;

namespace Havit.Diagnostics
{
	/// <summary>
	/// TraceListener, který výstup posílá mailem.
	/// </summary>
	/// <remarks>
	/// Inspirováno implementaci System.Diagnostics.XmlWriterListener.
	/// </remarks>
	public class SmtpTraceListener : TraceListener
	{
		#region MailTo
		/// <summary>
		/// E-mailová adresa, na kterou se posílají zprávy.
		/// </summary>
		public string MailTo
		{
			get
			{
				if (_mailTo == null)
				{
					return "devmail@havit.cz";
				}
				return _mailTo;
			}
			set
			{
				_mailTo = value;
			}
		}
		private string _mailTo;
		#endregion

		#region Subject
		/// <summary>
		/// Subject zprávy.
		/// </summary>
		public string Subject
		{
			get
			{
				if (_subject == null)
				{
					return "SmtpTraceListener";
				}
				return _subject;
			}
			set
			{
				_subject = value;
			}
		}
		private string _subject;
		#endregion

		#region Constructors
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
					switch (paramValue[0].Trim().ToUpper())
					{
						case "TO":
							MailTo = paramValue[1].Trim();
							break;
						case "SUBJECT":
							Subject = paramValue[1].Trim();
							break;
						default:
							throw new InvalidOperationException("Neznámý parametr konfigurace SmtpTraceListeneru v initializeData.");
					}
				}
			}
		}
		#endregion

		#region SendMessage
		/// <summary>
		/// Interní implementace odesílání mailu.
		/// </summary>
		/// <param name="message">zpráva z trace</param>
		private void SendMessage(string message)
		{
			if (String.IsNullOrEmpty(this.MailTo))
			{
				return;
			}

			try
			{
				MailMessage mailMessage = new MailMessage();
				mailMessage.To.Add(this.MailTo);
				mailMessage.Subject = this.Subject;
				mailMessage.Body = message;
				SmtpClient smtpClient = new SmtpClient();
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
		#endregion

		#region SendTrace
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
			foreach (object item in data)
			{
				if (item != null)
				{
					message.AppendLine(item.ToString());
				}
			}
			message.AppendLine();

			message.Append("CommandLine: ");
			message.AppendLine(Environment.CommandLine);

			message.Append("CurrentDirectory: ");
			message.AppendLine(Environment.CurrentDirectory);

			message.Append("MachineName: ");
			message.AppendLine(Environment.MachineName);

			message.Append("UserDomainName: ");
			message.AppendLine(Environment.UserDomainName);

			message.Append(".NET Framework: ");
			message.AppendLine(Environment.Version.ToString());

			if (eventCache != null)
			{
				message.AppendLine();
				message.AppendLine("Call stack:");
				message.AppendLine(eventCache.Callstack);
				message.AppendLine();

				message.AppendLine("Logical operation stack:");
				foreach (object item in eventCache.LogicalOperationStack)
				{
					if (item != null)
					{
						message.AppendLine(item.ToString());
					}
				}
				message.AppendLine();

				message.Append("DateTime: ");
				message.AppendLine(eventCache.DateTime.ToString());

				message.Append("Timestamp: ");
				message.AppendLine(eventCache.Timestamp.ToString());

				message.Append("ProcessId: ");
				message.AppendLine(eventCache.ProcessId.ToString());

				message.Append("ThreadId: ");
				message.AppendLine(eventCache.ThreadId);
			}

			if (!String.IsNullOrEmpty(source))
			{
				message.Append("Source: ");
				message.AppendLine(source);
			}

			message.Append("EventType: ");
			message.AppendLine(eventType.ToString("g"));

			message.Append("EventId: ");
			message.AppendLine(id.ToString("g"));

			SendMessage(message.ToString());
		}
		#endregion

		#region TraceData (override)
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
		#endregion

		#region TraceEvent (override)
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
		#endregion

		#region TraceTransfer (override)
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
		#endregion

		#region Write, WriteLine (override)
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
		#endregion
	}
}
