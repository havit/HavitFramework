using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;
using Glimpse.Core.SerializationConverter;

using Havit.Data.Glimpse.Message;
using Havit.Data.Trace;

namespace Havit.Data.Glimpse.Inspector;

/// <summary>
/// Writes DbConnector messages to timeline.
/// </summary>
public class DbConnectorTimelineInspector : IInspector
{
	/// <summary>
	/// Message broker.
	/// </summary>
	protected IMessageBroker MessageBroker { get; private set; }

	/// <summary>
	/// Timer - strategy pattern.
	/// </summary>
	protected Func<IExecutionTimer> TimerStrategy { get; private set; }

	/// <summary>
	/// Sets up inspector.
	/// </summary>
	public void Setup(IInspectorContext context)
	{
		this.MessageBroker = context.MessageBroker;
		this.TimerStrategy = context.TimerStrategy;
		context.MessageBroker.Subscribe<DbCommandTraceData>(ProcessMessage);
	}

	/// <summary>
	/// Writes dbCommandTraceData to timeline.
	/// </summary>
	private void ProcessMessage(DbCommandTraceData dbCommandTraceData)
	{
		DateTime now = DateTime.Now;

		IExecutionTimer timer = null;
		if (this.TimerStrategy != null)
		{
			timer = this.TimerStrategy();
		}

		ITimelineMessage message = new DbConnectorTimelineMessage();

		message.EventCategory = DbConnectorTimelineCategory.TimelineCategory;
		message.EventName = dbCommandTraceData.Operation;
		message.EventSubText = dbCommandTraceData.CommandText;
		message.Duration = new TimeSpan(dbCommandTraceData.DurationTicks);

		if (timer != null)
		{
			TimerResult timePoint = timer.Point();
			message.Offset = new TimeSpan(timePoint.Offset.Ticks - dbCommandTraceData.DurationTicks);
			message.StartTime = now - message.Duration;
		}

		this.MessageBroker.Publish(message);
	}
}
