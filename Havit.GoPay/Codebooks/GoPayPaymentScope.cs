namespace Havit.GoPay.Codebooks
{
	/// <summary>
	/// Scope funkcionalit, které je možné provádět v GoPay API pod příslušným tokenem
	/// </summary>
	public enum GoPayPaymentScope
	{
		/// <summary>
		/// Vytvoření platby
		/// </summary>
		PaymentCreate,

		/// <summary>
		/// Všechny dostupné operace nad platbami
		/// </summary>
		PaymentAll
	}
}
