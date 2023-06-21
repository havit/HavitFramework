using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Common;

using Glimpse.Core.Message;

namespace Havit.Data.Entity.Glimpse.Model;

/// <summary>
/// Položka záložky Entity Framework, zároveň je zobrazovaná i v timeline.
/// </summary>
public class DbCommandMessage : MessageBase, ITimelineMessage
{
	/// <summary>
	/// Operace (ExecuteScalar, ExecuteReader, ExecuteNonQuery).
	/// </summary>
	public string Operation { get; set; }

	/// <summary>
	/// Provedený databázový dotaz.
	/// </summary>
	public string CommandText { get; set; }

	/// <summary>
	/// Indikuje, zda byl dotaz proveden asynchronně.
	/// </summary>
	public bool IsAsync { get; set; }

	/// <summary>
	/// Indikuje, k jeké došlo výjimce.
	/// </summary>
	public Exception Exception { get; set; }

	/// <summary>
	/// Výsledek dotazu. Pro ExecuteScalar je zde hodnota, která je výsledkem. Pro ExecuteDataReader je zde instance DbDataReaderResult, která udává počet záznamů v DbDataReaderu.
	/// </summary>
	public object Result { get; set; }

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

	/// <summary>
	/// Duration in Timeline tab.
	/// </summary>
	public TimeSpan Duration
	{
		get;
		set;
	}

	/// <summary>
	/// Time offset of the event in the Timeline tab.
	/// </summary>
	public TimeSpan Offset
	{
		get;
		set;
	}

	/// <summary>
	/// Event start time in the Timeline tab.
	/// </summary>
	public DateTime StartTime
	{
		get;
		set;
	}

	/// <summary>
	/// Event category in Timeline tab.
	/// </summary>
	TimelineCategoryItem ITimelineMessage.EventCategory
	{
		get { return DbCommandTimelineCategory.TimelineCategory; }
		set { throw new NotSupportedException(); }
	}

	/// <summary>
	/// Event name in Timeline tab.
	/// </summary>
	string ITimelineMessage.EventName
	{
		get { return Operation; }
		set { throw new NotSupportedException(); }
	}

	/// <summary>
	/// Event sub text (detailed description) in Timeline tab.
	/// </summary>
	string ITimelineMessage.EventSubText
	{
		get { return CommandText; }
		set { throw new NotSupportedException(); }
	}
}
