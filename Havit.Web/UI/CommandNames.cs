using System.Web.UI.WebControls;

namespace Havit.Web.UI;

/// <summary>
/// Třída CommandNames obsahuje sdílené názvy příkazů (CommandNames).
/// Sdílí se mezi prvky vyvolávající událost příkazu (potomci GridViewImageButton)
/// a EnterpriseGridView.
/// </summary>
public static class CommandNames
{
	/// <summary>
	/// Cancel.
	/// </summary>
	public const string Cancel = DataControlCommands.CancelCommandName;

	/// <summary>
	/// Delete.
	/// </summary>
	public const string Delete = DataControlCommands.DeleteCommandName;

	/// <summary>
	/// Edit.
	/// </summary>
	public const string Edit = DataControlCommands.EditCommandName;

	/// <summary>
	/// New.
	/// </summary>
	public const string New = DataControlCommands.NewCommandName;

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
	public const string Update = DataControlCommands.UpdateCommandName;

	/// <summary>
	/// Select.
	/// </summary>
	public const string Select = DataControlCommands.SelectCommandName;

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
	public const string Insert = DataControlCommands.InsertCommandName;

	/// <summary>
	/// Save.
	/// </summary>
	public const string Save = "Save";

	/// <summary>
	/// OK.
	/// </summary>
	public const string OK = "OK";

	/// <summary>
	/// Next.
	/// </summary>
	public const string Next = "Next";

	/// <summary>
	/// Previous.
	/// </summary>
	public const string Previous = "Previous";
}
