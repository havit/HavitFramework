namespace Havit.Ares.FinancniSprava;

/// <summary>
/// Typ chyby v PlatceDphException
/// </summary>
public enum PlatceDphStatusCode
{
	// TODO: Jiný enum nebo vyhodit
	///// <summary>
	///// Chyba WebService -  příliš mnoho Dic na vstupu
	///// </summary>
	//MaxResultsExceeded = 1,

	///// <summary>
	///// Chyba WebService -  service dočasně nefunguje
	///// </summary>
	//TechnologicalShutdown = 2,

	///// <summary>
	///// Chyba WebService -  služba neexistuje
	///// </summary>
	//ServiceNotAvailable = 3,

	/// <summary>
	/// HttpClient Error 
	/// </summary>
	ConnectionError = -1,

	/// <summary>
	/// Chyba Parsování XML odpovědi (celé XML je součástí chyby) 
	/// </summary>
	XmlError = -2,

	/// <summary>
	/// Nepovolený/Neznámý status kód v odpovědi 
	/// </summary>
	BadStatusCode = -3
}
