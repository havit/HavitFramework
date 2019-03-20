namespace Havit.GoPay.DataObjects.Errors
{
	/// <summary>
	/// Výčet typů chyb vracející GoPay API
	/// </summary>
	public enum GoPayResponseErrorType
	{
		/// <summary>
		/// Systémová chyba
		/// </summary>
		System = 100,

		/// <summary>
		/// Povinný
		/// </summary>
		Required = 110,

		/// <summary>
		/// Chybný formát
		/// </summary>
		BadFormat = 111,

		/// <summary>
		/// Již existuje
		/// </summary>
		ExistsAlready = 112,

		/// <summary>
		/// Nelze změnit
		/// </summary>
		CannotEdit = 113,

		/// <summary>
		/// Nelze smazat
		/// </summary>
		CannotDelete = 114,

		/// <summary>
		/// Nejednoznačné
		/// </summary>
		Ambiguous = 115,

		/// <summary>
		/// Neoprávněný přístup
		/// </summary>
		Unauthorized = 200,

		/// <summary>
		/// Způsob přidělení práv není podporován
		/// </summary>
		NotSupportedAuthorizationGrant = 201,

		/// <summary>
		/// Chybné přístupové údaje
		/// </summary>
		BadCredentials = 202,

		/// <summary>
		/// Přístup přes PIN byl deaktivován
		/// </summary>
		AccessByPinDeactivated = 203,

		/// <summary>
		/// Platbu nelze vytvořit
		/// </summary>
		PaymentCannotBeCreated = 301,

		/// <summary>
		/// Platbu nelze provést
		/// </summary>
		PaymentCannotBeProcessed = 302,

		/// <summary>
		/// Platba v chybném stavu
		/// </summary>
		PaymentBadState = 303,

		/// <summary>
		/// Platbu nelze vrátit
		/// </summary>
		PaymentCannotBeRefunded = 330,

		/// <summary>
		/// Platbu nelze vrátit
		/// </summary>
		PaymentRefundNotSupported = 331,

		/// <summary>
		/// Chybná částka
		/// </summary>
		BadAmount = 332,

		/// <summary>
		/// Nedostatek peněz na účtu
		/// </summary>
		InsufficientFunds = 333,

		/// <summary>
		/// Provedení opakované platby selhalo
		/// </summary>
		RecurringPaymentFailed = 340,

		/// <summary>
		/// Provedení opakované platby není podporováno
		/// </summary>
		RecurringPaymentNotSupported = 341,

		/// <summary>
		/// Opakování platby zastaveno
		/// </summary>
		RecurringPaymentStopped = 342,

		/// <summary>
		/// Překročen časový limit počtu provedení opakované platby
		/// </summary>
		RecurringPaymentTimeLimitExceeded = 343,

		/// <summary>
		/// Stržení platby selhalo
		/// </summary>
		PaymentCaptureFailed = 350,

		/// <summary>
		/// Stržení platby provedeno
		/// </summary>
		PaymentCaptureSuccess = 351,

		/// <summary>
		/// Zrušení předautorizace selhalo
		/// </summary>
		CancelingPreauthorizationFailed = 352,

		/// <summary>
		/// Zrušení předautorizace provedeno
		/// </summary>
		CancelingPreauthorizationSuccess = 353
	}
}