namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Reim renderování struktury HTML DateTimeBoxu.
	/// </summary>
	public enum DateTimeBoxContainerRenderMode
	{
		/// <summary>
		/// Zajišuje renderování standardní struktury - textbox pro hodnotu, tvrdá mezera a obrázek/ikonka pro vıbìr kalendáøe.
		/// </summary>
		Standard,

		/// <summary>
		/// Viz BootstrapInputGroupButtonOnRight, avšak ikonka pro vıbìr kalendáøe je nalevo.
		/// </summary>
		BootstrapInputGroupButtonOnLeft,

		/// <summary>
		/// Zajišuje renderování struktury dle konvencí Bootstrapu pro Input Group a Input Group Button.
		/// Tøídy input-group, input-group-btn pro DIVy a form-control pro textbox jsou pøidány automaticky.
		/// Ikonka pro vıbìr kalendáøe je napravo.
		/// </summary>
		BootstrapInputGroupButtonOnRight
	}
}