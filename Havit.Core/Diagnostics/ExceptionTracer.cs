using System;
using System.Diagnostics;
using Havit.Diagnostics.Contracts;

namespace Havit.Diagnostics;

/// <summary>
/// Class responsible for sending exceptions to trace through TraceSource.
/// In addition to explicit calls, the class can be subscribed to handle unhandled exceptions, including WinForms.
/// The class is available only for full .NET Framework (not for .NET Standard 2.0).
/// <example>
/// Example of usage in ConsoleApplication:
/// <code>
/// namespace ExceptionLogging
/// {
///		class Program
///		{
///			static void Main(string[] args)
///			{
///				ExceptionTracer.Default.SubscribeToUnhandledExceptions(false);
///
///				ExceptionTracer.Default.TraceException(new ArgumentNullException("param", "Error!"));
///
///				throw new InvalidOperationException("Error!");
///			}
///		}
/// }
/// </code>
/// Example of usage in WindowsApplication:
/// <code>
///	static void Main()
///	{
///		ExceptionTracer.Default.SubscribeToUnhandledExceptions(true);
///
///		Application.EnableVisualStyles();
///		Application.SetCompatibleTextRenderingDefault(false);
///		Application.Run(new Form1());
///	}
/// </code>
/// Example of App.config configuration:
/// <code>
/// &lt;configuration&gt;
///		&lt;system.diagnostics&gt;
///			&lt;sources&gt;
///				&lt;source name="Exceptions" switchValue="Error"&gt;
///					&lt;listeners&gt;
///						&lt;add name="LogFileListener"
///							type="System.Diagnostics.TextWriterTraceListener"
///							 initializeData="Exceptions.log"
///						/&gt;
///						&lt;add name="XmlListener"
///							 initializeData="Exceptions.xml"
///							 type="System.Diagnostics.XmlWriterTraceListener"
///					/&gt;
///					&lt;/listeners&gt;
///				&lt;/source&gt;
///			&lt;/sources&gt;
///		&lt;/system.diagnostics&gt;
/// &lt;/configuration&gt;
/// </code>
/// </example>
/// </summary>
public class ExceptionTracer
{
	private const TraceEventType traceExceptionMethodDefaultEventType = TraceEventType.Error;
	private const int traceExceptionMethodDefaultEventId = 0;

	/// <summary>
	/// Name of the TraceSource through which exceptions will be emitted.
	/// </summary>
	public string TraceSourceName
	{
		get
		{
			return _traceSourceName;
		}
		private set
		{
			_traceSourceName = value;
		}
	}
	private string _traceSourceName;

	/// <summary>
	/// Name of the default TraceSource through which exceptions will be emitted.
	/// </summary>
	public const string DefaultTraceSourceName = "Exceptions";

	/// <summary>
	/// Default ExceptionTracer directing output through TraceSource with DefaultTraceSourceName.
	/// </summary>
	public static ExceptionTracer Default
	{
		get
		{
			if (_default == null)
			{
				lock (defaultLock)
				{
					if (_default == null)
					{
						_default = new ExceptionTracer(DefaultTraceSourceName);
					}
				}
			}
			return _default;
		}
		set
		{
			_default = value;
		}
	}
	private static ExceptionTracer _default;
	private static readonly object defaultLock = new object();

	/// <summary>
	/// Creates an instance of ExceptionTracer that will direct its output through the TraceSource with the specified name.
	/// </summary>
	/// <param name="traceSourceName">The name of the TraceSource through which exceptions will be emitted.</param>
	public ExceptionTracer(string traceSourceName)
	{
		this._traceSourceName = traceSourceName;
	}

	/// <summary>
	/// Subscribes the ExceptionTracer to handle all unhandled exceptions (event AppDomain.CurrentDomain.UnhandledException).
	/// Obsolete, use method with no arguments.
	/// </summary>
	[Obsolete("Use SubscribeToUnhandledExceptions() with no parameters.", error: true)]
	public void SubscribeToUnhandledExceptions(bool includeWindowsFormsThreadExceptions)
	{
		throw new NotSupportedException();
	}

	/// <summary>
	/// Subscribes the ExceptionTracer to handle all unhandled exceptions (event AppDomain.CurrentDomain.UnhandledException).
	/// </summary>
	public void SubscribeToUnhandledExceptions()
	{
		AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
	}

	/// <summary>
	/// Handles the AppDomain.CurrentDomain.UnhandledException event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
	private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		if (e.ExceptionObject is Exception)
		{
			TraceException((Exception)e.ExceptionObject, TraceEventType.Critical);
		}
	}

	/// <summary>
	/// Sends the specified exception to trace.
	/// </summary>
	/// <param name="exception">The exception to be logged.</param>
	/// <param name="eventType">The event type under which the exception should be logged.</param>
	/// <param name="eventId">The event ID under which the exception should be logged.</param>
	public void TraceException(Exception exception, TraceEventType eventType, int eventId)
	{
		Contract.Requires<ArgumentNullException>(exception != null, nameof(exception));

		RunUsingTraceSource(delegate (TraceSource ts)
		{
			ts.TraceEvent(eventType, eventId, FormatException(exception));
		});
	}

	/// <summary>
	/// Sends the specified exception to trace.
	/// </summary>
	/// <param name="exception">The exception to be logged.</param>
	/// <param name="eventType">The event type under which the exception should be logged.</param>
	public void TraceException(Exception exception, TraceEventType eventType)
	{
		TraceException(exception, eventType, traceExceptionMethodDefaultEventId);
	}

	/// <summary>
	/// Sends the specified exception to trace.
	/// </summary>
	/// <param name="exception">exception to be logged</param>
	public void TraceException(Exception exception)
	{
		TraceException(exception, traceExceptionMethodDefaultEventType, traceExceptionMethodDefaultEventId);
	}

	/// <summary>
	/// Formats the exception for writing to trace.
	/// </summary>
	/// <param name="exception">The exception.</param>
	/// <returns>The text output to be sent to trace (exception information).</returns>
	private string FormatException(Exception exception)
	{
		// In the future, it is possible to extend the object model with ExceptionTraceFormatter, etc.
		return exception.ToString();
	}

	/// <summary>
	/// Executes an action using the TraceSource used by the ExceptionListener.
	/// </summary>
	/// <param name="action">action to be executed (delegate)</param>
	private void RunUsingTraceSource(Action<TraceSource> action)
	{
		Debug.Assert(action != null);

		TraceSource ts = new TraceSource(this.TraceSourceName);

		action(ts);

		ts.Flush();
		ts.Close();
	}
}
