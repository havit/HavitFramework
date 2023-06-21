using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glimpse.Core.Message;

namespace Havit.Data.Glimpse.Message;

/// <summary>
/// Timeline category for DbConnector.
/// </summary>
public static class DbConnectorTimelineCategory
{
	/// <summary>
	/// Timeline category for DbConnector.
	/// </summary>
	public static TimelineCategoryItem TimelineCategory
	{
		get
		{
			return _timelineCategory;
		}
	}
	private static readonly TimelineCategoryItem _timelineCategory = new TimelineCategoryItem("DbConnector", "#233D91", "#118DD5");
}