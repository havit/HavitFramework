using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.PayPal
{
	/// <summary>
	/// Enum popisující typ PayPal page, která se má zobrazit uživateli po příchodu na PayPal.
	/// </summary>
	public enum PayPalLandingPage
	{
		/// <summary>
		/// Billing (nalevo bude možnost platit kartou s možností vytvoření PayPal účtu a napravo bude dialog pro přihlášení).
		/// </summary>
		Billing,

		/// <summary>
		/// Zobrazí se pouze formulář na přihlášení.
		/// </summary>
		Login
	}

	/// <summary>
	/// Pomocná třída na práci s enumem PayPalLandingPage.
	/// </summary>
	public static class PayPalLandingPageHelper
	{
		#region GetPayPalLandingPageCode
		/// <summary>
		/// Contructor.
		/// </summary>
		public static string GetPayPalLandingPageCode(PayPalLandingPage landingPage)
		{
			switch (landingPage)
			{
				case PayPalLandingPage.Billing:
					return "Billing";
				case PayPalLandingPage.Login:
				default:
					return "Login";
			}
		}
		#endregion
	}
}
