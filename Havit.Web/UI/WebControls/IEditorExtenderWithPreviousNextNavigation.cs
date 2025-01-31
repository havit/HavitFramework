using System.ComponentModel;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Interface pro controly, které slouží jako externí editor záznamu a podporují navigaci na předchozí a následující záznam.
/// </summary>
public interface IEditorExtenderWithPreviousNextNavigation : IEditorExtender
{
	/// <summary>
	/// Oznamuje navigaci na předchozí záznam.
	/// </summary>
	event CancelEventHandler PreviousNavigating;

	/// <summary>
	/// Oznamuje dokončení navigace na předchozí záznam.
	/// </summary>
	event EventHandler PreviousNavigated;

	/// <summary>
	/// Oznamuji navigaci na následující záznam.
	/// </summary>
	event CancelEventHandler NextNavigating;

	/// <summary>
	/// Oznamuje dokončení navigace na následuící záznam.
	/// </summary>
	event EventHandler NextNavigated;

	/// <summary>
	/// Zjišťuje, zda je možné přejín na předchozí záznam.
	/// </summary>
	event DataEventHandler<bool> GetCanNavigatePrevious;

	/// <summary>
	/// Zjišťuje, zda je možné přejít na následující záznam.
	/// </summary>
	event DataEventHandler<bool> GetCanNavigateNext;

}