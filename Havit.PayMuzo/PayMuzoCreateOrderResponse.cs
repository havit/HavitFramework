using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Havit.PayMuzo
{
	/// <summary>
	/// Třída pro strong-type reprezentaci odpovědí z PayMUZO na request CREATE_ORDER (<see cref="PayMuzoHelper.CreateOrder"/>).
	/// </summary>
	public class PayMuzoCreateOrderResponse : PayMuzoRawResponse
	{
		/// <summary>
		/// Číslo objednávky na straně PayMUZO.
		/// </summary>
		public int? OrderNumber
		{
			get { return _orderNumber; }
		}
		private int? _orderNumber;

		/// <summary>
		/// Číslo objednávky na straně obchodníka (na naší straně).
		/// </summary>
		public int? MerchantOrderNumber
		{
			get { return _merchantOrderNumber; }
		}
		private int? _merchantOrderNumber;

		/// <summary>
		/// Data obchodníka (context).
		/// </summary>
		public string MerchantData
		{
			get { return _merchantData; }
		}
		private string _merchantData;

		/// <summary>
		/// Textová informace o výsledku operace.
		/// </summary>
		public string ResultText
		{
			get { return _resultText; }
			set { _resultText = value; }
		}
		private string _resultText;

		/// <summary>
		/// Gets a value indicating whether this instance is digest URL encoded.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is digest URL encoded; otherwise, <c>false</c>.
		/// </value>
		public override bool IsDigestUrlEncoded
		{
			get { return true; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PayMuzoCreateOrderResponse"/> class.
		/// </summary>
		/// <param name="rawResponseData">The response data, raw.</param>
		public PayMuzoCreateOrderResponse(NameValueCollection rawResponseData)
			: base(rawResponseData)
		{
		}

		/// <summary>
		/// Normalizuje data na vstupu, do správného pořadí.
		/// </summary>
		/// <param name="rawResponseData">data k normalizaci</param>
		/// <returns>data v normalizované podobě</returns>
		public override PayMuzoRequestData NormalizeData(NameValueCollection rawResponseData)
		{
			PayMuzoRequestData data = new PayMuzoRequestData();

			if (rawResponseData["OPERATION"] != null)
			{
				data.Add("OPERATION", rawResponseData["OPERATION"]);
			}
			else
			{
				data.Add("OPERATION", String.Empty);
			}

			if (rawResponseData["ORDERNUMBER"] != null)
			{
				data.Add("ORDERNUMBER", rawResponseData["ORDERNUMBER"]);
			}
			else
			{
				data.Add("ORDERNUMBER", String.Empty);
			}

			if (rawResponseData["MERORDERNUM"] != null)
			{
				data.Add("MERORDERNUM", rawResponseData["MERORDERNUM"]);
			}
			// Navzdory dokumentaci vyjde ověření podpisu správně, pokud pole v datech chybí a není použito "||"
			//else
			//{
			//    data.Add("MERORDERNUM", String.Empty);
			//}

			if (rawResponseData["MD"] != null)
			{
				data.Add("MD", rawResponseData["MD"]);
			}
			// Navzdory dokumentaci vyjde ověření podpisu správně, pokud pole v datech chybí a není použito "||"
			//else
			//{
			//    data.Add("MD", String.Empty);
			//}

			if (rawResponseData["PRCODE"] != null)
			{
				data.Add("PRCODE", rawResponseData["PRCODE"]);
			}
			else
			{
				data.Add("PRCODE", String.Empty);
			}

			if (rawResponseData["SRCODE"] != null)
			{
				data.Add("SRCODE", rawResponseData["SRCODE"]);
			}
			else
			{
				data.Add("SRCODE", String.Empty);
			}

			if (rawResponseData["RESULTTEXT"] != null)
			{
				data.Add("RESULTTEXT", rawResponseData["RESULTTEXT"]);
			}
			// Nepovinné, proto analogicky jako výše předpokládáme nenahrazování ||. Nebylo ověřeno.
			//else
			//{
			//    data.Add("RESULTTEXT", String.Empty);
			//}

			return data;
		}

		/// <summary>
		/// Rozparsuje data do strong-type properties.
		/// </summary>
		/// <param name="responseData">data požadavku</param>
		protected override void ParseResponseData(NameValueCollection responseData)
		{
			base.ParseResponseData(responseData);

			// ORDERNUMBER
			string strOrderNumber = responseData["ORDERNUMBER"];
			if (!String.IsNullOrEmpty(strOrderNumber))
			{
				int tmp;
				if (Int32.TryParse(strOrderNumber, out tmp))
				{
					_orderNumber = tmp;
				}
			}

			// MERORDERNUM
			string strMerchantOrderNumber = responseData["MERORDERNUM"];
			if (!String.IsNullOrEmpty(strMerchantOrderNumber))
			{
				int tmp;
				if (Int32.TryParse(strMerchantOrderNumber, out tmp))
				{
					_merchantOrderNumber = tmp;
				}
			}

			// MD
			_merchantData = responseData["MD"];

			// RESULTTEXT
			_resultText = responseData["RESULTTEXT"];
		}
	}
}
