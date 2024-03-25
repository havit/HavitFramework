namespace Havit.Ares;

/// <summary>
/// Typ chyby v PlatceDphException
/// </summary>
public enum PlatceDphStatusCode
{
	/// Chyba WebService -  příliš mnoho Dic na vstupu
	MaxResultsExceeded = 1,
	/// Chyba WebService -  service dočasně nefunguje
	TechnologicalShutdown = 2,
	/// Chyba WebService -  služba neexistuje
	ServiceNotAvailable = 3,
	/// HttpClient Error 
	ConnectionError = -1,
	/// Chyba Parsování XML odpovědi (celé XML je součástí chyby) 
	XMLError = -2,
	/// Nepovolený/Neznámý status kód v odpovědi 
	BadStatusCode = -3
}
