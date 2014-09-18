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
		/// Viz BootstrapInputGroupAddOnOnRight, avšak ikonka pro vıbìr kalendáøe je nalevo.
		/// </summary>
		BootstrapInputGroupAddOnOnLeft,

		/// <summary>
		/// Zajišuje renderování struktury dle konvencí Bootstrapu pro Input Group a Input Group Addon.
		/// Tøídy input-group, input-group-addon pro DIVy a form-control pro textbox jsou pøidány automaticky.
		/// Ikonka pro vıbìr kalendáøe je napravo.
		/// </summary>
		BootstrapInputGroupAddOnOnRight
	}
}