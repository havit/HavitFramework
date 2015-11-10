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
		#region Prifate fields
		private readonly TraceSource traceSource;
		#endregion

		#region Constructor
		public FilePageStatePersisterLogService()
		{
			traceSource = new TraceSource("FilePageStatePersister", SourceLevels.All);				
		}
		#endregion

		#region Log
		/// <summary>
		/// Zapíše zprávu do logu.
		/// </summary>
		public void Log(string message)
		{
			traceSource.TraceInformation(message);
			traceSource.Flush();
		}
		#endregion
	}
}