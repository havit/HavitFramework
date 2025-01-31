using Havit.Diagnostics.Contracts;
using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects;

/// <summary>
/// Bázová třída reprezentující požadavek pro GoPay API
/// </summary>
public abstract class GoPayRequestBase
{
	/// <summary>
	/// Access token pro ověření autorizace požadavku
	/// </summary>
	[JsonIgnore]
	public string AccessToken { get; }

	/// <summary>
	/// Nastavení access tokenu pro ověření autorizace požadavku
	/// </summary>
	/// <param name="accessToken">Access token pro ověření autorizace požadavku</param>
	protected GoPayRequestBase(string accessToken)
	{
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(accessToken), nameof(accessToken));

		AccessToken = accessToken;
	}
}