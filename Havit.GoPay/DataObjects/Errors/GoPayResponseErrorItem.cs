using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects.Errors;

/// <summary>
/// Chyba v odpovědi od GoPay
/// </summary>
public class GoPayResponseErrorItem
{
	/// <summary>
	/// Rozsah chyby
	/// </summary>
	[JsonProperty("scope")]
	public GoPayErrorItemScope Scope { get; internal set; }

	/// <summary>
	/// Chyba v poli
	/// </summary>
	[JsonProperty("field")]
	public string Field { get; internal set; }

	/// <summary>
	/// Chybová zpráva
	/// </summary>
	[JsonProperty("message")]
	public string Message { get; internal set; }

	/// <summary>
	/// Bližší popis chyby
	/// </summary>
	[JsonProperty("description")]
	public string Description { get; internal set; }

	/// <summary>
	/// Kód chyby
	/// </summary>
	[JsonProperty("error_code")]
	public int Code { get; internal set; }

	/// <summary>
	/// Název chyby
	/// </summary>
	[JsonProperty("error_name")]
	public string Name { get; internal set; }

	/// <summary>
	/// Typ chyby
	/// </summary>
	[JsonIgnore]
	public GoPayResponseErrorType ErrorType => (GoPayResponseErrorType)Code;
}
