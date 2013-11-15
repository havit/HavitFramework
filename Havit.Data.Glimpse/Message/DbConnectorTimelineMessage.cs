using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glimpse.Core.Message;

namespace Havit.Data.Glimpse.Message
{
	public class DbConnectorTimelineMessage : MessageBase, ITimelineMessage
	{
		#region EventCategory
		/// <summary>
		/// Event category (should be always DbConnetorTimelineCategory.TimelineCategory.
		/// </summary>
		public TimelineCategoryItem EventCategory
		{
			get;
			set;
		}
		#endregion

		#region EventName
		/// <summary>
		/// Event name
		/// </summary>
		public string EventName
		{
			get;
			set;
		}
		#endregion

		#region EventSubText
		/// <summary>
		/// Event sub text (detailed description)
		/// </summary>
		public string EventSubText
		{
			get;
			set;
		}
		#endregion

		#region Duration
		/// <summary>
		/// Duration.
		/// </summary>
		public TimeSpan Duration
		{
			get;
			set;
		}
		#endregion

		#region Offset
		/// <summary>
		/// Time offset of the event.
		/// </summary>
		public TimeSpan Offset
		{
			get;
			set;
		}
		#endregion

		#region StartTime
		/// <summary>
		/// Event start time.
		/// </summary>
		public DateTime StartTime
		{
			get;
			set;
		}
		#endregion

	}
}
