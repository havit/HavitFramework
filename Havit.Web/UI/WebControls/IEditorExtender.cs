using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Interface pro controly, které slouží jako externí editor záznamu.
	/// </summary>
	public interface IEditorExtender
	{
		/// <summary>
		/// Zobrazí editor.
		/// </summary>
		void StartEditor();

		/// <summary>
		/// Extrahuje hodnoty z editoru do datového objektu.
		/// </summary>
		void ExtractValues(object dataObject);

		/// <summary>
		/// Událost pro získání instance editovaného objektu.
		/// </summary>
		/// <remarks>
		/// Obsluha události má nastavit instanci editovaného objektu.
		/// </remarks>
		event DataEventHandler<object> GetEditedObject;

		/// <summary>
		/// Oznamuje začátek ukládání záznamu.
		/// </summary>
		/// <remarks>
		/// V obsluze události se očekává uložení editovaného záznamu.
		/// </remarks>
		event CancelEventHandler ItemSaving;

		/// <summary>
		/// Oznamuje uložení záznamu.
		/// </summary>
		event EventHandler ItemSaved;

		/// <summary>
		/// Oznamuje ukončení editace.
		/// </summary>
		event EventHandler EditClosed;
	}
}