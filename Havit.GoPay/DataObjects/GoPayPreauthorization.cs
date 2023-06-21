using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.GoPay.Codebooks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects;

/// <summary>
/// Reprezentuje stav předautorizace platby
/// </summary>
public class GoPayPreauthorization
{
	/// <summary>
	/// Indikuje zdali bylo o předautorizovanou platbu zažádáno;
	/// </summary>
	[JsonProperty("requested")]
	public bool Requested { get; set; }

	/// <summary>
	/// Stav předautorizace
	/// </summary>
	[JsonProperty("state")]
	public GoPayPaymentState? State { get; set; }
}
