using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Havit.PayMuzo;

/// <summary>
/// Abstraktní předek pro odezvy parsované z čisté datové podoby (další cestou je např. PayMuzoWebServiceResponse).
/// </summary>
public abstract class PayMuzoRawResponse : PayMuzoResponse
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PayMuzoResponse"/> class.
	/// </summary>
	/// <param name="rawResponseData">The response data, raw.</param>
	protected PayMuzoRawResponse(NameValueCollection rawResponseData)
	{
		NormalizedRawData = NormalizeData(rawResponseData);

		ParseResponseData(rawResponseData);
	}
	
	/// <summary>
	/// Normalizuje data na vstupu, do správného pořadí.
	/// </summary>
	/// <param name="rawResponseData">data k normalizaci</param>
	/// <returns>data v normalizované podobě</returns>
	public abstract PayMuzoRequestData NormalizeData(NameValueCollection rawResponseData);

	/// <summary>
	/// Rozparsuje data do strong-type properties.
	/// </summary>
	/// <param name="responseData">data požadavku</param>
	protected virtual void ParseResponseData(NameValueCollection responseData)
	{
		this.Digest = responseData["DIGEST"];

		string strPrCode = responseData["PRCODE"];
		if (!String.IsNullOrEmpty(strPrCode))
		{
			int tmp;
			if (Int32.TryParse(strPrCode, out tmp))
			{
				this.PrimaryReturnCode = PayMuzoPrimaryReturnCode.FindByValue(tmp);
			}
		}

		string strSrCode = responseData["SRCODE"];
		if (!String.IsNullOrEmpty(strSrCode))
		{
			int tmp;
			if (Int32.TryParse(strSrCode, out tmp))
			{
				this.SecondaryReturnCode = PayMuzoSecondaryReturnCode.FindByValue(tmp);
			}
		}
	}
}
