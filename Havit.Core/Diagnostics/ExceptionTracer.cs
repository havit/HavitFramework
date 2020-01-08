#if NET462
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using Havit.Diagnostics.Contracts;

namespace Havit.Diagnostics
{
	/// <summary>
	/// Třída zajišťující posílání výjimek do trace prostřednictvím TraceSource.
	/// Mimo explicitního volání lze třídu přihlásit k odběru neošetřených výjimek, včetně WinForms.
	/// Třída je dostupná pouze pro full .NET Framework (nikoliv pro .NET Standard 2.0).
	/// <example>
	/// Příklad použití v ConsoleApplication:
	/// <code>
	/// namespace ExceptionLogging
	/// {
	///		class Program
	///		{
	///			static void Main(string[] args)
	///			{
	///				ExceptionTracer.Default.SubscribeToUnhandledExceptions(false);
	///
	///				ExceptionTracer.Default.TraceException(new ArgumentNullException("param", "Chybááá!"));
	///
	///				throw new InvalidOperationException("Chybka!");
	///			}
	///		}
	/// }
	/// </code>
	/// Příklad použití ve WindowsApplication:
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
	/// Příklad konfigurace App.config:
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
		/// Jméno TraceSource, přes který se budou výjimky emitovat.
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
		/// Název výchozího TraceSource, přes který jsou výjimky emitovány.
		/// </summary>
		public const string DefaultTraceSourceName = "Exceptions";

		/// <summary>
		/// Výchozí ExceptionTracer směřující výstup přes TraceSource s DefaultTraceSourceName.
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
		/// Vytvoří instanci ExceptionTraceru, který bude svůj výstup směřovat přes TraceSource se zadaným jménem.
		/// </summary>
		/// <param name="traceSourceName">jméno TraceSource, přes který se budou výjimky emitovat</param>
		public ExceptionTracer(string traceSourceName)
		{
			this._traceSourceName = traceSourceName;
		}

		/// <summary>
		/// Přihlásí ExceptionTracer k odběru všech neobsloužených výjimek (event AppDomain.CurrentDomain.UnhandledException).
		/// </summary>
		public void SubscribeToUnhandledExceptions(bool includeWindowsFormsThreadExceptions)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			if (includeWindowsFormsThreadExceptions)
			{
				SubscribeToWindowsFormsThreadExceptions();
			}
		}

		/// <summary>
		/// Obsluha události AppDomain.CurrentDomain.UnhandledException.
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
		/// Přihlásí ExceptionTracer k odběru všech neobsloužených výjimek WinForm (event Application.ThreadException).
		/// </summary>
		public void SubscribeToWindowsFormsThreadExceptions()
		{
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
		}

		/// <summary>
		/// Obsluha události Application.ThreadException.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Threading.ThreadExceptionEventArgs"/> instance containing the event data.</param>
		private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			TraceException(e.Exception, TraceEventType.Critical);

			// původní implementace obsluhy výjimky
			using (ThreadExceptionDialog excptDlg = new ThreadExceptionDialog(e.Exception))
			{
				DialogResult result = excptDlg.ShowDialog();
				if (result == DialogResult.Abort)
				{
					Application.Exit();
				}
			}
		}

		/// <summary>
		/// Pošle do trace zadanou výjimku.
		/// </summary>
		/// <param name="exception">výjimka k zaznamenání</param>
		/// <param name="eventType">typ události, pod kterým se má výjimka zaznamenat</param>
		/// <param name="eventId">ID eventu, pod kterým se má výjimka zaznamenat</param>
		public void TraceException(Exception exception, TraceEventType eventType, int eventId)
		{
			Contract.Requires<ArgumentNullException>(exception != null, nameof(exception));

			RunUsingTraceSource(delegate(TraceSource ts)
			{
				ts.TraceEvent(eventType, eventId, FormatException(exception));
			});
		}

		/// <summary>
		/// Pošle do trace zadanou výjimku.
		/// </summary>
		/// <param name="exception">výjimka k zaznamenání</param>
		/// <param name="eventType">typ události, pod kterým se má výjimka zaznamenat</param>
		public void TraceException(Exception exception, TraceEventType eventType)
		{
			TraceException(exception, eventType, traceExceptionMethodDefaultEventId);
		}

		/// <summary>
		/// Pošle do trace zadanou výjimku.
		/// </summary>
		/// <param name="exception">výjimka k zaznamenání</param>
		public void TraceException(Exception exception)
		{
			TraceException(exception, traceExceptionMethodDefaultEventType, traceExceptionMethodDefaultEventId);
		}

		/// <summary>
		/// Naformátuje výjimku pro zápis do trace.
		/// </summary>
		/// <param name="exception">výjimka</param>
		/// <returns>textový výstup, který se pošle do trace (informace o výjimce)</returns>
		private string FormatException(Exception exception)
		{
			// do budoucna je možné rozšířit objektový model o ExceptionTraceFormatter, atp.
			return exception.ToString();
		}

		/// <summary>
		/// Vykoná akci pomocí TraceSource používaného ExceptionListenerem.
		/// </summary>
		/// <param name="action">akce k vykonání (delegát)</param>
		private void RunUsingTraceSource(Action<TraceSource> action)
		{
			Debug.Assert(action != null);

			TraceSource ts = new TraceSource(this.TraceSourceName);

			action(ts);

			ts.Flush();
			ts.Close();
		}
	}
}
#endif