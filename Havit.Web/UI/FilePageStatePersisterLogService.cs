using System.Diagnostics;

namespace Havit.Web.UI;

/// <summary>
/// Loguje oprace FilePageStatePersisteru.
/// </summary>
internal class FilePageStatePersisterLogService : FilePageStatePersister.ILogService
{
	private readonly TraceSource traceSource;

	public FilePageStatePersisterLogService()
	{
		traceSource = new TraceSource("FilePageStatePersister", SourceLevels.All);
	}

	/// <summary>
	/// Zapíše zprávu do logu.
	/// </summary>
	public void Log(string message, TraceEventType eventType = TraceEventType.Information)
	{
		traceSource.TraceEvent(eventType, 0, message, null);
		traceSource.Flush();
	}
}