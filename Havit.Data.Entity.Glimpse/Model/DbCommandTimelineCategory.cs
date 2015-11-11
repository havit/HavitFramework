using Glimpse.Core.Message;

namespace Havit.Data.Entity.Glimpse.Model
{
	/// <summary>
	/// Timeline category for DbConnector.
	/// </summary>
	public static class DbCommandTimelineCategory
	{		
		#region TimelineCategory
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
		private static readonly TimelineCategoryItem _timelineCategory = new TimelineCategoryItem("DbCommand", "#30A000", "#30D000");
		#endregion
	}

}