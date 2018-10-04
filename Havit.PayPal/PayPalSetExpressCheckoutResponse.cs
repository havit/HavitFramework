using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Havit.PayPal
{
	/// <summary>
	/// Třída pro strong-type reprezentaci odpovědí z PayPal po volání SetExpressCheckout API.
	/// https://cms.paypal.com/us/cgi-bin/?&amp;cmd=_render-content&amp;content_ID=developer/e_howto_api_nvp_r_SetExpressCheckout
	/// </summary>
	public class PayPalSetExpressCheckoutResponse : PayPalResponseBase
	{
		#region Constructor
		/// <summary>
		/// Initializes a new instance of the PayPalSetExpressCheckoutResponse class.
		/// </summary>
		/// <param name="rawResponseData">The response data, raw.</param>
		public PayPalSetExpressCheckoutResponse(NameValueCollection rawResponseData)
			: base(rawResponseData)
		{
		}
		#endregion

		#region ParseResponseData
		/// <summary>
		/// Rozparsuje data do strong-type properties.		
		/// </summary>
		protected override void ParseResponseData(System.Collections.Specialized.NameValueCollection rawResponseData)
		{
			// Stačí zavolat base metodu. Tady nám jde jenom o TOKEN.
			base.ParseResponseData(rawResponseData);
		}
		#endregion	
	}
}
