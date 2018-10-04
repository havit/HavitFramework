using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Common;

using Glimpse.Core.Message;

namespace Havit.Data.Entity.Glimpse.Model
{
	/// <summary>
	/// Položka záložky Entity Framework, zároveň je zobrazovaná i v timeline.
	/// </summary>
	public class DbCommandMessage : MessageBase, ITimelineMessage
	{
		#region Operation
		/// <summary>
		/// Operace (ExecuteScalar, ExecuteReader, ExecuteNonQuery).
		/// </summary>
		public string Operation { get; set; }
		#endregion

		#region CommandText
		/// <summary>
		/// Provedený databázový dotaz.
		/// </summary>
		public string CommandText { get; set; }
		#endregion

		#region IsAsync
		/// <summary>
		/// Indikuje, zda byl dotaz proveden asynchronně.
		/// </summary>
		public bool IsAsync { get; set; }
		#endregion

		#region Exception
		/// <summary>
		/// Indikuje, k jeké došlo výjimce.
		/// </summary>
		public Exception Exception { get; set; }
		#endregion

		#region Result
		/// <summary>
		/// Výsledek dotazu. Pro ExecuteScalar je zde hodnota, která je výsledkem. Pro ExecuteDataReader je zde instance DbDataReaderResult, která udává počet záznamů v DbDataReaderu.
		/// </summary>
		public object Result { get; set; }
		#endregion

		#region CommandParameters
		/// <summary>
		/// Parametry provedeného databázového dotazu.
		/// </summary>
		public List<DbParameterData> CommandParameters
		{
			get
			{
				return _commandParameters;
			}
		}
		private readonly List<DbParameterData> _commandParameters = new List<DbParameterData>();
		#endregion

		#region Duration
		/// <summary>
		/// Duration in Timeline tab.
		/// </summary>
		public TimeSpan Duration
		{
			get;
			set;
		}
		#endregion

		#region Offset
		/// <summary>
		/// Time offset of the event in the Timeline tab.
		/// </summary>
		public TimeSpan Offset
		{
			get;
			set;
		}
		#endregion

		#region StartTime
		/// <summary>
		/// Event start time in the Timeline tab.
		/// </summary>
		public DateTime StartTime
		{
			get;
			set;
		}
		#endregion

		#region Timeline tab properties (EventCategory, EventName, EventSubText)

		#region EventCategory
		/// <summary>
		/// Event category in Timeline tab.
		/// </summary>
		TimelineCategoryItem ITimelineMessage.EventCategory
		{
			get { return DbCommandTimelineCategory.TimelineCategory; }
			set { throw new NotSupportedException(); }
		}
		#endregion

		#region EventName
		/// <summary>
		/// Event name in Timeline tab.
		/// </summary>
		string ITimelineMessage.EventName
		{
			get { return Operation; }
			set { throw new NotSupportedException(); }
		}
		#endregion

		#region EventSubText
		/// <summary>
		/// Event sub text (detailed description) in Timeline tab.
		/// </summary>
		string ITimelineMessage.EventSubText
		{
			get { return CommandText; }
			set { throw new NotSupportedException(); }
		}
		#endregion

		#endregion
	}
}
