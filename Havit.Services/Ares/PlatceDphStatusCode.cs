namespace Havit.Services.Ares;

public enum PlatceDphStatusCode
{
	Ok = 0,
	MaxResultsExceeded = 1,
	TechnologicalShutdown = 2,
	ServiceNotAvailable = 3,
	ConnectionError = -1,
	XMLError = -2,
	BadStatusCode = -3,
	InputParamError = -4
}
