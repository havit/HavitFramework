namespace Havit.PayMuzo;

/// <summary>
/// Třída pro zpracování odezvy webové služby ORDER STATE.
/// </summary>
public class PayMuzoOrderStateWebServiceResponse : PayMuzoOrderWebServiceResponse
{
	/// <summary>
	/// Stav objednávky PayMUZO.
	/// </summary>
	public PayMuzoOrderState OrderState
	{
		get { return _orderState; }
		protected set { _orderState = value; }
	}
	private PayMuzoOrderState _orderState;

	/// <summary>
	/// Initializes a new instance of the <see cref="PayMuzoOrderStateWebServiceResponse"/> class.
	/// </summary>
	/// <param name="response">The response.</param>
	public PayMuzoOrderStateWebServiceResponse(Havit.PayMuzo.WebServiceProxies.OrderStateResponse response)
		: base(response)
	{
	}

	/// <summary>
	/// Vytahá z web-servicové response data do properties.
	/// </summary>
	/// <param name="response">odpověď z WebService</param>
	public override void ParseResponse(Havit.PayMuzo.WebServiceProxies.Response response)
	{
		base.ParseResponse(response);

		Havit.PayMuzo.WebServiceProxies.OrderStateResponse orderStateResponse = (Havit.PayMuzo.WebServiceProxies.OrderStateResponse)response;

		this.OrderState = (PayMuzoOrderState)orderStateResponse.state;
	}

	/// <summary>
	/// Vytvoří normalizovaná data pro ověření podpisu.
	/// </summary>
	/// <param name="response">odpověď z WebService</param>
	public override PayMuzoRequestData CreateNormalizedData(Havit.PayMuzo.WebServiceProxies.Response response)
	{
		PayMuzoRequestData data = new PayMuzoRequestData();

		data.Add("orderNumber", this.OrderNumber.ToString());
		data.Add("state", this.OrderState.ToString("d"));
		data.Add("primaryReturnCode", this.PrimaryReturnCode.Value.ToString());
		data.Add("secondaryReturnCode", this.SecondaryReturnCode.Value.ToString());

		return data;
	}
}
