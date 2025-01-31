using System.Web.UI;

namespace Havit.Web.UI.Scriptlets;

/// <summary>
/// Repository extenderů. 
/// </summary>
public interface IControlExtenderRepository
{
	/// <summary>
	/// Vrací extender, který má Control zpracovávat.
	/// </summary>
	/// <param name="control">Control, který bude zpracováván.</param>
	/// <returns>Nalezený extender.</returns>
	IControlExtender FindControlExtender(Control control);
}