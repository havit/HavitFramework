using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Havit.PayMuzo
{
	/// <summary>
	/// Třída pro sekundární návratový kód PayMUZO.
	/// </summary>
	public class PayMuzoSecondaryReturnCode : PayMuzoReturnCode
	{
		#region Return Codes
		public static PayMuzoSecondaryReturnCode None { get { return FindByValue(0); } }
		public static PayMuzoSecondaryReturnCode OrderNumber { get { return FindByValue(1); } }
		public static PayMuzoSecondaryReturnCode MerchantNumber { get { return FindByValue(2); } }
		public static PayMuzoSecondaryReturnCode Amount { get { return FindByValue(6); } }
		public static PayMuzoSecondaryReturnCode Currency { get { return FindByValue(7); } }
		public static PayMuzoSecondaryReturnCode DepositFlag { get { return FindByValue(8); } }
		public static PayMuzoSecondaryReturnCode MerchantOrderNumber { get { return FindByValue(10); } }
		public static PayMuzoSecondaryReturnCode CreditNumber { get { return FindByValue(11); } }
		public static PayMuzoSecondaryReturnCode Operation { get { return FindByValue(12); } }
		public static PayMuzoSecondaryReturnCode Batch { get { return FindByValue(18); } }
		public static PayMuzoSecondaryReturnCode Order { get { return FindByValue(22); } }
		public static PayMuzoSecondaryReturnCode Url { get { return FindByValue(24); } }
		public static PayMuzoSecondaryReturnCode MerchantData { get { return FindByValue(25); } }
		public static PayMuzoSecondaryReturnCode Description { get { return FindByValue(26); } }
		public static PayMuzoSecondaryReturnCode Digest { get { return FindByValue(34); } }
		public static PayMuzoSecondaryReturnCode DeclinedIn3DCardhodlerNotAuthenticated { get { return FindByValue(3000); } }
		public static PayMuzoSecondaryReturnCode Authenticated { get { return FindByValue(3001); } }
		public static PayMuzoSecondaryReturnCode DeclinedIn3DNotParticipating { get { return FindByValue(3002); } }
		public static PayMuzoSecondaryReturnCode DeclinedIn3DNotEnrolled { get { return FindByValue(3004); } }
		public static PayMuzoSecondaryReturnCode DeclinedIn3DTechnicalProblem1 { get { return FindByValue(3005); } }
		public static PayMuzoSecondaryReturnCode DeclinedIn3DTechnicalProblem2 { get { return FindByValue(3006); } }
		public static PayMuzoSecondaryReturnCode DeclinedIn3DAcquirerTechnicalProblem { get { return FindByValue(3007); } }
		public static PayMuzoSecondaryReturnCode DeclinedIn3DUnsupportedCard { get { return FindByValue(3008); } }
		public static PayMuzoSecondaryReturnCode DeclinedInAuthorizationCenterCardBlocked { get { return FindByValue(1001); } }
		public static PayMuzoSecondaryReturnCode DeclinedInAuthorizationCenterDeclined { get { return FindByValue(1002); } }
		public static PayMuzoSecondaryReturnCode DeclinedInAuthorizationCenterCardProblem { get { return FindByValue(1003); } }
		public static PayMuzoSecondaryReturnCode DeclinedInAuthorizationCenterTechnicalProblem { get { return FindByValue(1004); } }
		public static PayMuzoSecondaryReturnCode DeclinedInAuthorizationCenterAccountProblem { get { return FindByValue(1005); } }
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci <see cref="PayMuzoSecondaryReturnCode"/>.
		/// </summary>
		/// <param name="value">numeric code</param>
		/// <param name="csText">význam česky</param>
		/// <param name="enText">význam anglicky</param>
		protected PayMuzoSecondaryReturnCode(int value, string csText, string enText)
			: base(value, csText, enText)
		{
		}
		#endregion

		#region Static constructor
		/// <summary>
		/// Statický constructor
		/// </summary>
		static PayMuzoSecondaryReturnCode()
		{
			// PRCODE 1 - 5, 15 a 20
			RegisterCode(new PayMuzoSecondaryReturnCode(0, "", ""));
			RegisterCode(new PayMuzoSecondaryReturnCode(1, "ORDERNUMBER", "ORDERNUMBER"));
			RegisterCode(new PayMuzoSecondaryReturnCode(2, "MERCHANTNUMBER", "MERCHANTNUMBER"));
			RegisterCode(new PayMuzoSecondaryReturnCode(6, "AMOUNT", "AMOUNT"));
			RegisterCode(new PayMuzoSecondaryReturnCode(7, "CURRENCY", "CURRENCY"));
			RegisterCode(new PayMuzoSecondaryReturnCode(8, "DEPOSITFLAG", "DEPOSITFLAG"));
			RegisterCode(new PayMuzoSecondaryReturnCode(10, "MERORDERNUM", "MERORDERNUM"));
			RegisterCode(new PayMuzoSecondaryReturnCode(11, "CREDITNUMBER", "CREDITNUMBER"));
			RegisterCode(new PayMuzoSecondaryReturnCode(12, "OPERATION", "OPERATION"));
			RegisterCode(new PayMuzoSecondaryReturnCode(18, "BATCH", "BATCH"));
			RegisterCode(new PayMuzoSecondaryReturnCode(22, "ORDER", "ORDER"));
			RegisterCode(new PayMuzoSecondaryReturnCode(24, "URL", "URL"));
			RegisterCode(new PayMuzoSecondaryReturnCode(25, "MD", "MD"));
			RegisterCode(new PayMuzoSecondaryReturnCode(26, "DESC", "DESC"));
			RegisterCode(new PayMuzoSecondaryReturnCode(34, "DIGEST", "DIGEST"));

			// PRCODE 28
			RegisterCode(new PayMuzoSecondaryReturnCode(3000, "Neověřeno v 3D. Vydavatel karty není zapojen do 3D nebo karta nebyla aktivována. Kontaktujte vydavatele karty.", "Declined in 3D. Cardholder not authenticated in 3D. Contact your card issuer."));
			RegisterCode(new PayMuzoSecondaryReturnCode(3001, "Držitel karty ověřen.", "Authenticated."));
			RegisterCode(new PayMuzoSecondaryReturnCode(3002, "Neověřeno v 3D. Vydavatel karty nebo karta není zapojena do 3D. Kontaktuje vydavatele karty.", "Not authenticated id 3D. Issuer od Cerdholder not participating in 3D. Contact your card issuer."));
			RegisterCode(new PayMuzoSecondaryReturnCode(3004, "Neověřeno v 3D. Vydavatel karty není zapojen do 3D nebo karta nebyla aktivována. Kontaktujte vydavatele karty.", "Declined in 3D. Cardholder not authenticated in 3D. Contact your card issuer."));
			RegisterCode(new PayMuzoSecondaryReturnCode(3005, "Zamítnuto v 3D. Technický problém při ověření držitele karty. Kontaktujte vydavatele karty.", "Declined in 3D. Technical problem during Cardholder authentication. Contact yout card issuer."));
			RegisterCode(new PayMuzoSecondaryReturnCode(3006, "Zamítnuto v 3D. Technický problém při ověření držitele karty.", "Declined in 3D. Technical problem during cardholder authentication."));
			RegisterCode(new PayMuzoSecondaryReturnCode(3007, "Zamítnuto v 3D. Technický problém v systému zúčtující banky. Kontaktujte obchodníka.", "Declined in 3D. Acquier technical problem. Contact merchant."));
			RegisterCode(new PayMuzoSecondaryReturnCode(3008, "Zamítnuto v 3D. Použit nepodporovaný karetní produkt. Kontaktujte vydavatele karty.", "Declined in 3D. Unsupported card product. Contat your card issuer."));

			// PRCODE 30
			RegisterCode(new PayMuzoSecondaryReturnCode(1001, "Zamítnuto v autorizačním centru, karta blokována.", "Declined in AC, Card blocked"));
			RegisterCode(new PayMuzoSecondaryReturnCode(1002, "Zamítnuto v autorizačním centru, autorizace zamítnuta.", "Declined in AC, Declined"));
			RegisterCode(new PayMuzoSecondaryReturnCode(1003, "Zamítnuto v autorizačním centru, problém karty .", "Declined in AC, card problem."));
			RegisterCode(new PayMuzoSecondaryReturnCode(1004, "Zamítnuto v autorizačním centru, technický problém.", "Declined in AC, technical problem in authorization process."));
			RegisterCode(new PayMuzoSecondaryReturnCode(1005, "Zamítnuto v autorizačním centru, problém účtu.", "Declined in AC, accout problem"));
		}
		#endregion

		#region FindByValue
		/// <summary>
		/// Najde instanci podle numerické hodnoty kódu. Pokud není nalezen, vrací <c>null</c>.
		/// </summary>
		/// <param name="value">numerická hodnota kódu</param>
		public static PayMuzoSecondaryReturnCode FindByValue(int value)
		{
			return PayMuzoReturnCode.FindByValueInternal<PayMuzoSecondaryReturnCode>(value);
		}
		#endregion
	}
}
