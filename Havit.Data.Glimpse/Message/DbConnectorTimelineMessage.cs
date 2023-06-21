using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glimpse.Core.Message;

namespace Havit.Data.Glimpse.Message;

/// <summary>
/// DbConnector timeline message.
/// </summary>
public class DbConnectorTimelineMessage : MessageBase, ITimelineMessage
{
	/// <summary>
	/// Event category (should be always DbConnetorTimelineCategory.TimelineCategory.
	/// </summary>
	public TimelineCategoryItem EventCategory
	{
		get;
		set;
	}

	/// <summary>
	/// Event name
	/// </summary>
	public string EventName
	{
		get;
		set;
	}

	/// <summary>
	/// Event sub text (detailed description)
	/// </summary>
	public string EventSubText
	{
		get;
		set;
	}

	/// <summary>
	/// Duration.
	/// </summary>
	public TimeSpan Duration
	{
		get;
		set;
	}

	/// <summary>
	/// Time offset of the event.
	/// </summary>
	public TimeSpan Offset
	{
		get;
		set;
	}

	/// <summary>
	/// Event start time.
	/// </summary>
	public DateTime StartTime
	{
		get;
		set;
	}
}
