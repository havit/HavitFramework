using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.PayPal
{
	/// <summary>
	/// Enum popisující typy PaymentAction (Handling Payment Settlements).
	/// </summary>
	public enum PayPalPaymentAction
	{
		/// <summary>
		/// A sale payment action represents a single payment that completes a purchase for a specified amount.
		/// </summary>
		Sale,

		/// <summary>
		/// An authorization payment action represents an agreement to pay and places the buyer’s funds on hold for up to three days.
		/// </summary>
		Authorization,

		/// <summary>
		/// An order payment action represents an agreement to pay one or more authorized amounts up to the specified total over a maximum of 29 days.
		/// </summary>		
		Order
	}

	/// <summary>
	/// Pomocná třída na práci s enumem PayPalPaymentAction.
	/// </summary>
	public static class PayPalPaymentActionHelper
	{
		#region GetPayPalPaymentActionCode
		/// <summary>
		/// Constructor.
		/// </summary>
		public static string GetPayPalPaymentActionCode(PayPalPaymentAction action)
		{
			switch (action)
			{
				case PayPalPaymentAction.Authorization:
					return "Authorization";
				case PayPalPaymentAction.Order:
					return "Order";
				case PayPalPaymentAction.Sale:
				default:
					return "Sale";
			}
		}
		#endregion		
	}
}
