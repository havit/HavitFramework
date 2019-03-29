namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Režim renderování struktury HTML DateTimeBoxu.
	/// </summary>
	public enum DateTimeBoxContainerRenderMode
	{
		/// <summary>
		/// Zajišťuje renderování standardní struktury - textbox pro hodnotu, tvrdá mezera a obrázek/ikonka pro výběr kalendáře.
		/// </summary>
		Standard,

		/// <summary>
		/// Viz BootstrapInputGroupButtonOnRight, avšak ikonka pro výběr kalendáře je nalevo.
		/// </summary>
		BootstrapInputGroupButtonOnLeft,

		/// <summary>
		/// Zajišťuje renderování struktury dle konvencí Bootstrapu pro Input Group a Input Group Button.
		/// Třídy input-group, input-group-btn pro DIVy a form-control pro textbox jsou přidány automaticky.
		/// Ikonka pro výběr kalendáře je napravo.
		/// </summary>
		BootstrapInputGroupButtonOnRight
	}
}