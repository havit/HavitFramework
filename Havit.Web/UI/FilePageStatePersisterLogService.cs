using System;
using System.Diagnostics;
using System.ServiceModel.Channels;

namespace Havit.Web.UI
{
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
		public void Log(string message)
		{
			traceSource.TraceInformation(message);
			traceSource.Flush();
		}
	}
}