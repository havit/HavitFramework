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
		internal IMessageBroker MessageBroker
		{
			get
			{
				if (messageBroker == null)
				{
#pragma warning disable 0618
					messageBroker = GlimpseConfiguration.GetConfiguredMessageBroker();
#pragma warning restore 0618
				}
				return messageBroker;
			}
			set
			{
				messageBroker = value;
			}
		}
		private IMessageBroker messageBroker;

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
					lock (iMessageBroker) // viz TFS 14220 - zdá se, že metoda publish není thread safe
					{
						iMessageBroker.Publish((DbCommandTraceData)data);
					}
				}
			}
		}

		/// <summary>
		/// Does nothing (NOOP).
		/// </summary>
		public override void Write(string message)
		{
			// NOOP
		}

		/// <summary>
		/// Does nothing (NOOP).
		/// </summary>
		public override void WriteLine(string message)
		{
			// NOOP
		}
	}
}
