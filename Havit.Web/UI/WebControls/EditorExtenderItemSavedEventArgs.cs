using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Argumenty události uložení datového objektu.
/// </summary>
public class EditorExtenderItemSavedEventArgs : EventArgs
{
	/// <summary>
	/// Editovaný datový objekt. Slouží k použití v obsluze události ukládání objektu.
	/// </summary>
	public object SavedObject { get; set; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EditorExtenderItemSavedEventArgs()
	{
		
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EditorExtenderItemSavedEventArgs(object savedObject)
	{
		this.SavedObject = savedObject;
	}
}

/// <summary>
/// Událost uložení datového objektu.
/// </summary>
public delegate void EditorExtenderItemSavedEventHandler(object sender, EditorExtenderItemSavedEventArgs args);
