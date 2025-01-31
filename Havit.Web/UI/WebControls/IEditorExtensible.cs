namespace Havit.Web.UI.WebControls;

/// <summary>
/// Interface pro controly, které umožňují editaci záznamu v externím editoru.
/// </summary>
public interface IEditorExtensible
{
	/// <summary>
	/// Controlu, který umožňuje editaci záznamu v externím editoru, je oznámena registrace editoru.
	/// </summary>
	/// <param name="editorExtender">Externí editor použití pro editaci záznamu.</param>
	void RegisterEditor(IEditorExtender editorExtender);
}
