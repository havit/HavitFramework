using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework;
using Havit.Data.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Glimpse
{
	/// <summary>
	/// Trace listener for DbConnector command execution trace source.
	/// </summary>
	public class DbConnectorTraceListener : System.Diagnostics.TraceListener
	{
		#region MessageBroker
		internal IMessageBroker MessageBroker
		{
#pragma warning disable 0618
			get { return messageBroker ?? (messageBroker = GlimpseConfiguration.GetConfiguredMessageBroker()); }
#pragma warning restore 0618
			set { messageBroker = value; }
		}
		private IMessageBroker messageBroker;
		#endregion

		#region Constructors
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbConnectorTraceListener()
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbConnectorTraceListener(string initializeData) 
        {
        }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbConnectorTraceListener(IMessageBroker messageBroker)
		{
			MessageBroker = messageBroker;
		}
		#endregion

		#region TraceData
		/// <summary>
		/// For DbCommandTraceData in data, writes data to iMessageBroker to display in DbConnectorTab.
		/// </summary>
		public override void TraceData(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, object data)
		{
			base.TraceData(eventCache, source, eventType, id, data);

			if ((data != null) && (data is DbCommandTraceData))
			{
				IMessageBroker iMessageBroker = MessageBroker;
				if (iMessageBroker != null)
				{
					iMessageBroker.Publish((DbCommandTraceData)data);
				}
			}
		}
		#endregion

		#region Write
		/// <summary>
		/// Does nothing (NOOP).
		/// </summary>
		public override void Write(string message)
		{
			// NOOP
		}
		#endregion

		#region WriteLine
		/// <summary>
		/// Does nothing (NOOP).
		/// </summary>
		public override void WriteLine(string message)
		{
			// NOOP
		}
		#endregion
	}
}
