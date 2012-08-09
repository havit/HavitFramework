using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI
{
	/// <summary>
	/// Tøída CommandNames obsahuje sdílené názvy pøíkazù (CommandNames).
	/// Sdílí se mezi prvky vyvolávající událost pøíkazu (potomci GridViewImageButton)
	/// a EnterpriseGridView.
	/// </summary>
	public static class CommandNames
	{
		/// <summary>
		/// Cancel.
		/// </summary>
		public const string Cancel = "Cancel";

		/// <summary>
		/// Delete.
		/// </summary>
		public const string Delete = "Delete";

		/// <summary>
		/// Edit.
		/// </summary>
		public const string Edit = "Edit";

		/// <summary>
		/// New.
		/// </summary>
		public const string New = "New";

		/// <summary>
		/// MoveDown.
		/// </summary>
		public const string MoveDown = "MoveDown";

		/// <summary>
		/// MoveUp.
		/// </summary>
		public const string MoveUp = "MoveUp";

		/// <summary>
		/// Update.
		/// </summary>
		public const string Update = "Update";

		/// <summary>
		/// Select.
		/// </summary>
		public const string Select = "Select";

		/// <summary>
		/// Detail.
		/// </summary>
		public const string Detail = "Detail";

		/// <summary>
		/// Report.
		/// </summary>
		public const string Report = "Report";

		/// <summary>
		/// Insert.
		/// </summary>
		public const string Insert = "Insert";
	}
}
